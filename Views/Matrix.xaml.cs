using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Matrix_UWP.Views {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class Matrix : Page {
    public Matrix() {
      this.InitializeComponent();
      ContentFrame.Navigated += ContentPageHandlerInject;
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      await FetchUser();
      NavigateContent("home");
    }

    private bool DialogLock = false;
 
    private bool DialogLockAcquire() {
      lock (this) {
        if (!DialogLock) {
          DialogLock = true;
          return true;
        }
      }
      return false;
    }

    private void DialogLockRelease() {
      DialogLock = false;
    }

    #region ContentPageInject

    private HashSet<Helpers.INavigationViewContent> NavContents = new HashSet<Helpers.INavigationViewContent>();

    // 为导航到的新页面实例注册事件
    private void ContentPageHandlerInject(object sender, NavigationEventArgs e) {
      if (!(e.Content is Helpers.INavigationViewContent content)) {
        Debug.WriteLine($"注入页面失败, 非法的页面: {e.SourcePageType.FullName}");
        return;
      }
      if (!NavContents.Contains(content)) {
        NavContents.Add(content);
        content.OnContentError += Content_OnContentError;
        content.OnContentLoaded += Content_OnContentLoaded;
        content.OnContentLoading += Content_OnContentLoading;
        content.TitleChanged += Content_TitleChanged;
      }
    }

    private void Content_TitleChanged(object sender, Helpers.NavigationViewContentEvent e) {
      viewModel.Title = e.Message;
    }

    private void Content_OnContentLoading(object sender, Helpers.NavigationViewContentEvent e) {
      ShowLoading();
    }

    private void Content_OnContentLoaded(object sender, Helpers.NavigationViewContentEvent e) {
      StopLoading();
    }

    private async void Content_OnContentError(object sender, Helpers.NavigationViewContentEvent e) {
      if (e.Exception is MatrixException.NotLogin) {
        await DoLogin();
      } else {
        await ErrorPrompt(e.Message);
      }
    }

    #endregion

    ViewModel.MatrixViewModel viewModel = new ViewModel.MatrixViewModel();

    #region NavigationHandle
    // 储存对应页面的类型
    private readonly Dictionary<String, Type> ContentMap = new Dictionary<string, Type> {
      ["home"] = typeof(Contents.HomePage),
      ["course"] = typeof(Contents.CoursesPage),
      ["library"] = typeof(Contents.LibrariesPage),
      //["exam"] = typeof(Contents.ExamsPage),
      ["profile"] = typeof(Contents.ProfilePage),
      ["setting"] = typeof(Contents.ProfilePage),
      ["notification"] = typeof(Contents.Notification),
    };

    private string previousTag = null;

    private Dictionary<String, String> NavigateHistory = new Dictionary<string, string>();

    private void ClearHistory() {
      NavigateHistory.Clear();
      previousTag = null;
    }

    private void NavigateContent(string tag) {
      if (!ContentMap.ContainsKey(tag)) {
        Debug.WriteLine($"未知的内容: {tag}");
        return;
      }
      // has history
      if (previousTag != null) {
        // save previous content history
        NavigateHistory[previousTag] = ContentFrame.GetNavigationState();
      }
      if (NavigateHistory.ContainsKey(tag)) {
        // restore history
        ContentFrame.SetNavigationState(NavigateHistory[tag]);
        if (ContentFrame.Content is Helpers.INavigationViewContent content) {
          Title.Text = content.GetTitle();
        }
      } else {
        // navigate to new content
        ContentFrame.Navigate(ContentMap[tag]);
      }
      previousTag = tag;
    }

    #endregion

    private void ShowLoading() {
      LoadingRing.Visibility = Visibility.Visible;
    }

    private void StopLoading() {
      LoadingRing.Visibility = Visibility.Collapsed;
    }

    private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) {
      if (ContentFrame.CanGoBack) {
        ContentFrame.GoBack();
      }
    }

    private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args) {
      Indicator.Visibility = Visibility.Collapsed;
      if (args.IsSettingsInvoked) {
        NavigateContent("setting");
      } else {
        var selected = sender.MenuItems.OfType<NavigationViewItem>().First(item => item.Content.ToString() == args.InvokedItem.ToString());
        NavigateContent(selected.Tag.ToString());
      }
    }

    private void Profile_Tapped(object sender, TappedRoutedEventArgs e) {
      NavView.SelectedItem = null;
      Indicator.Visibility = Visibility.Visible;
      NavigateContent("profile");
    }

    private async void Refresh_Click(object sender, RoutedEventArgs e) {
      await RefreshContent();
    }

    private async Task RefreshContent() {
      if (ContentFrame.Content is Helpers.INavigationViewContent content) {
        await content.Refresh();
      }
      try {
        viewModel.User = await Model.MatrixRequest.GetProfile();
      } catch (MatrixException.MatrixException err) {
        await ErrorPrompt($"获取用户信息失败: {err.Message}");
      }
    }

    private async void Logout_Click(object sender, RoutedEventArgs e) {
      try {
        await Model.MatrixRequest.logout();
      } catch (MatrixException.NotLogin) {
      } catch (MatrixException.MatrixException err) {
        await ErrorPrompt($"退出登录失败: {err.Message}");
      }
      await DoLogin(true);
    }

    #region LoginHandle
    private async Task<bool> FetchUser() {
      bool userChanged = true;
      int previousUserId = viewModel.User.user_id;
      try {
        viewModel.User = await Model.MatrixRequest.GetProfile();
        userChanged = (viewModel.User.user_id != previousUserId);
      } catch (MatrixException.MatrixException err) {
        await ErrorPrompt($"请求用户信息失败: {err.Message}");
      }
      return userChanged;
    }

    private async Task DoLogin(bool force = false) {
      bool isLogin = false;

      if (force == false) {
        try {
          isLogin = await Model.MatrixRequest.IsLogin();
        } catch (MatrixException.NotLogin) {
          isLogin = false;
        } catch (MatrixException.MatrixException err) {
          Debug.WriteLine($"请求登陆状态失败: {err.Message}");
        }
      }

      bool cannotSkip = force || !isLogin;

      bool success = true;

      if (cannotSkip) {
        success = await LoginPrompt();
      }

      if (!success) {
        ClearHistory();
        NavigateContent("setting");
      } else if (await FetchUser()) {
        // 新用户
        ClearHistory();
        NavigateContent("home");
      }
      await RefreshContent();
    }

    private Dialogs.LoginDialog LoginDialog = new Dialogs.LoginDialog();

    private async Task<bool> LoginPrompt() {
      if (!DialogLockAcquire()) {
        return false;
      }
      await LoginDialog.ShowAsync();
      DialogLockRelease();
      return LoginDialog.Result == Dialogs.LoginDialog.LoginResult.Success;
    }

    #endregion

    #region ErrorHandle
    // 错误处理对话框
    private ContentDialog dialog = new ContentDialog {
      IsPrimaryButtonEnabled = true,
      IsSecondaryButtonEnabled = true,
      PrimaryButtonText = "确定",
      SecondaryButtonText = "退出应用",
      Title = "错误",
    };

    private async Task ErrorPrompt(string message) {
      dialog.Content = message;
      if (!DialogLockAcquire()) {
        return;
      }
      var result = await dialog.ShowAsync();
      DialogLockRelease();
      if (result == ContentDialogResult.Secondary) {
        Application.Current.Exit();
      }
    }
    #endregion

    private void Notification_Click(object sender, RoutedEventArgs e) {
      NavigateContent("notification");
    }
  }
}
