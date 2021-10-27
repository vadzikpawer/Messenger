using Microsoft.AspNetCore.SignalR.Client;
using ModernClient.Core;
using ModernClient.MVVM1.Model;
using ModernClient.MVVM1.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using RelayCommand = ModernClient.Core.RelayCommand;

namespace ModernClient.MVVM1.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static HubConnection connection = new HubConnectionBuilder().WithUrl("http://168.63.110.193:80/chat").Build();
        public ObservableCollection<Message> Messages { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Sticker> Stickers { get; set; }

        private User _SelectedUser;
        private UserOut _currentUser;
        public ICommand LogOut { get; private set; }
        public UserOut CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        public MenuViewModel MenuVM { get; set; }
        public LoginView LoginVM { get; set; }
        public Buttons ButtonsView { get; set; }

        private UserControl _currentView { get; set; }
        public UserControl CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged("CurrentView");
            }
        }

        internal void SetNewContent(UserControl _content)
        {
            CurrentView = _content;
        }

        public static DispatcherTimer timer = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 1)

        };

        public static DispatcherTimer timer_users = new DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 1)

        };

        private Web_API API = new Web_API();

        public User SelectedUser
        {
            get
            {
                return _SelectedUser;
            }
            set
            {

                _SelectedUser = value;
                Get_msgWs();
                /*timer.Start();*/
                OnPropertyChanged("SelectedUser");
            }
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            var Get_msg = new Func<Task>(async () =>
            {
                ObservableCollection<Message> Temp_messages = new ObservableCollection<Message>();
                if (_SelectedUser != null)
                {
                    Temp_messages = await API.Async_GetMessages(new Message { From = CurrentUser.Id, FromName = CurrentUser.Name, To = SelectedUser.Id });
                    if (Temp_messages.Count > Messages.Count)
                    {
                        for (int i = Messages.Count; i < Temp_messages.Count; i++)
                        {
                            Messages.Add(Temp_messages[i]);
                        }
                        for (int i = 0; i < Messages.Count; i++)
                        {
                            if (Messages[i].IsSticker)
                            {
                                Messages[i].PathToSticker = Directory.GetCurrentDirectory() + "//Images//Stickers//" + Messages[i].PathToSticker;
                            }
                            Messages[i].dateStapm = Messages[i].dateStapm.ToLocalTime();
                        }

                    }
                    SelectedUser.Messages = Messages;
                    OnPropertyChanged("SelectedUser");
                }

            });

            Get_msg.Invoke();
            OnPropertyChanged("SelectedUser");
        }

        private void Timer_tick_users(object sender, EventArgs e)
        {
            var Get_user = new Func<Task>(async () =>
            {
                ObservableCollection<User> Temp_users = new ObservableCollection<User>();
                Temp_users = await API.Get_User_All_async();

                bool equal = true;
                if (Temp_users.Count == Users.Count)
                {

                    for (int i = 0; i < Temp_users.Count; i++)
                    {
                        if ((Temp_users[i].Id == Users[i].Id) && ((Temp_users[i].Name != Users[i].Name) || (Temp_users[i].Online != Users[i].Online)))
                        {
                            equal = false;
                        }
                    }
                }
                else equal = false;
                if (!equal)
                {
                    Users.Clear();
                    for (int i = 0; i < Temp_users.Count; i++)
                    {
                        Users.Add(Temp_users[i]);
                    }

                    OnPropertyChanged("Users");
                }
            }
            );

            Get_user.Invoke();

            OnPropertyChanged("Users");

        }
        public RelayCommand SendCommand { get; set; }
        public RelayCommand SendCommandWs { get; set; }
        public RelayCommand Home { get; set; }

        private string _message;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }

        private string _messageFrom;
        public string MessageFrom
        {
            get
            {
                return _messageFrom;
            }
            set
            {
                _messageFrom = value;
                OnPropertyChanged("MessageFrom");
            }
        }

        private Sticker _selectedSticker;

        public Sticker SelectedSticker
        {
            get { return _selectedSticker; }
            set
            {
                _selectedSticker = value;
                SendStickerWs();
                OnPropertyChanged("SelectedSticker");

            }
        }

        //private async Task SendSticker()
        //{
        //    await API.Send_message_async(new Message
        //    {
        //        From = CurrentUser.Id,
        //        FromName = CurrentUser.Name,
        //        To = SelectedUser.Id,
        //        Text = "",
        //        Color = CurrentUser.Color,
        //        dateStapm = System.DateTime.Now,
        //        IsSticker = true,
        //        PathToSticker = SelectedSticker.NameForSent
        //    }) ;

        //    Messages.Add(new Message
        //    {
        //        From = CurrentUser.Id,
        //        To = SelectedUser.Id,
        //        FromName = CurrentUser.Name,
        //        Text = "",
        //        dateStapm = System.DateTime.Now,
        //        IsSticker = true,
        //        PathToSticker = SelectedSticker.Name
        //    });
        //}

        private async void SendStickerWs()
        {
            await connection.InvokeAsync("Send", new Message
            {
                From = CurrentUser.Id,
                To = SelectedUser.Id,
                FromName = CurrentUser.Name,
                Text = "",
                Color = CurrentUser.Color,
                dateStapm = System.DateTime.Now,
                IsSticker = true,
                PathToSticker = SelectedSticker.NameForSent
            });

            Messages.Add(new Message
            {
                From = CurrentUser.Id,
                To = SelectedUser.Id,
                FromName = CurrentUser.Name,
                Text = "",
                Color = CurrentUser.Color,
                dateStapm = System.DateTime.Now,
                IsSticker = true,
                PathToSticker = SelectedSticker.Name
            });
            OnPropertyChanged("Messages");
        }

        private async void Get_msg()
        {
            Messages.Clear();
            if (_SelectedUser != null)
            {
                Messages = await API.Async_GetMessages(new Message { From = CurrentUser.Id, FromName = CurrentUser.Name, To = _SelectedUser.Id });
                for (int i = 0; i < Messages.Count; i++)
                {
                    if (Messages[i].IsSticker)
                    {
                        Messages[i].PathToSticker = Directory.GetCurrentDirectory() + "//Images//Stickers//" + Messages[i].PathToSticker;
                    }
                    Messages[i].dateStapm = Messages[i].dateStapm.ToLocalTime();
                }
                SelectedUser.Messages = Messages;
            }
            OnPropertyChanged("SelectedUser");
            OnPropertyChanged("Messages");
        }

        private async void Get_msgWs()
        {
            if (_SelectedUser != null)
            {
                await connection.InvokeAsync("GetMessages", new Message { From = CurrentUser.Id, FromName = CurrentUser.Name, To = _SelectedUser.Id });
            }
        }

        public static List<string> GetFilesFrom(string searchFolder, string[] filters, bool isRecursive)
        {
            List<string> filesFound = new List<string>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, string.Format("*.{0}", filter), searchOption));
            }

            return filesFound;
        }

        private async void LogOutCommand(object _param)
        {
            ButtonsViewModel ButtonsVM = new ButtonsViewModel(this);
            ButtonsView = new Buttons();
            ButtonsView.DataContext = ButtonsVM;
            Users.Clear();
            Messages.Clear();
            await connection.InvokeAsync("LogOut", CurrentUser);
            CurrentUser = null;
            SetNewContent(ButtonsView);
        }

        private bool CanLogOut(object _param)
        {
            return true;
        }

        private async Task SortUsers()
        {
            for (int i = 0; i < Users.Count - 1; i++)
            {
                User Max_user = Users[i];
                int Max = i;
                for (int j = i + 1; j < Users.Count; j++)
                {
                    if (Users[i].LastMessage < Users[j].LastMessage)
                    {
                        Max = j;
                    }
                }

                if (Max != i)
                {
                    Max_user = Users[i];
                    Users[i] = Users[Max];
                    Users[Max] = Max_user;

                }
            }

            await Task.Delay(3000);
        }
        public MainViewModel()
        {
            connection.StartAsync();
            Messages = new ObservableCollection<Message>();
            Users = new ObservableCollection<User>();
            CurrentUser = new UserOut();
            Stickers = new ObservableCollection<Sticker>();
            LogOut = new RelayCommand(LogOutCommand, CanLogOut);

            connection.On<ObservableCollection<User>>("UpdateUser", temp =>
            {
                bool equal = true;
                if (temp.Count == Users.Count)
                {
                    foreach (var item in temp)
                    {
                        User tempuser1 = item;
                        User tempuser2 = Users.FirstOrDefault(x => x.Id == tempuser1.Id);
                        tempuser2.Online = tempuser1.Online;
                    }

                }
                else
                {
                    equal = false;
                };
                if (!equal)
                {
                    Users.Clear();
                    for (int i = 0; i < temp.Count; i++)
                    {
                        Users.Add(temp[i]);
                    }
                    OnPropertyChanged("Users");
                }
                /*

                                for (int i = 0; i < Users.Count; i++)
                                {
                                    connection.InvokeAsync("GetLastMessage", new Message
                                    {
                                        From = CurrentUser.Id,
                                        FromName = CurrentUser.Name,
                                        To = Users[i].Id
                                    });
                                }*/

            });


            connection.On<Message>("NewMessage", message =>
            {
                if (message.From == SelectedUser.Id && message.From != CurrentUser.Id)
                {
                    User temp_user = Users.First(x => x.Id == message.To);

                    /*connection.InvokeAsync("GetLastMessage", new Message
                    {
                        From = CurrentUser.Id,
                        FromName = CurrentUser.Name,
                        To = temp_user.Id
                    });*/
                    if (message.IsSticker)
                    {
                        message.PathToSticker = Directory.GetCurrentDirectory() + "\\Images\\Stickers\\" + message.PathToSticker;
                    }
                    message.dateStapm = message.dateStapm.ToLocalTime();
                    Messages.Add(message);
                    OnPropertyChanged("Messages");
                }
            });

            connection.On<List<Message>>("ReceiveMessages", temp =>
            {
                Messages.Clear();
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].IsSticker)
                    {
                        temp[i].PathToSticker = Directory.GetCurrentDirectory() + "\\Images\\Stickers\\" + temp[i].PathToSticker;
                    }
                    temp[i].dateStapm = temp[i].dateStapm.ToLocalTime();
                    Messages.Add(temp[i]);
                }
                SelectedUser.Messages = Messages;
                OnPropertyChanged("SelectedUser");
            }
            );

            /*connection.On<DateTime, int>("RecieveLastMessage", async (_temp, id) =>
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    if (Users[i].Id == id)
                    {
                        Users[i].LastMessage = _temp;
                    }
                }

                await SortUsers();
                OnPropertyChanged("Users");
            });*/

            connection.On<string>("NoMessage", _temp =>
           {
               OnPropertyChanged("Users");
           });

            string searchFolder = Directory.GetCurrentDirectory() + "//Images//Stickers//";
            var filters = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
            var filesFound = GetFilesFrom(searchFolder, filters, false);

            for (int i = 0; i < filesFound.Count; i++)
            {
                Stickers.Add(new Sticker { Name = filesFound[i], NameForSent = filesFound[i].Substring(searchFolder.Length) });
            }



            timer.Tick += Timer_tick;
            timer_users.Tick += Timer_tick_users;

            SendCommandWs = new RelayCommand(async o =>
            {
                if (Message != null && SelectedUser != null)
                {
                    await connection.InvokeAsync("Send", new Message
                    {
                        From = CurrentUser.Id,
                        To = SelectedUser.Id,
                        FromName = CurrentUser.Name,
                        Text = Message,
                        Color = CurrentUser.Color,
                        dateStapm = System.DateTime.Now,
                        IsSticker = false
                    });

                    Messages.Add(new Message
                    {
                        From = CurrentUser.Id,
                        To = SelectedUser.Id,
                        FromName = CurrentUser.Name,
                        Text = Message,
                        Color = CurrentUser.Color,
                        dateStapm = System.DateTime.Now,
                        IsSticker = false
                    });
                    /*SelectedUser.LastMessage = Messages.Last().dateStapm;*/
                    Message = "";
                    /*await SortUsers();*/
                    OnPropertyChanged("Messages");
                    /*SelectedUser = Users.First();*/
                }
            });

            SendCommand = new RelayCommand(async o =>
            {
                if (Message != null && SelectedUser != null)
                {
                    await API.Send_message_async(new Message
                    {
                        From = CurrentUser.Id,
                        FromName = CurrentUser.Name,
                        To = SelectedUser.Id,
                        Text = Message,
                        Color = CurrentUser.Color,
                        dateStapm = System.DateTime.Now,
                        IsSticker = false
                    });

                    Messages.Add(new Message
                    {
                        From = CurrentUser.Id,
                        To = SelectedUser.Id,
                        FromName = CurrentUser.Name,
                        Text = Message,
                        Color = CurrentUser.Color,
                        dateStapm = System.DateTime.Now,
                        IsSticker = false
                    });

                    Message = "";
                }
            });

            Home = new RelayCommand(o =>
            {
                Messages.Clear();
            });

        }
    }
}
