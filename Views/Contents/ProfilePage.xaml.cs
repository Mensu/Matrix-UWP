using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views.Contents {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class ProfilePage : Page, Helpers.INavigationViewContent {
    public ProfilePage() {
      this.InitializeComponent();
    }

    private ViewModel.SettingViewModel viewModel = new ViewModel.SettingViewModel();

    public event NavigationViewContentHandler OnContentError;

    public async Task Refresh() {
      try {
        viewModel.curUser = await Model.MatrixRequest.GetProfile();
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"获取用户信息失败, {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
    }

    private async void Submit_Click(object sender, RoutedEventArgs e) {
      var nickname = viewModel.curUser.nickname;
      var email = viewModel.curUser.email;
      var phone = viewModel.curUser.phone;
      var homepage = viewModel.curUser.homepage;
      try {
        await Model.MatrixRequest.ChangeProfile(nickname, email, phone, homepage);
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"更改用户信息失败, {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
    }
  }
}
