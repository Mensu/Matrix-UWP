using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  class Course : BindableBase {
    public Course(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      var data = token as JObject;
      this.course_id = Helpers.Nullable.toInt(data["course_id"]);
      this.name = Helpers.Nullable.toString(data["name"]);
      this.creator = new User(data["creator"]);
      this.progressingNum = Helpers.Nullable.toInt(data["progressing_num"]);
      this.unfinishedNum = Helpers.Nullable.toInt(data["unfinished_num"]);
      this.role = Helpers.Nullable.toString(data["role"], "student");
      this.semester = Helpers.Nullable.toString(data["semester"]);
      this.isOpen = Helpers.Nullable.toString(data["status"], "open") == "open";
      this.stuNum = Helpers.Nullable.toInt(data["student_num"]);
      this.description = Helpers.Nullable.toString(data["description"]);
      this.teacher = Helpers.Nullable.toString(data["teacher"]);
    }
    public int course_id {
      get;
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

    private User _creator;
    public User creator {
      get {
        return this._creator;
      }
      set {
        this.SetProperty(ref this._creator, value);
      }
    }

    private int _progressingNum;
    public int progressingNum {
      get {
        return this._progressingNum;
      }
      set {
        this.SetProperty(ref this._progressingNum, value);
      }
    }

    private int _unfinishedNum;
    public int unfinishedNum {
      get {
        return this._unfinishedNum;
      }
      set {
        this.SetProperty(ref this._unfinishedNum, value);
      }
    }

    private string _role;
    public string role {
      get {
        return this._role;
      }
      set {
        this.SetProperty(ref this._role, value);
      }
    }

    private string _semester;
    public string semester {
      get {
        return this._semester;
      }
      set {
        this.SetProperty(ref this._semester, value);
      }
    }

    private bool _isOpen;
    public bool isOpen {
      get {
        return this._isOpen;
      }
      set {
        this.SetProperty(ref this._isOpen, value);
      }
    }

    private int _stuNum;
    public int stuNum {
      get {
        return this._stuNum;
      }
      set {
        this.SetProperty(ref this._stuNum, value);
      }
    }

    private string _teacher;
    public string teacher {
      get {
        return this._teacher;
      }
      set {
        this.SetProperty(ref this._teacher, value);
      }
    }

    private string _description;
    public string description {
      get {
        return this._description;
      }
      set {
        this.SetProperty(ref this._description, value);
      }
    }
  }
}
