using System.Windows.Controls;

namespace ModernClientNet6.MVVM.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public static PasswordBox pass;
        public static CheckBox rememberMe;
        public LoginView()
        {
            InitializeComponent();
            pass = PassBox;
            rememberMe = RememberMe;
        }

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
