using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Web.Http;
using Matrix_UWP.Helpers;
using Windows.Storage.Streams;
using System.Collections.Generic;

namespace Matrix_UWP {
  namespace Model {
    class MatrixRequest {
      static private HttpJsonRequest json_req = new HttpJsonRequest();
      static private MatrixHttpRequest normal_req = new MatrixHttpRequest();
      static private Random rand = new Random();
      private const string root = "https://vmatrix.org.cn";
      static async private Task<MatrixRequestResult> GetAsync(string uri, string query = "") {
        string uri_str;
        if (query != null) {
          query = (query.Length == 0 ? "" : $"&{query}");
          uri_str = $"{uri}?t={rand.Next()}{query}";
        } else {
          uri_str = $"{uri}";
        }
        var obj = await json_req.GetAsync(new Uri(uri_str));
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
      static async private Task<MatrixRequestResult> PostAsync(string uri, JObject body) {
        var obj = await json_req.PostAsync(new Uri(uri), body);
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
      static async private Task<MatrixRequestResult> PutAsync(string uri, JObject body) {
        var obj = await json_req.PutAsync(new Uri(uri), body);
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
      static async public Task<User> Login(string username, string password, string captcha = "") {
        var body = new JObject {
          ["username"] = username,
          ["password"] = password
        };
        if (captcha.Length > 0) {
          body["captcha"] = captcha;
        }
        var loginResult = await PostAsync($"{root}/api/users/login", body);
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
        var profile = await GetAsync($"{root}/api/users/profile");
        if (profile.success) {
          profile.data["is_valid"] = loginResult.data["is_valid"];
          return new User(profile.data);
        }
        throw new MatrixException.SoftError(profile);
      }

      static async public Task<bool> IsLogin() {
        MatrixRequestResult result = null;
        result = await GetAsync($"{root}/api/users/login");
        return result.success;
      }

      static async public Task<bool> logout() {
        MatrixRequestResult result = null;
        result = await PostAsync($"{root}/api/users/logout", new JObject());
        return result.success;
      }

      static async public Task<User> GetProfile() {
        var result = await GetAsync($"{root}/api/users/profile");
        if (result.success) {
          return new User(result.data);
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task ChangeProfile(string nickname, string email, string phone, string homepage) {
        var body = new JObject {
          ["nickname"] = nickname
        };
        if (email.Length > 0) {
          body["email"] = email;
        }
        if (phone.Length > 0) {
          body["phone"] = phone;
        }
        if (homepage.Length > 0) {
          body["homepage"] = homepage;
        }

        var result = await PostAsync($"{root}/api/users/profile", body);
        if (result.success) {
          return;
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task ChangePassword(string oldPassword, string newPassword, string newConfirmPassword) {
        var body = new JObject {
          ["old_password"] = oldPassword,
          ["new_password"] = newPassword,
          ["new_confirm_password"] = newConfirmPassword
        };
        var result = await PostAsync($"{root}/api/users/password", body);
        if (result.success) {
          return;
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task ForgetPassword(string email) {
        var body = new JObject {
          ["email"] = email
        };
        var result = await PostAsync($"{root}/api/mail/password", body);
        if (result.success) {
          return;
        }
        throw new MatrixException.SoftError(result);
      }

      static public Uri GetAvatarUri(string username = "undefined") {
        return new Uri($"{root}/api/users/profile/avatar?t={rand.Next()}&username={username}");
      }

      static async public Task<string> GetCaptcha() {
        var result = await GetAsync($"{root}/api/captcha");
        if (result.success) {
          return Helpers.Nullable.ToString(result.data["captcha"]);
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task<List<Course>> GetCourseList() {
        return await GetListAsync<Course>($"{root}/api/courses");
      }

      static async public Task<Course> GetCourse(int course_id) {
        var result = await GetAsync($"{root}/api/courses");
        var data = new JObject();
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        var arr = result.data as JArray;
        foreach (JObject one in arr) {
          if (Helpers.Nullable.ToInt(one["course_id"]) == course_id) {
            data = one;
            break;
          }
        }
        result = await GetAsync($"{root}/api/courses/{course_id}");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        data["description"] = result.data["description"];
        return new Course(data);
      }

      static public async Task ChangeMsgState(int msg_id, bool? is_read) {
        var body = new JObject {
          ["status"] = is_read ?? false,
        };
        var ids = new JArray {
          msg_id
        };
        body["id"] = ids;
        var result = await PutAsync($"{root}/api/notifications/status", body);
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
      }

      static public async Task ReadAllNotifications() {
        var body = new JObject() {
          ["status"] = true,
        };
        var result = await PutAsync($"{root}/api/notifications/statuses", body);
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
      }

      static async public Task<List<Notification>> GetNotificationList() {
        var result = await GetAsync($"{root}/api/notifications");
        if (result.success) {
          var notifications = new List<Notification>();
          foreach (JToken notification in result.data["notifications"] as JArray) {
            notifications.Add(new Notification(notification));
          }
          return notifications;
        }
        throw new MatrixException.SoftError(result);
      }

      static async public Task<List<Assignment>> GetAssignmentList(int course_id) {
        return await GetListAsync<Assignment>($"{root}/api/courses/{course_id}/assignments");
      }

      static async public Task<Assignment> GetAssignment(int course_id, int ca_id) {
        var result = await GetAsync($"{root}/api/courses/{course_id}/assignments/{ca_id}");
        if (!result.success) {
          throw new MatrixException.SoftError(result);
        }
        return new Assignment(result.data);
      }

      static async public Task<List<Library>> GetLibraryList() {
        return await GetListAsync<Library>($"{root}/api/libraries");
      }

      static async public Task<List<Assignment>> GetUnjudgeAssignment() {
        return await GetListAsync<Assignment>($"{root}/api/courses/assignments?state=started&waitingForMyJudging=1", null);
      }

      static async public Task<List<Assignment>> GetUnfinishAssignment() {
        return await GetListAsync<Assignment>($"{root}/api/courses/assignments?state=progressing&unsubmitted=1&notFullGrade=1", null);
      }

      static async public Task<string> GetMatrixNotification() {
        HttpResponseMessage response;
        response = await normal_req.GetAsync(new Uri($"{root}/data/notification.md"));
        IBuffer buffer = await response.Content.ReadAsBufferAsync();
        DataReader reader = DataReader.FromBuffer(buffer);
        return reader.ReadString(buffer.Length);
      }

      static async public Task<int> SubmitProgramming(int course_id, int ca_id, List<CodeFile> submissions) {
        JArray answers = new JArray();
        foreach (var submission in submissions) {
          answers.Add(new JObject() {
            ["code"] = submission.Code,
            ["name"] = submission.Name,
          });
        }
        JObject body = new JObject() {
          ["detail"] = new JObject() {
            ["answers"] = answers,
          }
        };
        var result = await PostAsync($"{root}/api/courses/{course_id}/assignments/{ca_id}/submissions", body);
        if (result.success) return Helpers.Nullable.ToInt(result.data["sub_asgn_id"]);
        throw new MatrixException.SoftError(result);
      }

      static public Task<List<Submission>> GetSubmissionList(int course_id, int ca_id) {
        return GetListAsync<Submission>($"{root}/api/courses/{course_id}/assignments/{ca_id}/submissions");
      }

      static async public Task<List<T>> GetListAsync<T>(string uri, string query = "") where T : class {
        MatrixRequestResult result = await GetAsync(uri, query);
        if (result.success) {
          JArray arr = result.data as JArray;
          List<T> ret = new List<T>();
          foreach (JObject one in arr) {
            // 软爸爸的C#泛型类不能初始化，实在是很蛋疼啊。
            // 还好软爸爸留了一条退路。
            // Waiting for proposal: Extend generic type new() constraint with parameter types. https://github.com/dotnet/csharplang/issues/769
            ret.Add(Activator.CreateInstance(typeof(T), one) as T);
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
      public string Captcha {
        get;
      }
      public WrongCaptcha(Model.MatrixRequestResult result) : base("验证码错误") {
        JObject data = result.data as JObject;
        Captcha = Helpers.Nullable.ToString(result.data["captcha"]);
      }
    }
  }
}
