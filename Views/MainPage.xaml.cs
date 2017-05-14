using Matrix_UWP.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Matrix_UWP.Views {
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page {
    ViewModel.MainPageViewModel vm = new ViewModel.MainPageViewModel();
    public MainPage() {
      this.InitializeComponent();
      this.vm.addMenuItem("主页", "Home", HomeView);
      this.vm.addMenuItem("课程", "Edit", CourseList);
      this.vm.addMenuItem("题库", "Library", LibraryView);
      this.vm.addMenuItem("通知", "Read", NotificationView);
      this.vm.addMenuItem("设置", "Setting", SettingView);
      this.vm.addMenuItem("关于", "Tag", AboutView);
      Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 620));
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e) {
      bool isLogin = false;
      try {
        isLogin = await Model.MatrixRequest.isLogin();
      } catch (MatrixException.FatalError err) {
        this.ShowError(this, new HamburgerContentEventArgs(err.Message));
      }
      if (!isLogin) {
        Frame.Navigate(typeof(Views.Login));
      }

      try {
        this.vm.curUser = await Model.MatrixRequest.getProfile();
      } catch (MatrixException.FatalError err) {
        this.ShowError(this, new HamburgerContentEventArgs(err.Message));
      }
    }
    private async void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e) {
      var menuItem = e.ClickedItem as HamburgerMenuItem;
      await ShowContent(menuItem.Label);
    }

    private async Task ShowContent(string label) {
      foreach (KeyValuePair<string, UserControl> pair in vm.userCtrls) {
        pair.Value.Visibility = Visibility.Collapsed;
      }
      if (!vm.userCtrls.ContainsKey(label)) return;
      UserControl ctrl = vm.userCtrls[label];
      IHamburgerContent icontent = null;
      try {
        icontent = (IHamburgerContent)ctrl;
      } finally {
        if (icontent != null) {
          icontent.onError += this.ShowError;
          await icontent?.ResetContentAsync();
        }
        ctrl.Visibility = Visibility;
      }
    }

    private void ShowError(object sender, HamburgerContentEventArgs e) {
      Debug.WriteLine("does not work: " + e.message);
    }

    private void FakeBtn_Click(object sender, RoutedEventArgs e) {
      Navigate.IsPaneOpen = !Navigate.IsPaneOpen;
      if (Navigate.IsPaneOpen) {
        Navigate.Focus(FocusState.Programmatic);
        Debug.WriteLine("Get focus to Navigate");
      }
      Debug.WriteLine($"Navigate.IsPaneOpen = {Navigate.IsPaneOpen}");
    }

    private async void Logout_Click(object sender, RoutedEventArgs e) {
      try {
        await Model.MatrixRequest.logout();
      } catch (Exception err) {
        Debug.WriteLine(err.Message);
      } finally {
        Frame.Navigate(typeof(Views.Login));
      }
    }

    private async void Setting_Click(object sender, RoutedEventArgs e) {
      Navigate.SelectedIndex = vm.menu.FindIndex(one => one.Label == "设置");
      await this.ShowContent("设置");
    }

    private async void Home_Click(object sender, RoutedEventArgs e) {
      Navigate.SelectedIndex = vm.menu.FindIndex(one => one.Label == "主页");
      await this.ShowContent("主页");
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      Navigate.SelectedIndex = vm.menu.FindIndex(one => one.Label == "主页");
      await this.ShowContent("主页");
    }
  }
}
