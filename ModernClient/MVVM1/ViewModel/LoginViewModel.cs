using Microsoft.AspNetCore.SignalR.Client;
using ModernClient.Core;
using ModernClient.MVVM1.Model;
using ModernClient.MVVM1.View;
using System.Windows.Input;
using ObservableObject = ModernClient.Core.ObservableObject;

namespace ModernClient.MVVM1.ViewModel
{
    class LoginViewModel : ObservableObject
    {
        MainViewModel _mainModel;
        Web_API API = new Web_API();

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
                Password = null;
                _mainModel.CurrentUser = temp;
                _mainModel.SetNewContent(menu);

            });

            MainViewModel.connection.On<string>("LoginError", (temp) =>
            {
                LoginGet = "";
                Password = "";
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

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        //public async void Login_command()
        //{
        //    Error = "";
        //    if ((LoginGet != null && Password != null) || (LoginGet != "" && Password != ""))
        //    {
        //        var response = await API.Get_User_async(new UserOut
        //        {
        //            Name = LoginGet,
        //            Pass = Password
        //        });
        //        if (response.GetType() == typeof(UserOut))
        //        {
        //            MenuView menu = new MenuView();
        //            menu.DataContext = _mainModel;
        //            LoginGet = null;
        //            Password = null;
        //            _mainModel.CurrentUser = (UserOut)response;
        //            //MainViewModel.timer_users.Start();
        //            _mainModel.SetNewContent(menu);
        //        }
        //        else if (response.GetType() == typeof(int))
        //        {
        //            if ((int)response == 404)
        //            {
        //                LoginGet = "";
        //                Password = "";
        //                Error = "Пользователь с таким именем и паролем не найден";
        //            }
        //        }
        //        else if (response.GetType() == typeof(string))
        //        {
        //            Error = (string)response;
        //        }
        //    }
        //    else if (LoginGet == null || LoginGet == "")
        //    {
        //        Error = "Введите имя пользователя";
        //    }
        //    else if (Password == null || Password == "")
        //    {
        //        Error = "Введите пароль";
        //    }
        //}

        public async void Login_commandWs()
        {
            if ((LoginGet != null && Password != null) || (LoginGet != "" && Password != ""))
            {
                await MainViewModel.connection.InvokeAsync("Login", new UserOut
                {
                    Name = LoginGet,
                    Pass = Password
                });
            }
            else if (LoginGet == null || LoginGet == "")
            {
                Error = "Введите имя пользователя";
            }
            else if (Password == null || Password == "")
            {
                Error = "Введите пароль";
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
