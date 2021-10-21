using ModernClient.Core;
using ModernClient.MVVM1.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using RelayCommand = ModernClient.Core.RelayCommand;

namespace ModernClient.MVVM1.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public MenuViewModel MenuVM{ get; set; }
        public LoginViewModel LoginVM{ get; set; }

        private object _currentView { get; set; }
        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value;
                OnPropertyChanged();
            }
        }


        public static DispatcherTimer timer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 1)

        };
        
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<User> Users { get; set; }

        private Web_API API = new Web_API();

        private User _SelectedUser;
        public User SelectedUser
        {
            get
            {
                return _SelectedUser;
            }
            set
            {

                _SelectedUser = value;
               /* Messages.Clear();
                if (_SelectedUser != null)
                {
                    Messages = API.GetMessages(new Message { From = "test1", To = _SelectedUser.Name });
                    _SelectedUser.Messages = Messages;
                }
                
                timer.Start();*/
                OnPropertyChanged();
            }
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            var Get_msg = new Func<Task>(async () =>
            {
                ObservableCollection<Message> Temp_messages = new ObservableCollection<Message>();
                if (_SelectedUser != null)
                {
                    Temp_messages = await API.Async_GetMessages(new Message { From = "test1", To = _SelectedUser.Name });
                    if (Temp_messages.Count > Messages.Count)
                    {
                        for (int i = Messages.Count; i < Temp_messages.Count; i++)
                        {
                            Messages.Add(Temp_messages[i]);
                        }

                    }
                    if (_SelectedUser != null) _SelectedUser.Messages = Messages;
                }
            });

            Get_msg.Invoke();
            OnPropertyChanged();
        }

        public RelayCommand SendCommand { get; set; }
        public RelayCommand LoginCommand{ get; set; }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }

        private string _login;

        public string Login
        {
            get { return _login; }
            set { _login = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            LoginVM = new LoginViewModel();
            MenuVM = new MenuViewModel();
            CurrentView = MenuVM;

            Messages = new ObservableCollection<Message>();
            Users = new ObservableCollection<User>();

            timer.Tick += Timer_tick;

            SendCommand = new RelayCommand(async o =>
            {
                if (Message != null && SelectedUser != null)
                {
                    await API.Send_message_async(new Message
                    {
                        From = "test1",
                        To = SelectedUser.Name,
                        Text = Message,
                        dateStapm = System.DateTime.Now
                    });

                    Messages.Add(new Message
                    {
                        From = "test1",
                        To = SelectedUser.Name,
                        Text = Message,
                        dateStapm = System.DateTime.Now
                    });

                    Message = "";
                }
            });

            LoginCommand = new RelayCommand(o =>
            {
                CurrentView = null;
                CurrentView = new MenuViewModel();
            });

            Messages.Add(new Message
            {
                From = "test1",
                To = "test2",
                Text = "Test",
                dateStapm = DateTime.UtcNow
            });

            for (int i = 0; i < 1; i++)
            {
                Users.Add(new User
                {
                    Name = $"test2",
                    Id = i,
                    Color = "CornflowerBlue",
                    Messages = Messages
                });
            }
        }
    }
}
