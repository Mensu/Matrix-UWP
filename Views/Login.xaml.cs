using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Matrix_UWP.Helpers;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Matrix_UWP.Views {
  /// <summary>
  /// 可用于自身或导航至 Frame 内部的空白页。
  /// </summary>
  public sealed partial class Login : Page {
    ViewModel.LoginViewModel LoginVM = new ViewModel.LoginViewModel();
    private bool useCaptcha = false;
    private string captchaSvg;
    public Login() {
      InitializeComponent();
      this.DataContext = LoginVM;
    }

    private async void LoginBtn_Click(object sender, RoutedEventArgs e) {
      DisableLoginBtn();
      bool success = await TryLogin();
      if (useCaptcha) {
        ToggleMask();
        Captcha.Show(captchaSvg);
        return;
      }
      EnableLoginBtn();
    }

    private void ResetPasswd_Click(object sender, RoutedEventArgs e) {
      ContentDialog dia = new ContentDialog();
      dia.Content = "前方施工中！\n别急，还没做呢";
      dia.ShowAsync();
    }

    private async Task<bool> TryLogin() {
      Model.User currentUser;
      try {
        currentUser = await Model.MatrixRequest.login(LoginVM.username, LoginVM.password, LoginVM.captcha);
        return true;
      } catch (MatrixException.WrongCaptcha err) {
        useCaptcha = true;
        ShowError("验证码错了哦，再试一遍吧");
        captchaSvg = err.captcha.svgText;
      } catch (MatrixException.WrongPassword err) {
        ShowError("Haha，密码输错了.");
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

    private void ToggleMask() {
      if (CaptchaMask.Visibility == Visibility.Collapsed) {
        CaptchaMask.Visibility = Visibility.Visible;
      } else {
        CaptchaMask.Visibility = Visibility.Collapsed;
      }
    }

    private void Navigate(bool success) {
      if (!success) return;
      Frame.Navigate(typeof(MainPage));
    }

    private async void Captcha_OnSured(object sender, UserControls.CaptchaPopup.CaptchaEventArgs e) {
      ToggleMask();
      bool success = await TryLogin();
      EnableLoginBtn();
    }

    private void Captcha_OnClosed(object sender, UserControls.CaptchaPopup.CaptchaEventArgs e) {
      ToggleMask();
      EnableLoginBtn();
    }

    private async void Username_TextChanged(object sender, TextChangedEventArgs e) {
      // try get user avatar here
      await Task.Delay(100);
    }
  }
}
