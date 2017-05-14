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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Matrix_UWP.UserControls {
  public sealed partial class AssignmentList : UserControl {
    public AssignmentList() {
      this.InitializeComponent();
    }

    public class AssignmentItemClickEventArgs : EventArgs {
      public AssignmentItemClickEventArgs() { }
      public AssignmentItemClickEventArgs(int course_id, int ca_id) {
        this.course_id = course_id;
        this.ca_id = ca_id;
      }
      public int course_id = 0;
      public int ca_id = 0;
    }

    public delegate void AssignmentItemClickEventHandler(object sender, AssignmentItemClickEventArgs e);
    public event AssignmentItemClickEventHandler OnItemClicked;

    private void listView_ItemClick(object sender, ItemClickEventArgs e) {
      var assignment = e.ClickedItem as Model.Assignment;
      OnItemClicked?.Invoke(sender, new AssignmentItemClickEventArgs(assignment.course_id, assignment.ca_id));
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      this.progressingListView.SelectedIndex = -1;
      this.endedListView.SelectedIndex = -1;
    }
  }
}
