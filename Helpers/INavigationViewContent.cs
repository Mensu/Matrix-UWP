using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Helpers {
  public interface INavigationViewContent {
    Task Refresh();
    void EnablePageCache();
    event NavigationViewContentHandler OnContentError;
    event NavigationViewContentHandler OnContentLoading;
    event NavigationViewContentHandler OnContentLoaded;
    event NavigationViewContentHandler TitleChanged;
  }

  public delegate void NavigationViewContentHandler(object sender, NavigationViewContentEvent e);

  public class NavigationViewContentEvent: EventArgs {
    public MatrixException.MatrixException Exception { get; set; }
    public string Message { get; set; }

    public NavigationViewContentEvent(MatrixException.MatrixException exception = null, string message = "") {
      Exception = exception;
      Message = message;
    }

    public NavigationViewContentEvent(string message) {
      Exception = null;
      Message = message;
    }
  }
}
