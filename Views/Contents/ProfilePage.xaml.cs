using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Newtonsoft.Json.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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

    // ViewModel
    private ViewModel.SettingViewModel viewModel = new ViewModel.SettingViewModel();

    // Interface Error Event
    public event NavigationViewContentHandler OnContentError;

    // Interface Refresh
    public async Task Refresh() {
      try {
        viewModel.curUser = await Model.MatrixRequest.GetProfile();
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"获取用户信息失败, {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await Refresh();
    }

    private static readonly Dictionary<string, string> ProfileNames = new Dictionary<string, string> {
      ["nickname"] = "昵称",
      ["email"] = "电子邮件",
      ["phone"] = "电话",
      ["homepage"] = "主页",
      ["password"] = "密码",
      ["passwordOld"] = "原先密码",
    };

    private async void Submit_Click(object sender, RoutedEventArgs e) {
      var userProfile = new {
        nickname = NicknameInput.Text,
        email = EmailInput.Text,
        phone = PhoneInput.Text,
        homepage = HomepageInput.Text,
      };
      string invalidKey = CheckInput(JObject.FromObject(userProfile));
      if (invalidKey != null) {
        await ShowMessage($"{ProfileNames[invalidKey]}的值不能为空");
        return;
      }
      try {
        await Model.MatrixRequest.ChangeProfile(
          userProfile.nickname,
          userProfile.email,
          userProfile.phone,
          userProfile.homepage
        );
      } catch (MatrixException.MatrixException err) {
        Debug.WriteLine($"更改用户信息失败, {err.Message}");
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err));
      }
    }

    private string CheckInput(JObject userProfile) {
      foreach (var kp in userProfile) {
        if (String.IsNullOrEmpty(kp.Value.ToString())) {
          return kp.Key;
        }
      }
      return null;
    }

    private async Task ShowMessage(string message) {
      var dialog = new MessageDialog(message);
      await dialog.ShowAsync();
    }
  }
}
