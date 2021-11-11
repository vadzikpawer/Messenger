using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ModernClientNet6
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public void ChangeTheme(ResourceDictionary res)
        {
            ResourceDictionary ChatItem = new ResourceDictionary() { Source = new Uri("./Themes/ChatItem.xaml", UriKind.RelativeOrAbsolute) };
            ResourceDictionary ContactCard = new ResourceDictionary() { Source = new Uri("./Themes/ContactCard.xaml", UriKind.RelativeOrAbsolute) };
            ResourceDictionary InformButton = new ResourceDictionary() { Source = new Uri("./Themes/InformButton.xaml", UriKind.RelativeOrAbsolute) };
            ResourceDictionary MessageBox = new ResourceDictionary() { Source = new Uri("./Themes/MessageBox.xaml", UriKind.RelativeOrAbsolute) };
            ResourceDictionary StickerMenu = new ResourceDictionary() { Source = new Uri("./Themes/StickerMenu.xaml", UriKind.RelativeOrAbsolute) };
            ResourceDictionary StylishScrollbar = new ResourceDictionary() { Source = new Uri("./Themes/StylishScrollbar.xaml", UriKind.RelativeOrAbsolute) };
            Resources.MergedDictionaries.Clear();
            Resources.MergedDictionaries.Add(ChatItem);
            Resources.MergedDictionaries.Add(ContactCard);
            Resources.MergedDictionaries.Add(InformButton);
            Resources.MergedDictionaries.Add(MessageBox);
            Resources.MergedDictionaries.Add(StickerMenu);
            Resources.MergedDictionaries.Add(StylishScrollbar);
            Resources.MergedDictionaries.Add(res);
        }
    }
}
