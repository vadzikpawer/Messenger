using ModernClientNet6.Core;

namespace ModernClientNet6.MVVM.ViewModel
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
