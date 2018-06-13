using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Matrix_UWP.Model;
using System;
using System.Diagnostics;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class Launcher : Page {
    public Launcher() {
      this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
      this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      bool isLogin = await IsLogin();
      if (isLogin) {
        Frame.Navigate(typeof(MainPage));
      } else {
        Frame.Navigate(typeof(Login));
      }
    }

    private async Task<bool> IsLogin() {
      try {
        return await MatrixRequest.IsLogin();
      } catch (Exception e) {
        Debug.WriteLine($"Login Failed {e.Message}");
        return false;
      }
    }
  }
}
