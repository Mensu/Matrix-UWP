using System.Threading.Tasks;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Matrix_UWP.Helpers;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.Foundation;
using System.IO;
using Windows.UI.Popups;
using System.Collections.ObjectModel;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Matrix_UWP.Views {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class Login : Page {
    ViewModel.LoginViewModel LoginVM = new ViewModel.LoginViewModel();
    private bool useCaptcha = false;
    private string captchaSvg;
    private ObservableCollection<string> suggestions = new ObservableCollection<string>();
    private Model.SuggestionInput suggetstionInput = Model.SuggestionInput.GetInstance();
    public Login() {
      InitializeComponent();
      NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
      this.DataContext = LoginVM;
    }

    private async void LoginBtn_Click(object sender, RoutedEventArgs e) {
      await Virtual_Login_Click();
    }

    private async Task Virtual_Login_Click() {
      DisableLoginBtn();
      bool success = await TryLogin();
      if (useCaptcha) {
        Captcha.Show(captchaSvg);
        return;
      }
      TryLeave(success);
    }

    private void ResetPasswd_Click(object sender, RoutedEventArgs e) {
      MessageDialog dia = new MessageDialog("前方施工中！\n别急，还没做呢");
      dia.ShowAsync();
    }

    private async Task<bool> TryLogin() {
      Model.User currentUser;
      try {
        currentUser = await Model.MatrixRequest.login(LoginVM.username, LoginVM.password, LoginVM.captcha);
        return true;
      } catch (MatrixException.WrongPassword err) {
        ShowError("Haha，密码输错了.");
      } catch (MatrixException.WrongCaptcha err) {
        if (useCaptcha) {
          ShowError("验证码错了哦，再试一遍吧");
        } else {
          useCaptcha = true;
        }
        captchaSvg = err.captcha;
      } catch (MatrixException.FatalError err) {
        ShowError($"搞出事了吧？\n致命错误：{err.Message}");
      } catch (MatrixException.MatrixException err) {
        ShowError($"非洲人...\n未知错误：{err.Message}");
      }
      return false;
    }
    
    private void ShowError(string msg, UserControls.InfoMessage.MessageLevel level = UserControls.InfoMessage.MessageLevel.Warning) {
      this.Msg.Level = level;
      Msg.Text = msg;
      Msg.Show();
    }

    private void DisableLoginBtn() {
      LoginBtn.Content = "登陆中...";
      LoginBtn.IsEnabled = false;
    }

    private void EnableLoginBtn() {
      LoginBtn.Content = "登陆";
      LoginVM.password = "";
      useCaptcha = false;
      LoginBtn.IsEnabled = true;
    }

    private void TryLeave(bool success) {
      EnableLoginBtn();
      if (!success) return;
        suggetstionInput.addUser(Username.Text);
      Frame.Navigate(typeof(Views.MainPage));
    }

    private async void Captcha_OnSured(object sender, UserControls.CaptchaPopup.CaptchaEventArgs e) {
      LoginVM.captcha = e.captcha;
      bool success = await TryLogin();
      TryLeave(success);
    }

    private void Captcha_OnClosed(object sender, UserControls.CaptchaPopup.CaptchaEventArgs e) {
      EnableLoginBtn();
    }
    BitmapImage defaultAvatar = new BitmapImage(new Uri("ms-appx:///Assets/Login/Avatar.png"));
    private void Username_LostFocus(object sender, RoutedEventArgs e) {
      if (Username.Text == "") return;
      try {
        LoginVM.avatar = new BitmapImage(Model.MatrixRequest.getAvatarUri(Username.Text));
      } catch (Exception err) {
        LoginVM.avatar = defaultAvatar;
      }
    }

    private void CloseQrCode_Click(object sender, RoutedEventArgs e) {
      QrCodePanel.Visibility = Visibility.Collapsed;
    }

    private void ShowQrCode_Click(object sender, RoutedEventArgs e) {
      QrCodePanel.Visibility = Visibility.Visible;
    }

    private async void Username_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
      if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
      this.LoginVM.suggestions.Clear();
      suggestions = suggetstionInput.loadUser(Username.Text);
      sender.ItemsSource = suggestions;
    }

    private void Username_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {
      sender.Text = args.SelectedItem?.ToString();
    }

    private async void Password_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
      if (e.Key == Windows.System.VirtualKey.Enter) {
        await Virtual_Login_Click();
      }
    }
  }
}
