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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RelayCommand = ModernClient.Core.RelayCommand;

namespace ModernClient.MVVM1.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static ResourceDictionary DarkTheme = new ResourceDictionary() { Source = new Uri("/Themes/DarkTheme.xaml", UriKind.RelativeOrAbsolute)};
        public static ResourceDictionary LightTheme = new ResourceDictionary() { Source = new Uri("/Themes/LightTheme.xaml", UriKind.RelativeOrAbsolute)};
        public static string iplocal = "http://localhost:5000/chat";
        public static string ipserver = "http://168.63.110.193:80/chat";

        public static string CurrentTheme  = "DarkTheme";

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

        public User SelectedUser
        {
            get
            {
                return _SelectedUser;
            }
            set
            {

                _SelectedUser = value;
                OnPropertyChanged("SelectedUser");
                if (value != null)
                {
                    new Task(async () =>
                    {
                        await connection.InvokeAsync("GetMessages", new Message { From = CurrentUser.Id, FromName = CurrentUser.Name, To = _SelectedUser.Id });
                    }).Start();
                }
            }
        }

        public RelayCommand SendCommand { get; set; }
        public RelayCommand SendCommandWs { get; set; }
        public RelayCommand Home { get; set; }

        public RelayCommand ChangeTheme { get; set; }

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
                new Task(async () =>
                {
                    await SendStickerWs();
                }).Start();
                Messages.Add(new Message
                {
                    From = CurrentUser.Id,
                    To = SelectedUser.Id,
                    FromName = CurrentUser.Name,
                    Text = "",
                    Color = CurrentUser.Color,
                    dateStapm = DateTime.Now,
                    IsSticker = true,
                    PathToSticker = SelectedSticker.Name
                });
                OnPropertyChanged("Messages");
                OnPropertyChanged("SelectedSticker");

            }
        }

        private async Task SendStickerWs()
        {
            await connection.InvokeAsync("Send", new Message
            {
                From = CurrentUser.Id,
                To = SelectedUser.Id,
                FromName = CurrentUser.Name,
                Text = "",
                Color = CurrentUser.Color,
                dateStapm = DateTime.Now,
                IsSticker = true,
                PathToSticker = SelectedSticker.NameForSent
            });

            SelectedUser.LastMessage = Messages.Last().dateStapm;
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

        public static HubConnection connection = new HubConnectionBuilder().WithUrl($"{ipserver}").Build();

        public MainViewModel()
        {
            Messages = new ObservableCollection<Message>();
            Users = new ObservableCollection<User>();
            CurrentUser = new UserOut();
            Stickers = new ObservableCollection<Sticker>();
            LogOut = new RelayCommand(async o =>
            {
                ButtonsViewModel ButtonsVM = new ButtonsViewModel(this);
                ButtonsView = new Buttons();
                ButtonsView.DataContext = ButtonsVM;
                Users.Clear();
                Messages.Clear();
                await connection.InvokeAsync("LogOut", CurrentUser);
                CurrentUser = null;
                SetNewContent(ButtonsView);
            });

            connection.On<ObservableCollection<User>>("UpdateUser", temp =>
            {
                bool equal = true;
                if (temp.Count == Users.Count)
                {
                    foreach (var item in temp)
                    {
                        User tempuser1 = item;
                        User tempuser2 = Users.Where(x => x.Id == tempuser1.Id).First();
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


                for (int i = 0; i < Users.Count; i++)
                {
                    connection.InvokeAsync("GetLastMessage", new Message
                    {
                        From = Users[i].Id,
                        FromName = CurrentUser.Name,
                        To = CurrentUser.Id
                    });
                }

            });

            connection.On<DateTime, int, int>("RecieveLastMessageFromAll", (_tempdate, id, unseen) =>
            {
                User temp = Users.Where(x => x.Id == id).First();
                temp.LastMessage = _tempdate.ToLocalTime();
                if (unseen != 0)
                {
                    temp.HaveUnseen = true;
                    temp.Unseen = unseen;
                }
                else
                {
                    temp.HaveUnseen = false;
                    temp.Unseen = unseen;
                }
            });


            connection.On<Message>("NewMessage", message =>
            {
                if (SelectedUser != null)
                {
                    if (message.From == SelectedUser.Id && message.From != CurrentUser.Id)
                    {
                        User temp_user = Users.First(x => x.Id == message.To);

                        if (message.IsSticker)
                        {
                            message.PathToSticker = Directory.GetCurrentDirectory() + "\\Images\\Stickers\\" + message.PathToSticker;
                        }
                        message.dateStapm = message.dateStapm.ToLocalTime();
                        Messages.Add(message);
                        OnPropertyChanged("Messages");
                    }
                }
                User temp = Users.Where(x => x.Id == message.From).First();
                temp.LastMessage = message.dateStapm.ToLocalTime();
                if (SelectedUser != null)
                {
                    if (SelectedUser.Id != message.From)
                    {
                        temp.HaveUnseen = true;
                        temp.Unseen += 1;
                    }
                    else
                    {
                        new Task(async () =>
                        {
                            await connection.InvokeAsync("SeenAllMessages", new Message { From = CurrentUser.Id, FromName = CurrentUser.Name, To = SelectedUser.Id });
                        }).Start();
                    }
                }
                else
                {
                    temp.HaveUnseen = true;
                    temp.Unseen += 1;
                }

            });

            connection.On<List<Message>>("ReceiveMessages", temp =>
            {
                SelectedUser.HaveUnseen = false;
                SelectedUser.Unseen = 0;
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

            connection.On<int>("NoMessage", _temp =>
           {
               Users.Where(x => x.Id == _temp).First().LastMessage = DateTime.MinValue;
           });

            string searchFolder = Directory.GetCurrentDirectory() + "//Images//Stickers//";
            var filters = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };
            var filesFound = GetFilesFrom(searchFolder, filters, false);

            for (int i = 0; i < filesFound.Count; i++)
            {
                Stickers.Add(new Sticker { Name = filesFound[i], NameForSent = filesFound[i].Substring(searchFolder.Length) });
            }

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
                        dateStapm = DateTime.Now,
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
                    SelectedUser.LastMessage = Messages.Last().dateStapm;
                    Message = "";
                    OnPropertyChanged("Messages");
                }
            });

            ChangeTheme = new RelayCommand(o =>
            {
                var app = (App)Application.Current;
                if (CurrentTheme == "DarkTheme")
                {
                    app.ChangeTheme(LightTheme);
                    CurrentTheme = "LightTheme";
                }
                else
                { 
                    app.ChangeTheme(DarkTheme);
                    CurrentTheme = "DarkTheme";
                }
            });

            Home = new RelayCommand(o =>
            {
                Messages.Clear();
            });

        }
    }
}
