using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  public class Library : BindableBase {
    public Library(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      var data = token as JObject;
      this.lib_id = Helpers.Nullable.toInt(data["lib_id"]);
      this.owner = Helpers.Nullable.toString(data["creator"]);
      this.name = Helpers.Nullable.toString(data["name"]);
      this.problemNum = Helpers.Nullable.toInt(data["problem_num"]);
      this.memberNum = Helpers.Nullable.toInt(data["member_num"]);
      this.createdDate = Helpers.Nullable.toDateTimeOffset(data["created_at"], DateTimeOffset.MinValue);
    }

    public int lib_id {
      get;
    }

    private string _owner;
    public string owner {
      get {
        return this._owner;
      }
      set {
        this.SetProperty(ref this._owner, value);
      }
    }

    private string _name;
    public string name {
      get {
        return this._name;
      }
      set {
        this.SetProperty(ref this._name, value);
      }
    }

    private int _problemNum;
    public int problemNum {
      get {
        return this._problemNum;
      }
      set {
        this.SetProperty(ref this._problemNum, value);
      }
    }

    private int _memberNum;
    public int memberNum {
      get {
        return this._memberNum;
      }
      set {
        this.SetProperty(ref this._memberNum, value);
      }
    }

    private DateTimeOffset _createdDate;
    public DateTimeOffset createdDate {
      get {
        return this._createdDate;
      }
      set {
        this.SetProperty(ref this._createdDate, value);
      }
    }
  }
}
