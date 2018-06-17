using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix_UWP.Helpers {
  interface INavigationViewContent {
    Task Refresh();
    event NavigationViewContentHandler OnError;
  }

  delegate void NavigationViewContentHandler(object sender, NavigationViewContentEvent e);

  class NavigationViewContentEvent: EventArgs {
    public MatrixException.MatrixException Exception { get; set; }
    public string Message { get; set; }

    public NavigationViewContentEvent(MatrixException.MatrixException exception, string message = "") {
      Exception = exception;
      Message = message;
    }
  }
}
