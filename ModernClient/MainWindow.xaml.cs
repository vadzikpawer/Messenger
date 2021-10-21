using ModernClient.MVVM1.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace ModernClient
{

    public partial class MainWindow : Window
    {

        public MainWindow()
        {

            InitializeComponent();

        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                DragMove();
            }

        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void Full_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AcceptNik_Click(object sender, RoutedEventArgs e)
        {
            //CurrentUser.Name = Username.Name;
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
