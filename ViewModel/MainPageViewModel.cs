using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Prism.Mvvm;
using Windows.UI.Xaml.Controls;

namespace Matrix_UWP.ViewModel {
  public class MainPageViewModel : BindableBase {
    internal HamburgerMenuItemCollection menu = new HamburgerMenuItemCollection() {
      new HamburgerMenuGlyphItem() { Glyph = "Home", Label = "主页" }
    };
    private Model.User _curUser = new Model.User();
    public Model.User curUser {
      get { return this._curUser; }
      set { this.SetProperty(ref this._curUser, value); }
    }
    public Dictionary<string, UserControl> userCtrls {
      get;
    }
    public MainPageViewModel() {
      this.userCtrls = new Dictionary<string, UserControl>();
    }
    public void addMenuItem(string label, string glyph, UserControl uc) {
      this.userCtrls.Add(label, uc);
      this.menu.Add(new HamburgerMenuGlyphItem() { Glyph = glyph, Label = label });
    }
  }
}
