using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class CaptchaPopup : UserControl {
    public class CaptchaEventArgs : EventArgs {
      public CaptchaEventArgs() { }
      public CaptchaEventArgs(string captcha) {
        this.captcha = captcha;
      }
      public string captcha = "";
    }

    private string text = "";
    public string Text {
      get { return text; }
      set { text = value; LoadCaptcha(); }
    }

    public delegate void CaptchaEventHandler(object sender, CaptchaEventArgs e);
    public event CaptchaEventHandler OnSured;
    public event CaptchaEventHandler OnClosed;

    public CaptchaPopup() {
      this.InitializeComponent();
    }

    public void Show(string captcha="") {
      if (captcha != "") Text = captcha;
      ParentPopup.IsOpen = true;
    }

    private void LoginBtn_Click(object sender, RoutedEventArgs e) {
      string captcha = this.CaptchaInput.Text;
      ParentPopup.IsOpen = false;
      OnSured?.Invoke(this, new CaptchaEventArgs(captcha));
    }


    private void AppBarButton_Click(object sender, RoutedEventArgs e) {
      ParentPopup.IsOpen = false;
      OnClosed?.Invoke(this, new CaptchaEventArgs());
    }

    private void ParentPopup_LayoutUpdated(object sender, object e) {
      if (Panel.ActualWidth == 0 && Panel.ActualHeight == 0) {
        return;
      }

      double ActualHorizontalOffset = this.ParentPopup.HorizontalOffset;
      double ActualVerticalOffset = this.ParentPopup.VerticalOffset;

      double NewHorizontalOffset = (Window.Current.Bounds.Width - Panel.ActualWidth) / 2;
      double NewVerticalOffset = (Window.Current.Bounds.Height - Panel.ActualHeight) / 2;

      if (ActualHorizontalOffset != NewHorizontalOffset || ActualVerticalOffset != NewVerticalOffset) {
        this.ParentPopup.HorizontalOffset = NewHorizontalOffset;
        this.ParentPopup.VerticalOffset = NewVerticalOffset;
      }
    }

    private async void LoadCaptcha() {
      CaptchaImage.LoadText(text);
      await Task.Delay(200);
      CaptchaImage.LoadText(text);
    }
  }
}
