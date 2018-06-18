using System;
using System.Collections.Generic;
using System.Diagnostics;
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
      }
    }

    private void Content_OnContentLoading(object sender, Helpers.NavigationViewContentEvent e) {
      throw new NotImplementedException();
    }

    private void Content_OnContentLoaded(object sender, Helpers.NavigationViewContentEvent e) {
      throw new NotImplementedException();
    }

    private void Content_OnContentError(object sender, Helpers.NavigationViewContentEvent e) {
      throw new NotImplementedException();
    }

    #endregion

    ViewModel.MatrixViewModel viewModel = new ViewModel.MatrixViewModel();

    #region NavigationHandle
    // 储存对应页面的类型
    private readonly Dictionary<String, Type> ContentMap = new Dictionary<string, Type> {
      //["home"] = typeof(Contents.Home),
      ["course"] = typeof(Contents.CoursesPage),
      ["library"] = typeof(Contents.LibrariesPage),
      //["exam"] = typeof(Contents.ExamsPage),
      ["profile"] = typeof(Contents.ProfilePage),
      //["setting"] = typeof(Contents.SettingPage),
    };

    private string previousTag = null;

    private Dictionary<String, String> NavigateHistory = new Dictionary<string, string>();

    private void NavigateContent(string tag) {
      if (!ContentMap.ContainsKey(tag)) {
        Debug.WriteLine($"未知的内容: {tag}");
        return;
      }
      if (NavigateHistory.ContainsKey(tag)) {
        // has history
        if (previousTag != null) {
          // save previous content history
          NavigateHistory[previousTag] = ContentFrame.GetNavigationState();
        }
        // restore history
        ContentFrame.SetNavigationState(NavigateHistory[tag]);
      } else {
        // navigate to new content
        ContentFrame.Navigate(ContentMap[tag]);
      }
      previousTag = tag;
    }

    #endregion

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
      if (ContentFrame.Content is Helpers.INavigationViewContent content) {
        await content.Refresh();
      }
    }

    private async void Logout_Click(object sender, RoutedEventArgs e) {

    }

    private async Task FetchUser() {
      try {
        viewModel.User = await Model.MatrixRequest.GetProfile();
      } catch (MatrixException.MatrixException err) {
        // handel error
      }
    }

    private async Task<bool> LoginPrompt() {
      return true;
    }
  }
}
