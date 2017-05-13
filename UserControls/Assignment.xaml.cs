using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Matrix_UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class Assignment : UserControl, Helpers.IHamburgerContent {
    internal ViewModel.AssignmentViewModel vm = new ViewModel.AssignmentViewModel();
    public Assignment() {
      this.InitializeComponent();
      this.listColView.DataContext = vm;
      this.detailColView.DataContext = vm;
    }

    public event HamburgerContentHandler onError;

    public async Task ResetContentAsync() {
      var courseList = await Model.MatrixRequest.getCourseList();
      await refreshList(59);
    }

    public async Task refreshList(int course_id) {
      var newList = this.vm.progressingList;
      bool success = false;
      this.vm.listIsLoading = true;
      try {
        newList = await Model.MatrixRequest.getAssignmentList(course_id);
        success = true;
      } catch (MatrixException.SoftError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } catch (MatrixException.FatalError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } finally {
        this.vm.listIsLoading = false;
      }
      if (!success) {
        return;
      }
      this.vm.updateWith(newList);
    }

    private async void listView_OnItemClicked(object sender, AssignmentList.AssignmentItemClickEventArgs e) {
      this.vm.detailIsLoading = true;
      try {
        this.vm.curAssignment = await Model.MatrixRequest.getAssignment(e.course_id, e.ca_id);
      } catch (MatrixException.SoftError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } catch (MatrixException.FatalError err) {
        onError?.Invoke(this, new HamburgerContentEventArgs(err.Message));
        return;
      } finally {
        this.vm.detailIsLoading = false;
      }
    }
  }
}
