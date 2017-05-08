using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  class Assignment : BindableBase {
    public Assignment(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      var data = token as JObject;
      this.course_id = Helpers.Nullable.toInt(data["course_id"]);
      this.ca_id = Helpers.Nullable.toInt(data["ca_id"]);
      this.startDate = Helpers.Nullable.toDateTimeOffset(data["startdate"], DateTimeOffset.MinValue);
      this.endDate = Helpers.Nullable.toDateTimeOffset(data["enddate"], DateTimeOffset.MinValue);
      this.name = Helpers.Nullable.toString(data["title"]);
      this.description = Helpers.Nullable.toString(data["description"]);
      this.creator = new User(data["author"]);
      this.type = this.getType(data["type"]);
    }

    public enum Type {
      RealtimeProgramming,
      ScheduleProgramming,
      Choice,
      Report,
      FileUpload,
      ProgrammingOutput,
      ProgrammingFillingBlank,
      ShortAnswer
    }

    public int course_id {
      get;
    }

    public int ca_id {
      get;
    }

    private DateTimeOffset _startDate;
    public DateTimeOffset startDate {
      get {
        return this._startDate;
      }
      set {
        this.SetProperty(ref this._startDate, value);
      }
    }

    private DateTimeOffset _endDate;
    public DateTimeOffset endDate {
      get {
        return this._endDate;
      }
      set {
        this.SetProperty(ref this._endDate, value);
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

    private string _description;
    public string description {
      get {
        return this._description;
      }
      set {
        this.SetProperty(ref this._description, value);
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

    private Type _type;
    public Type type {
      get {
        return this._type;
      }
      set {
        this.SetProperty(ref this._type, value);
      }
    }

    private Type getType(JToken token) {
      switch (token.ToString()) {
        case "Realtime Programming":
          return Type.RealtimeProgramming;
        case "Schedule Programming":
          return Type.ScheduleProgramming;
        case "Choice problem":
          return Type.Choice;
        case "Report":
          return Type.Report;
        case "Fileupload problem":
          return Type.FileUpload;
        case "Program Output problem":
          return Type.ProgrammingOutput;
        case "Program Blank Filling problem":
          return Type.ProgrammingFillingBlank;
        default:
          break;
      }
      return Type.ShortAnswer;
    }

  }
}
