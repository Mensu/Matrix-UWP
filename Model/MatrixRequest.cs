using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP {
  namespace Model {
    class MatrixRequest : Helpers.HttpJsonRequest {
      static private Helpers.HttpJsonRequest req = new Helpers.HttpJsonRequest();
      private const string root = "https://vmatrix.org.cn";
      static async private Task<MatrixRequestResult> getAsync(string uri) {
        JObject obj = await req.getAsync(new Uri(uri));
        var result = new MatrixRequestResult(obj);
        switch (result.status) {
          case "UNKNOWN_ERROR":
            throw new MatrixException.ServerError();
          case "NOT_AUTHORIZED":
            throw new MatrixException.NotLogin();
          default: break;
        }
        return result;
      }
      static async private Task<MatrixRequestResult> postAsync(string uri, JObject body) {
        JObject obj = await req.postAsync(new Uri(uri), body);
        var result = new MatrixRequestResult(obj);
        switch (result.status) {
          case "UNKNOWN_ERROR":
            throw new MatrixException.ServerError();
          case "NOT_AUTHORIZED":
            throw new MatrixException.NotLogin();
          default:
            break;
        }
        return result;
      }
      static async public Task<User> login(string username, string password, string captcha = "") {
        var body = new JObject();
        body["username"] = username;
        body["password"] = password;
        if (captcha.Length > 0) {
          body["captcha"] = captcha;
        }
        var result = await postAsync($"{root}/api/users/login", body);
        if (result.success) {
          return new User(result.data);
        }
        switch (result.status) {
          case "USER_NOT_FOUND":
          case "WRONG_PASSWORD":
            throw new MatrixException.WrongPassword();
          case "WRONG_CAPTCHA":
            throw new MatrixException.WrongCaptcha(result);
          default:
            break;
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task<Captcha> getCaptcha() {
        var result = await getAsync($"{root}/api/captcha");
        if (result.success) {
          return new Captcha(result.data);
        }
        if (result.status == "USER_NOT_FOUND" || result.status == "WRONG_PASSWORD") {
          throw new MatrixException.WrongPassword();
        }
        throw new MatrixException.SoftError(result);
      }
    }
  }

  namespace MatrixException {
    class ServerError : FatalError {
      public ServerError() : base("服务器出错") { }
    }

    class NotLogin : FatalError {
      public NotLogin() : base("登陆已过期，请重新登陆") { }
    }

    class SoftError : MatrixException {
      public SoftError(string message) : base(message) { }
      public SoftError(Model.MatrixRequestResult result) : base(result.msg) {
        Debug.WriteLine($"响应不 OK: {JsonConvert.SerializeObject(result)}");
      }
    }

    class WrongPassword : SoftError {
      public WrongPassword() : base("用户名或密码错误") { }
    }

    class WrongCaptcha : SoftError {
      private Model.Captcha _svg;
      public Model.Captcha svg {
        get {
          return _svg;
        }
      }
      public WrongCaptcha(Model.MatrixRequestResult result) : base("验证码错误") {
        JObject data = result.data as JObject;
        this._svg = new Model.Captcha(result.data);
      }
    }
  }
}
