using Matrix_UWP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class CourseList : UserControl, IHamburgerContent {
    internal ViewModel.CourseListViewModel vm = new ViewModel.CourseListViewModel();

    public CourseList() {
      this.InitializeComponent();
    }

    public event HamburgerContentHandler onError;

    public async Task ResetContentAsync() {
      await Refresh();
    }

    public async Task Refresh() {
      CourstListPivot.Visibility = Visibility.Visible;
      AssignmentPane.Visibility = Visibility.Collapsed;
      try {
        vm.Update(await Model.MatrixRequest.GetCourseList());
      } catch (MatrixException.NotLogin err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
      } catch (MatrixException.MatrixException err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
      }
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e) {
      await Refresh();
    }

    private async void Button_Click(object sender, RoutedEventArgs e) {
      Button btn = sender as Button;
      Task resetAssignment = AssignmentPane.refreshList(Convert.ToInt32(btn.Name));
      CourstListPivot.Visibility = Visibility.Collapsed;
      AssignmentPane.Visibility = Visibility.Visible;
      await resetAssignment;
    }
  }
}
