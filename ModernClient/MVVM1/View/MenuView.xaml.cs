using ModernClient.MVVM1.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernClient.MVVM1.View
{

    public partial class MenuView : UserControl
    {
        public MenuView()
        {

            InitializeComponent();

        }

        private void Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoxMessage.Visibility = Visibility.Visible;
            temp_logo.Visibility = Visibility.Collapsed;
            Chat.SelectedIndex = Chat.Items.Count - 1;
            Chat.ScrollIntoView(Chat.SelectedItem);
        }

        private void Home_button_Click(object sender, RoutedEventArgs e)
        {
            Users.UnselectAll();
            MainViewModel.timer.Stop();
            BoxMessage.Visibility = Visibility.Collapsed;
            temp_logo.Visibility = Visibility.Visible;
        }
    }
}
