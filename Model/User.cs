using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  class User : BindableBase {

    public User(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      JObject data = token as JObject;
      bool is_valid = Helpers.Nullable.toBool(data["is_valid"], true);
      if (is_valid == false) {
        throw new MatrixException.SoftError("登陆失败，请先去网页端验证邮箱");
      }
      this.user_id = Helpers.Nullable.toInt(data["user_id"]);
      this.username = Helpers.Nullable.toString(data["username"]);
      this.realname = Helpers.Nullable.toString(data["realname"]);
      this.nickname = Helpers.Nullable.toString(data["nickname"]);
      this.email = Helpers.Nullable.toString(data["email"]);
      this.academy = Helpers.Nullable.toString(data["academy"]);
      this.specialty = Helpers.Nullable.toString(data["specialty"]);
      this.joinDate = Helpers.Nullable.toDateTimeOffset(data["create_at"], DateTimeOffset.Now);
    }

    public int user_id {
      get;
    }

    private string _username;
    public string username {
      get {
        return this._username;
      }
      set {
        this.SetProperty(ref this._username, value);
      }
    }

    private string _nickname;
    public string nickname {
      get {
        return this._nickname;
      }
      set {
        this.SetProperty(ref this._nickname, value);
      }
    }

    private string _realname;
    public string realname {
      get {
        return this._realname;
      }
      set {
        this.SetProperty(ref this._realname, value);
      }
    }

    private string _email;
    public string email {
      get {
        return this._email;
      }
      set {
        this.SetProperty(ref this._email, value);
      }
    }

    private string _academy;
    public string academy {
      get {
        return this._academy;
      }
      set {
        this.SetProperty(ref this._academy, value);
      }
    }

    private string _specialty;
    public string specialty {
      get {
        return this._specialty;
      }
      set {
        this.SetProperty(ref this._specialty, value);
      }
    }

    private DateTimeOffset _joinDate;
    public DateTimeOffset joinDate {
      get {
        return this._joinDate;
      }
      set {
        this.SetProperty(ref this._joinDate, value);
      }
    }
  }
}
