using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class InfoMessage : UserControl {
    public InfoMessage() {
      this.InitializeComponent();
      this.Container.Visibility = Visibility.Collapsed;
    }
    private int timeout = -1;
    public string Timeout {
      get { return timeout.ToString(); }
      set { timeout = Convert.ToInt32((value ?? "0") == "" ? "0" : value); }
    }
    
    public string Text {
      get { return Message.Text; }
      set { Message.Text = value; }
    }
    private MessageLevel level = MessageLevel.Info;
    public MessageLevel Level {
      get { return level; }
      set { level = value; setColor(); }
    }
    private uint callId = 0;

    public enum MessageLevel {
      Info,
      Warning,
      Error,
      Fatal
    }

    private void Close_Click(object sender, RoutedEventArgs e) {
      this.Container.Visibility = Visibility.Collapsed;
    }
    private void setColor() {
      Color color;
      switch(level) {
        case MessageLevel.Info:
          color = Color.FromArgb(255, 153, 153, 153);
          break;
        case MessageLevel.Warning:
          color = Color.FromArgb(255, 255, 143, 53);
          break;
        case MessageLevel.Error:
          color = Color.FromArgb(255, 255, 51, 0);
          break;
        case MessageLevel.Fatal:
          color = Color.FromArgb(255, 204, 0, 255);
          break;
      }
      Container.Background = new SolidColorBrush(color);
    }
    public void Show() {
      callId++;
      if (timeout >= 0) {
        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => {
          uint curCallId = callId;
          await Task.Delay(timeout);
          if (curCallId == callId)
            this.Container.Visibility = Visibility.Collapsed;
        }).AsTask();
      }
      this.Container.Visibility = Visibility.Visible;
    }
  }
}
