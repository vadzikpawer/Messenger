using System.Windows.Controls;

namespace ModernClient.MVVM1.View
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public static PasswordBox pass;
        public RegisterView()
        {
            InitializeComponent();
            pass = PassBox;
        }
    }
}
