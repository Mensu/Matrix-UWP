using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prism.Mvvm;

namespace Matrix_UWP.Model {
  public class Notification : BindableBase {

    public Notification(JToken token = null) {
      if (token == null) {
        token = new JObject();
      }
      var data = token as JObject;
      this.msg_id = Helpers.Nullable.ToInt(data["id"]);
      this.is_read = Helpers.Nullable.ToBool(data["status"]);
      this.time = Helpers.Nullable.ToDateTimeOffset(data["time"], DateTimeOffset.MinValue);
      this.type = this.stringToType(Helpers.Nullable.ToString(data["type"]));
      this.content = this.getContent(data["content"]);
      this.sender = this.getSender(data["sender"]);
    }

    public enum Type {
      Course,
      Homework,
      Discussion,
      System
    }

    public int msg_id {
      get;
    }

    private bool? _is_read;
    public bool? is_read {
      get {
        return this._is_read;
      }
      set {
        this.SetProperty(ref this._is_read, value);
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

    private DateTimeOffset _time;
    public DateTimeOffset time {
      get {
        return this._time;
      }
      set {
        this.SetProperty(ref this._time, value);
      }
    }

    private string _sender;
    public string sender {
      get {
        return this._sender;
      }
      set {
        this.SetProperty(ref this._sender, value);
      }
    }

    private string _content;
    public string content {
      get {
        return this._content;
      }
      set {
        this.SetProperty(ref this._content, value);
      }
    }

    public async Task toggleReadState() {
      await MatrixRequest.ChangeMsgState(this.msg_id, !this.is_read);
    }

    private Type stringToType(string str) {
      switch (str) {
        case "course":
          return Type.Course;
        case "homework":
          return Type.Homework;
        case "discussion":
          return Type.Discussion;
        default:
          break;
      }
      return Type.System;
    }

    private string getContent(JToken content) {
      switch (this.type) {
        case Type.Course:
          return Helpers.Nullable.ToString(content["text"]);
        case Type.Homework:
          return $"{Helpers.Nullable.ToString(content["action"])}了作业“{Helpers.Nullable.ToString(content["prob_title"])}”";
        case Type.Discussion:
          string title = Helpers.Nullable.ToString(content["title"]);
          if (content["text"] == null) {
            if (content["rep_id"] == null) {
              return $"评论了你的话题“{title}”";
            } else {
              return $"回复了你在话题“{title}”中的评论";
            }
          } else {
            if (content["rep_id"] == null) {
              return $"在话题“{title}”的评论中提到了你";
            } else {
              return $"在话题“{title}”的评论回复中提到了你";
            }
          }
        default:
          break;
      }
      return Helpers.Nullable.ToString(content);
    }

    private string getSender(JToken sender) {
      switch (this.type) {
        case Type.Course:
          return Helpers.Nullable.ToString(sender["name"]);
        case Type.Homework:
        case Type.Discussion:
          return Helpers.Nullable.ToString(sender["name"]?["nickname"]);
        default:
          break;
      }
      return "Matrix 团队";
    }

  }

}
