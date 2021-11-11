using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernClientNet6.MVVM.View
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        private bool MenuOpen = false;
        public static CollectionView view;
        public static ListView _userlist;
        public MenuView()
        {
            InitializeComponent();
            _userlist = UsersList;
        }

        private void Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BoxMessage.Visibility = Visibility.Visible;
            temp_logo.Visibility = Visibility.Collapsed;
            Chat.SelectedIndex = Chat.Items.Count - 1;
            Chat.ScrollIntoView(Chat.SelectedItem);
        }

        private void Home_button_Click(object sender, RoutedEventArgs e)
        {
            Users.UnselectAll();
            BoxMessage.Visibility = Visibility.Collapsed;
            temp_logo.Visibility = Visibility.Visible;
        }

        private void Sticker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Stickers.Visibility = Visibility.Collapsed;
            MenuOpen = false;
        }


        private void StickerMenu_Click(object sender, RoutedEventArgs e)
        {
            if (!MenuOpen)
            {
                Stickers.Visibility = Visibility.Visible;
                MenuOpen = true;
            }
            else
            {
                Stickers.Visibility = Visibility.Collapsed;
                MenuOpen = false;
            }
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
