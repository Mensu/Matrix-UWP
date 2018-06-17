using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Helpers {
  public interface INavigationViewContent {
    Task Refresh();
    event NavigationViewContentHandler OnContentError;
  }

  public delegate void NavigationViewContentHandler(object sender, NavigationViewContentEvent e);

  public class NavigationViewContentEvent: EventArgs {
    public MatrixException.MatrixException Exception { get; set; }
    public string Message { get; set; }

    public NavigationViewContentEvent(MatrixException.MatrixException exception, string message = "") {
      Exception = exception;
      Message = message;
    }
  }
}
