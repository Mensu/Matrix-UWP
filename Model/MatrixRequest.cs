using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Matrix_UWP.Helpers;
using System.Text;
using Windows.Storage.Streams;

namespace Matrix_UWP {
  namespace Model {
    class MatrixRequest {
      static private Helpers.HttpJsonRequest json_req = new Helpers.HttpJsonRequest();
      static private Helpers.MatrixHttpRequest normal_req = new Helpers.MatrixHttpRequest();
      static private Random rand = new Random();
      private const string root = "https://vmatrix.org.cn";
      static async private Task<MatrixRequestResult> getAsync(string uri, string query = "") {
        string uri_str;
        if (query != null) {
          query = (query.Length == 0 ? "" : $"&{query}");
          uri_str = $"{uri}?t={rand.Next()}{query}";
        } else {
          uri_str = $"{uri}";
        }
        var obj = await json_req.getAsync(new Uri(uri_str));
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
        var obj = await json_req.postAsync(new Uri(uri), body);
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
        if (email.Length > 0) {
          body["email"] = email;
        }
        if (phone.Length > 0) {
          body["phone"] = phone;
        }
        if (homepage.Length > 0) {
          body["homepage"] = homepage;
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

      static async public Task forgetPassword(string email) {
        var body = new JObject();
        body["email"] = email;
        var result = await postAsync($"{root}/api/mail/password", body);
        if (result.success) {
          return;
        }
        throw new MatrixException.SoftError(result);
      }

      static public Uri getAvatarUri(string username = "undefined") {
        return new Uri($"{root}/api/users/profile/avatar?t={rand.Next()}&username={username}");
      }

      static async public Task<string> getCaptcha() {
        var result = await getAsync($"{root}/api/captcha");
        if (result.success) {
          return Helpers.Nullable.toString(result.data["captcha"]);
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task<ObservableCollection<Course>> getCourseList() {
       return await getListAsync<Course>($"{root}/api/courses");
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
        var ret = await getListAsync<Notification>($"{root}/api/notification/unReadMessages");
        var read = await getListAsync<Notification>($"{root}/api/notification/readedMessages");
        foreach (var notification in read) {
          ret.Add(notification);
        }
        return ret;
      }

      static async public Task<ObservableCollection<Assignment>> getAssignmentList(int course_id) {
       return await getListAsync<Assignment>($"{root}/api/courses/{course_id}/assignments");
      }

      static async public Task<Assignment> getAssignment(int course_id, int ca_id) {
        var result = await getAsync($"{root}/api/courses/{course_id}/assignments/{ca_id}");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        return new Assignment(result.data);
      }

      static async public Task<ObservableCollection<Library>> getLibraryList() {
        return await getListAsync<Library>($"{root}/api/libraries");
      }

      static async public Task<ObservableCollection<Assignment>> getUnjudgeAssignment() {
        return await getListAsync<Assignment>($"{root}/api/courses/assignments?state=started&waitingForMyJudging=1", null);
      }

      static async public Task<ObservableCollection<Assignment>> getUnfinishAssignment() {
        return await getListAsync<Assignment>($"{root}/api/courses/assignments?state=progressing&unsubmitted=1&notFullGrade=1", null);
      }

      static async public Task<string> getMatrixNotification() {
        HttpResponseMessage response;
        response = await normal_req.getAsync(new Uri($"{root}/data/notification.md"));
        IBuffer buffer = await response.Content.ReadAsBufferAsync();
        DataReader reader = DataReader.FromBuffer(buffer);
        return reader.ReadString(buffer.Length);
      }

      static async public Task<ObservableCollection<T>> getListAsync<T> (string uri, string query="") {
        MatrixRequestResult result = await getAsync(uri, query);
        if (result.success) {
          JArray arr = result.data as JArray;
          ObservableCollection<T> ret = new ObservableCollection<T>();
          foreach (JObject one in arr) {
            // 软爸爸的C#泛型类不能初始化，实在是很蛋疼啊。
            // 还好软爸爸留了一条退路。
            ret.Add((T)Activator.CreateInstance(typeof(T), one));
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
      public string captcha {
        get;
      }
      public WrongCaptcha(Model.MatrixRequestResult result) : base("验证码错误") {
        JObject data = result.data as JObject;
        this.captcha = Helpers.Nullable.toString(result.data["captcha"]);
      }
    }
  }
}
