using ModernClient.MVVM1.View;
using ModernClient.MVVM1.ViewModel;
using System.Windows;

namespace ModernClient
{

    public partial class MainWindow : Window
    {

        Web_API API = new Web_API();
        MainViewModel mainModel = new MainViewModel();
        public MainWindow()
        {

            InitializeComponent();
            
            ButtonsViewModel viewmodel = new ButtonsViewModel(mainModel);
            Buttons buttons = new Buttons();
            buttons.DataContext = viewmodel;

            mainModel.CurrentView = buttons;
            DataContext = mainModel;
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

        private async void Close_Click(object sender, RoutedEventArgs e)
        {
            await API.LogOut_User_async(mainModel.CurrentUser);
            MainViewModel.timer_users.Stop();
            Application.Current.Shutdown();
        }

        private void AcceptNik_Click(object sender, RoutedEventArgs e)
        {
            //CurrentUser.Name = Username.Name;
        }


    }
}
