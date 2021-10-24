using ModernClient.Core;

namespace ModernClient.MVVM1.ViewModel
{
    class MenuViewModel : ObservableObject
    {
        private MainViewModel _mainModel;

        public MenuViewModel(MainViewModel mainModel)
        {
            this._mainModel = mainModel;
        }
    }
}
