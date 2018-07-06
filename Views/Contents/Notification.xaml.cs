using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
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
  public sealed partial class Notification : Page, Helpers.INavigationViewContent {
    public Notification() {
      this.InitializeComponent();
    }

    private ViewModel.NotificationViewModel viewModel = new ViewModel.NotificationViewModel();

    public event NavigationViewContentHandler OnContentError;
    public event NavigationViewContentHandler OnContentLoading;
    public event NavigationViewContentHandler OnContentLoaded;
    public event NavigationViewContentHandler TitleChanged;

    public string GetTitle() {
      return "Notification";
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e) {
      base.OnNavigatedTo(e);
      TitleChanged?.Invoke(this, new NavigationViewContentEvent(GetTitle()));
      await Refresh();
    }

    public async Task Refresh() {
      OnContentLoading?.Invoke(this, new NavigationViewContentEvent());
      try {
        viewModel.list = await Model.MatrixRequest.GetNotificationList();
      } catch (MatrixException.MatrixException err) {
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err, $"获取通知列表失败：{err.Message}"));
      }
      OnContentLoaded?.Invoke(this, new NavigationViewContentEvent());
    }

    private async Task EnqueueNotification(Model.Notification notification) {
      var tile = await XmlDocument.LoadFromUriAsync(new Uri("ms-appx:///assets/tile.xml"));
      foreach (var subgroup in tile.GetElementsByTagName("subgroup")) {
        subgroup.ChildNodes[0].InnerText = notification.sender;
        subgroup.ChildNodes[1].InnerText = Helpers.ISOStringConverter.toReadableString(notification.time);
        subgroup.ChildNodes[2].InnerText = notification.content;
      }
      var tileNotification = new TileNotification(tile);
      var updater = TileUpdateManager.CreateTileUpdaterForApplication();
      updater.EnableNotificationQueue(true);
      updater.Update(tileNotification);
    }
    private async void Checkbox_Checked(object sender, RoutedEventArgs e) {
      FrameworkElement elem = sender as FrameworkElement;
      if (!(elem.DataContext is Model.Notification notification)) return;
      try {
        await notification.toggleReadState();
      } catch (MatrixException.MatrixException err) {
        OnContentError?.Invoke(this, new NavigationViewContentEvent(err, $"标记列表状态失败：{err.Message}"));
      }
    }
  }

}
