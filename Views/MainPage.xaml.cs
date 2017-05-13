using Matrix_UWP.Helpers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Matrix_UWP.Views {
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page {
    public MainPage() {
      this.InitializeComponent();
      ContentPanel = new Dictionary<string, UserControl> {
        { "课程", CourseList },
        { "题库", LibraryView },
        { "通知",  NotificationView },
        { "设置", SettingView },
        { "关于", AboutView },
      };

      Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(500, 620));
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e) {
      bool isLogin = false;
      try {
        isLogin = await Model.MatrixRequest.isLogin();
      } catch (Exception err) {
        System.Diagnostics.Debug.WriteLine(err);
      }
      if (!isLogin) {
        Frame.Navigate(typeof(Views.Login));
      }
    }
    private async void HamburgerMenu_OnItemClick(object sender, ItemClickEventArgs e) {
      var menuItem = e.ClickedItem as HamburgerMenuItem;
      await ShowContent(menuItem.Label);
    }

    private Dictionary<string, UserControl> ContentPanel;
    private async Task ShowContent(string label) {
      foreach (KeyValuePair<string, UserControl> pair in ContentPanel) {
        pair.Value.Visibility = Visibility.Collapsed;
      }
      if (ContentPanel.ContainsKey(label)) {
        UserControl ctrl = ContentPanel[label];
        IHamburgerContent icontent = null;
        try {
          icontent = (IHamburgerContent)(ctrl);
        } finally {
          if (icontent != null) {
            icontent.onError += this.ShowError;
            await icontent?.ResetContentAsync();
          }
          ctrl.Visibility = Visibility;
        }
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
  }
}
