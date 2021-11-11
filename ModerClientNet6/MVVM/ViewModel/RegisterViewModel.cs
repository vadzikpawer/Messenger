using Microsoft.AspNetCore.SignalR.Client;
using ModernClientNet6.MVVM.View;
using ModernClientNet6.Core;
using ModernClientNet6.MVVM.Model;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.IO;
using Newtonsoft.Json;

namespace ModernClientNet6.MVVM.ViewModel
{
    class RegisterViewModel : ObservableObject
    {
        MainViewModel _mainModel;
        MenuView menu = new MenuView();
        public ICommand RegisterCommand { get; private set; }
        public ICommand ChangeThemeCommand { get; private set; }
        public RegisterViewModel(MainViewModel mainModel)
        {
            _mainModel = mainModel;
            RegisterCommand = new RelayCommand(Register, CanRegister);
            ChangeThemeCommand = new RelayCommand(ChangeTheme, CanRegister);
            MainViewModel.connection.On<UserOut>("RegisterSuccess", (temp) =>
            {
                MainWindow.StartupSettings.RememberMe = (bool)LoginView.rememberMe.IsChecked;
                if (MainWindow.StartupSettings.RememberMe)
                {
                    MainWindow.StartupSettings.Login = LoginGet;
                    MainWindow.StartupSettings.Pass = ComputeSha512Hash(LoginView.pass.Password);
                    WriteJson();
                }

                MenuView menu = new MenuView();
                menu.DataContext = _mainModel;
                LoginGet = null;
                RegisterView.pass.Password = null;
                _mainModel.CurrentUser = temp;
                _mainModel.SetNewContent(menu);

            });
            MainViewModel.connection.On<string>("UserExist", (temp) =>
            {
                LoginGet = "";
                RegisterView.pass.Password = "";
                Error = "Пользователь с таким именем уже существует";
            });

        }

        public void WriteJson()
        {
            using (StreamWriter r = new StreamWriter("settings.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(r, MainWindow.StartupSettings);
            }
        }

        private void ChangeTheme(object _param)
        {
            var app = (App)Application.Current;
            if (MainViewModel.CurrentTheme == "DarkTheme")
            {
                app.ChangeTheme(MainViewModel.LightTheme);
                MainViewModel.CurrentTheme = "LightTheme";
            }
            else
            {
                app.ChangeTheme(MainViewModel.DarkTheme);
                MainViewModel.CurrentTheme = "DarkTheme";
            }
        }

        private void Register(object _param)
        {
            menu.DataContext = _mainModel;
            Login_commandWs();
        }

        private bool CanRegister(object _param)
        {
            return true;
        }

        private string _login;

        public string LoginGet
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public async void Login_commandWs()
        {
            Error = "";
            if (MainViewModel.connection.State != HubConnectionState.Disconnected)
                if ((LoginGet != null && RegisterView.pass.Password != null) || (LoginGet != "" && RegisterView.pass.Password != ""))
                {
                    await MainViewModel.connection.InvokeAsync("Register", new UserOut
                    {
                        Name = LoginGet,
                        Pass = ComputeSha512Hash(RegisterView.pass.Password)
                    });

                }
                else if (LoginGet == null || LoginGet == "")
                {
                    Error = "Введите имя пользователя";
                }
                else if (RegisterView.pass.Password == null || RegisterView.pass.Password == "")
                {
                    Error = "Введите пароль";
                }
                else
                {
                    Error = "Сервер недоступен";
                }
        }

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static string ComputeSha512Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA512 sha256Hash = SHA512.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private string _error;

        public string Error
        {
            get { return _error; }
            set
            {
                _error = value;
                OnPropertyChanged();
            }
        }
    }
}
