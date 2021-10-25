using ModernClient.Core;
using ModernClient.MVVM1.View;
using System.Windows.Input;

namespace ModernClient.MVVM1.ViewModel
{
    class MenuViewModel : ObservableObject
    {
        private MainViewModel _mainModel;
        
        private Web_API API = new Web_API();
        

        public MenuViewModel(MainViewModel mainModel)
        {
            this._mainModel = mainModel;
        }

    }
}
