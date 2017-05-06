using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Matrix_UWP.Model {
  class User : INotifyPropertyChanged {
    public event PropertyChangedEventHandler PropertyChanged;

    public int user_id {
      get;
    }

    private string _username;
    public string username {
      get {
        return this._username;
      }
      set {
        this._username = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("username"));
      }
    }

    private string _nickname;
    public string nickname {
      get {
        return this._nickname;
      }
      set {
        this._nickname = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("nickname"));
      }
    }

    private string _realname;
    public string realname {
      get {
        return this._realname;
      }
      set {
        this._realname = value;
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("realname"));
      }
    }
    public User(JToken token) {
      JObject data = token as JObject;
      bool is_valid = data["is_valid"].ToObject<bool>();
      if (is_valid == false) {
        throw new MatrixException.SoftError("登陆失败，请先去网页端验证邮箱");
      }
      this.user_id = data["user_id"].ToObject<int>();
      this.username = data["username"].ToString();
      this.realname = data["realname"].ToString();
      this.nickname = data["nickname"].ToString();
    }
  }
}
