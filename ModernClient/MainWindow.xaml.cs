using ModernClient.MVVM1.View;
using ModernClient.MVVM1.ViewModel;
using System.Windows;

namespace ModernClient
{

    public partial class MainWindow : Window
    {


        public MainWindow()
        {

            InitializeComponent();
            MainViewModel mainModel = new MainViewModel();
            LoginViewModel viewmodel = new LoginViewModel(mainModel);
            LoginView login = new LoginView();
            login.DataContext = viewmodel;

            mainModel.CurrentView = login;
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

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void AcceptNik_Click(object sender, RoutedEventArgs e)
        {
            //CurrentUser.Name = Username.Name;
        }


    }
}
