using System.Windows.Controls;

namespace ModernClientNet6.MVVM.View
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public static PasswordBox pass;
        public static CheckBox rememberMe;
        public RegisterView()
        {
            InitializeComponent();
            pass = PassBox;
            rememberMe = RememberMe;
        }
    }
}
