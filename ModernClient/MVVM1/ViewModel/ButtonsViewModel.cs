using ModernClient.Core;
using ModernClient.MVVM1.View;
using System.Windows.Input;

namespace ModernClient.MVVM1.ViewModel
{
    class ButtonsViewModel : ObservableObject
    {
        MainViewModel _mainModel;
        LoginView login = new LoginView();
        RegisterView register = new RegisterView();
        public ICommand LoginSwitch { get; private set; }
        public ICommand RegisterSwitch { get; private set; }
        public ButtonsViewModel(MainViewModel model)
        {
            _mainModel = model;

            LoginSwitch = new RelayCommand(LoginCommand, CanLogin);
            RegisterSwitch = new RelayCommand(RegisterCommand, CanLogin);
        }

        private void LoginCommand(object _param)
        {
            LoginViewModel LoginVM = new LoginViewModel(_mainModel);
            login.DataContext = LoginVM;
            _mainModel.SetNewContent(login);
        }

        private void RegisterCommand(object _param)
        {
            RegisterViewModel RegisterVM = new RegisterViewModel(_mainModel);
            register.DataContext = RegisterVM;
            _mainModel.SetNewContent(register);
        }


        private bool CanLogin(object _param)
        {
            return true;
        }

    }
}
