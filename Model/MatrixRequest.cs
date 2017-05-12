using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP {
  namespace Model {
    class MatrixRequest {
      static private Helpers.HttpJsonRequest req = new Helpers.HttpJsonRequest();
      static private Random rand = new Random();
      private const string root = "https://vmatrix.org.cn";
      static async private Task<MatrixRequestResult> getAsync(string uri, string query = "") {
        query = (query.Length == 0 ? "" : $"&{query}");
        var obj = await req.getAsync(new Uri($"{uri}?t={rand.Next()}{query}"));
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
        var obj = await req.postAsync(new Uri(uri), body);
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
        var loginResult = await postAsync($"{root}/api/users/login", body);
        if (!loginResult.success) {
          switch (loginResult.status) {
            case "NOT_FOUND":
            case "WRONG_PASSWORD":
              throw new MatrixException.WrongPassword();
            case "WRONG_CAPTCHA":
              throw new MatrixException.WrongCaptcha(loginResult);
            default:
              break;
          }
          throw new MatrixException.SoftError(loginResult);
        }
        var profile = await getAsync($"{root}/api/users/profile");
        if (profile.success) {
          profile.data["is_valid"] = loginResult.data["is_valid"];
          return new User(profile.data);
        }
        throw new MatrixException.SoftError(profile);
      }

      static async public Task<bool> isLogin() {
        MatrixRequestResult result = null;
        try {
          result = await getAsync($"{root}/api/users/login");
        } catch (MatrixException.FatalError err) {
          throw err;
        }
        return result.success;
      }

      static async public Task<bool> logout() {
        MatrixRequestResult result = null;
        try {
          result = await postAsync($"{root}/api/users/logout", new JObject());
        } catch (MatrixException.FatalError err) {
          throw err;
        }
        return result.success;
      }

      static async public Task<User> getProfile() {
        var result = await getAsync($"{root}/api/users/profile");
        if (result.success) {
          return new User(result.data);
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task changeProfile(string nickname, string email, string phone, string homepage) {
        var body = new JObject();
        body["nickname"] = nickname;
        body["email"] = email;
        if (phone.Length > 0) {
          body["phone"] = phone;
        } else {
          body["phone"] = null;
        }
        if (homepage.Length > 0) {
          body["homepage"] = homepage;
        } else {
          body["homepage"] = null;
        }
        
        var result = await postAsync($"{root}/api/users/profile", body);
        if (result.success) {
          return;
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task changePassword(string oldPassword, string newPassword, string newConfirmPassword) {
        var body = new JObject();
        body["old_password"] = oldPassword;
        body["new_password"] = newPassword;
        body["new_confirm_password"] = newConfirmPassword;
        var result = await postAsync($"{root}/api/users/password", body);
        if (result.success) {
          return;
        }
        throw new MatrixException.SoftError(result);
      }

      static public Uri getAvatarUri(string username = "undefined") {
        return new Uri($"{root}/api/users/profile/avatar?t={rand.Next()}&username={username}");
      }

      static async public Task<Captcha> getCaptcha() {
        var result = await getAsync($"{root}/api/captcha");
        if (result.success) {
          return new Captcha(result.data);
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task<ObservableCollection<Course>> getCourseList() {
        var result = await getAsync($"{root}/api/courses");
        if (result.success) {
          var arr = result.data as JArray;
          var ret = new ObservableCollection<Course>();
          foreach (JObject one in arr) {
            ret.Add(new Course(one));
          }
          return ret;
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task<Course> getCourse(int course_id) {
        var result = await getAsync($"{root}/api/courses");
        var data = new JObject();
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        var arr = result.data as JArray;
        foreach (JObject one in arr) {
          if (Helpers.Nullable.toInt(one["course_id"]) == course_id) {
            data = one;
            break;
          }
        }
        result = await getAsync($"{root}/api/courses/{course_id}");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        data["description"] = result.data["description"];
        return new Course(data);
      }

      static public async Task changeMsgState(int msg_id, bool? is_read) {
        var body = new JObject();
        body["status"] = is_read == true ? 1 : 0;
        var ids = new JArray();
        ids.Add(msg_id);
        body["ids"] = ids;
        var result = await postAsync($"{root}/api/notification/msgStatusChange", body);
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
      }

      static async public Task<ObservableCollection<Notification>> getNotificationList() {
        var result = await getAsync($"{root}/api/notification/unReadMessages");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        var arr = result.data as JArray;
        var ret = new ObservableCollection<Notification>();
        foreach (JObject one in arr) {
          ret.Add(new Notification(one));
        }
        result = await getAsync($"{root}/api/notification/readedMessages");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        arr = result.data as JArray;
        foreach (JObject one in arr) {
          ret.Add(new Notification(one));
        }
        return ret;
      }

      static async public Task<ObservableCollection<Assignment>> getAssignmentList(int course_id) {
        var result = await getAsync($"{root}/api/courses/{course_id}/assignments");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        var arr = result.data as JArray;
        var ret = new ObservableCollection<Assignment>();
        foreach (JObject one in arr) {
          ret.Add(new Assignment(one));
        }
        return ret;
      }

      static async public Task<Assignment> getAssignment(int course_id, int ca_id) {
        var result = await getAsync($"{root}/api/courses/{course_id}/assignments/{ca_id}");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        return new Assignment(result.data);
      }

      static async public Task<ObservableCollection<Library>> getLibraryList() {
        var result = await getAsync($"{root}/api/libraries");
        if (result.success) {
          var arr = result.data as JArray;
          var ret = new ObservableCollection<Library>();
          foreach (JObject one in arr) {
            ret.Add(new Library(one));
          }
          return ret;
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
        Debug.WriteLine($"响应不 OK: {JsonConvert.SerializeObject(result, Formatting.Indented)}");
      }
    }

    class UserNotFound : SoftError {
      public UserNotFound() : base("找不到该用户") { }
    }

    class WrongPassword : SoftError {
      public WrongPassword() : base("密码错误") { }
    }

    class WrongCaptcha : SoftError {
      public Model.Captcha captcha {
        get;
      }
      public WrongCaptcha(Model.MatrixRequestResult result) : base("验证码错误") {
        JObject data = result.data as JObject;
        this.captcha = new Model.Captcha(result.data);
      }
    }
  }
}
