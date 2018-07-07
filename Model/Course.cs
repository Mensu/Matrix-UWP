using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  public class Course : BindableBase {
    public enum UserRole {
      Teacher,
      TA,
      Student,
      Undefined,
    };
    public Course(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      var data = token as JObject;
      this.course_id = Helpers.Nullable.ToInt(data["course_id"]);
      this.name = Helpers.Nullable.ToString(data["course_name"]);
      this.creator = new User(data["creator"]);
      this.progressingNum = Helpers.Nullable.ToInt(data["progressing_num"]);
      this.unfinishedNum = Helpers.Nullable.ToInt(data["unfinished_num"]);
      this.semester = Helpers.Nullable.ToString(data["semester"]);
      this.isOpen = Helpers.Nullable.ToString(data["status"], "open") == "open";
      this.stuNum = Helpers.Nullable.ToInt(data["student_num"]);
      this.description = Helpers.Nullable.ToString(data["description"]);
      this.teacher = Helpers.Nullable.ToString(data["teacher"]);

      string role = Helpers.Nullable.ToString(data["role"], "student");
      switch (role) {
        case "teacher":
          this.role = UserRole.Teacher;
          break;
        case "TA":
          this.role = UserRole.TA;
          break;
        case "student":
          this.role = UserRole.Student;
          break;
        default:
          this.role = UserRole.Undefined;
          break;
      }
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

    private UserRole _role;
    public UserRole role {
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
