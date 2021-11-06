using Microsoft.AspNetCore.SignalR.Client;
using ModernClient.Core;
using ModernClient.MVVM1.Model;
using ModernClient.MVVM1.View;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ObservableObject = ModernClient.Core.ObservableObject;

namespace ModernClient.MVVM1.ViewModel
{
    class LoginViewModel : ObservableObject
    {
        MainViewModel _mainModel;

        public ICommand LoginCommand { get; private set; }

        public LoginViewModel(MainViewModel mainModel)
        {
            _mainModel = mainModel;
            LoginCommand = new RelayCommand(Login, CanLogin);
            MainViewModel.connection.On<UserOut>("LoginSuccess", (temp) =>
            {
                MenuView menu = new MenuView();
                menu.DataContext = _mainModel;
                LoginGet = null;
                LoginView.pass.Password = null;
                _mainModel.CurrentUser = temp;
                _mainModel.SetNewContent(menu);

            });

            MainViewModel.connection.On<string>("LoginError", (temp) =>
            {
                LoginGet = "";
                LoginView.pass.Password = "";
                Error = "Пользователь с таким именем и паролем не найден";
            });
        }

        private void Login(object _param)
        {
            Login_commandWs();
        }

        private bool CanLogin(object _param)
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
        public async void Login_commandWs()
        {
            if (MainViewModel.connection.State == HubConnectionState.Connected)
            {
                if ((LoginGet != null && LoginView.pass.Password != null) || (LoginGet != "" && LoginView.pass.Password != ""))
                {
                    await MainViewModel.connection.InvokeAsync("Login", new UserOut
                    {
                        Name = LoginGet,
                        Pass = ComputeSha512Hash(LoginView.pass.Password)
                    });
                }
                else if (LoginGet == null || LoginGet == "")
                {
                    Error = "Введите имя пользователя";
                }
                else if (LoginView.pass.Password == null || LoginView.pass.Password == "")
                {
                    Error = "Введите пароль";
                }
            }
            else
            {
                Error = "Сервер недоступен";
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
