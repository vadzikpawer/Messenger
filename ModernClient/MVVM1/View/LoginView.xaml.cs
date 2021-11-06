using System.Windows.Controls;

namespace ModernClient.MVVM1.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public static PasswordBox pass;
        public LoginView()
        {
            InitializeComponent();
            pass = PassBox;
        }

        private void Login_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
