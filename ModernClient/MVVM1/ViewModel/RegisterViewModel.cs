using Microsoft.AspNetCore.SignalR.Client;
using ModernClient.Core;
using ModernClient.MVVM1.Model;
using ModernClient.MVVM1.View;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ModernClient.MVVM1.ViewModel
{
    class RegisterViewModel : ObservableObject
    {
        MainViewModel _mainModel;
        MenuView menu = new MenuView();
        public ICommand RegisterCommand { get; private set; }
        public RegisterViewModel(MainViewModel mainModel)
        {
            _mainModel = mainModel;
            RegisterCommand = new RelayCommand(Register, CanRegister);
            MainViewModel.connection.On<UserOut>("RegisterSuccess", (temp) =>
            {
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
