using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace Matrix_UWP.Views.Dialogs {
  public sealed partial class LoginDialog : ContentDialog {
    public LoginDialog() {
      this.InitializeComponent();
    }

    public enum LoginResult {
      Success,
      Cancel,
    }

    public LoginResult Result = LoginResult.Cancel;

    private Model.SuggestionInput SuggestionServ = Model.SuggestionInput.GetInstance();

    private ViewModel.LoginViewModel viewModel = new ViewModel.LoginViewModel();

    private BitmapImage defaultAvatar = new BitmapImage(new Uri("ms-appx:///Assets/Login/Avatar.png"));

    private async Task<bool> DoLogin() {
      bool success = false;
      string captcha = "";
      if (viewModel.NeedCaptcha == true) {
        captcha = viewModel.captcha;
      }
      try {
        await Model.MatrixRequest.Login(viewModel.username, viewModel.password, captcha);
        success = true;
      } catch (MatrixException.WrongPassword) {
        ShowMessage("密码错误");
      } catch (MatrixException.WrongCaptcha) {
        viewModel.NeedCaptcha = true;
      } catch (MatrixException.MatrixException err) {
        ShowMessage("未知的错误");
      }
      if (success) {
        viewModel.NeedCaptcha = false;
      }
      return success;
    }

    private void ShowMessage(string message) {
      ErrorMessage.Text = message;
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
      bool success = await DoLogin();
      if (success) {
        Result = LoginResult.Success;
      } else {
        args.Cancel = false;
      }
    }

    private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) {
      Result = LoginResult.Cancel;
    }

    private void Username_LostFocus(object sender, RoutedEventArgs e) {
      if (viewModel.username == "") return;
      try {
        viewModel.avatar = new BitmapImage(Model.MatrixRequest.GetAvatarUri(viewModel.username));
      } catch (Exception) {
        viewModel.avatar = defaultAvatar;
      }
    }

    private void Username_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) {
      if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
      viewModel.suggestions = SuggestionServ.loadUser(Username.Text);
    }

    private void Username_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args) {

    }

    private async void Password_KeyUp(object sender, KeyRoutedEventArgs e) {
      if (e.Key == Windows.System.VirtualKey.Enter) {
        bool success = await DoLogin();
        if (success) {
          Result = LoginResult.Success;
          this.Hide();
        }
      }
    }

    private void ResetPasswd_Click(object sender, RoutedEventArgs e) {
    }

    private async Task LoadCaptcha() {
      string captcha = "";
      try {
        captcha = await Model.MatrixRequest.GetCaptcha();
      } catch (MatrixException.NetworkError) {
        ShowMessage("网络错误，无法获取验证码");
      }
      // cast string to IRandomAccessStream
      using (var stream = new MemoryStream()) {
        using (var writer = new StreamWriter(stream)) {
          writer.Write(captcha);
          writer.Flush();
          stream.Position = 0;
          var svg = new SvgImageSource();
          await svg.SetSourceAsync(stream.AsRandomAccessStream());
          viewModel.CaptchaSource = svg;
        }
      }
    }
  }
}
