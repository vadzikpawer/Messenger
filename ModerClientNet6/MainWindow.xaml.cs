using Microsoft.AspNetCore.SignalR.Client;
using ModernClientNet6.MVVM.Model;
using ModernClientNet6.MVVM.View;
using ModernClientNet6.MVVM.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Windows;


namespace ModernClientNet6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Web_API API = new Web_API();
        MainViewModel mainModel = new MainViewModel();
        public static Settings StartupSettings = new Settings();
        public MainWindow()
        {
            MainViewModel.connection.On<UserOut>("LoginSuccess", (temp) =>
            {
                MenuView menu = new MenuView();
                menu.DataContext = mainModel;
                mainModel.CurrentUser = temp;
                mainModel.SetNewContent(menu);

            });
            MainViewModel.connection.On<string>("LoginError", (temp) =>
            {
                ButtonsViewModel viewmodel = new ButtonsViewModel(mainModel);
                Buttons buttons = new Buttons();
                buttons.DataContext = viewmodel;
                mainModel.CurrentView = buttons;
            });

            InitializeComponent();
            LoadJson();
            MainViewModel.CurrentTheme = StartupSettings.Theme;
            if (StartupSettings.RememberMe) 
            { 
                MainViewModel.connection.StartAsync();

                MainViewModel.connection.InvokeAsync("Login", new UserOut
                {
                    Name = StartupSettings.Login,
                    Pass = StartupSettings.Pass
                });
            }
            else
            {
                ButtonsViewModel viewmodel = new ButtonsViewModel(mainModel);
                Buttons buttons = new Buttons();
                buttons.DataContext = viewmodel;
                mainModel.CurrentView = buttons;
            }
            
            DataContext = mainModel;
        }

        public void LoadJson()
        {
            using (StreamReader r = new StreamReader("settings.json"))
            {
                string json = r.ReadToEnd();
                StartupSettings = JsonConvert.DeserializeObject<Settings>(json);
            }
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
            if (MainViewModel.connection.State != HubConnectionState.Disconnected)
            {
                if (mainModel.CurrentUser != null)
                {
                    await MainViewModel.connection.InvokeAsync("LogOut", mainModel.CurrentUser);
                }
                await MainViewModel.connection.StopAsync();
            }
            Application.Current.Shutdown();
        }
    }
}
