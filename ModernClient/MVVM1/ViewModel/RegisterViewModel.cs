﻿using Microsoft.AspNetCore.SignalR.Client;
using ModernClient.Core;
using ModernClient.MVVM1.Model;
using ModernClient.MVVM1.View;
using System.Windows.Input;

namespace ModernClient.MVVM1.ViewModel
{
    class RegisterViewModel : ObservableObject
    {
        MainViewModel _mainModel;
        Web_API API = new Web_API();
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
                Password = null;
                _mainModel.CurrentUser = temp;
                _mainModel.SetNewContent(menu);

            });
            MainViewModel.connection.On<string>("UserExist", (temp) =>
            {
                LoginGet = "";
                Password = "";
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

        public async void Login_command()
        {
            Error = "";
            if ((LoginGet != null && Password != null) || (LoginGet != "" && Password != ""))
            {
                var response = await API.Add_User_async(new UserOut
                {
                    Name = LoginGet,
                    Pass = Password
                });
                if (response.GetType() == typeof(UserOut))
                {
                    LoginGet = null;
                    Password = null;
                    _mainModel.CurrentUser = (UserOut)response;
                    //MainViewModel.timer_users.Start();
                    _mainModel.SetNewContent(menu);
                }
                else if (response.GetType() == typeof(int))
                {
                    if ((int)response == 400)
                    {
                        LoginGet = "";
                        Password = "";
                        Error = "Пользователь с таким именем уже существует";
                    }
                }
                else if (response.GetType() == typeof(string))
                {
                    Error = (string)response;
                }
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
        public async void Login_commandWs()
        {
            Error = "";
            if ((LoginGet != null && Password != null) || (LoginGet != "" && Password != ""))
            {
                await MainViewModel.connection.InvokeAsync("Register", new UserOut
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