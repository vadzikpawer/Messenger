
using ModernClient.MVVM1.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModernClient
{
    public class Web_API
    {
        static public User user;
        static public ObservableCollection<Message> Messages = new ObservableCollection<Message>();

        /*Web_API()
        {

        }*/
        //Not async
        public ObservableCollection<Message> GetMessages(Message request_get)
        {
            WebRequest request = WebRequest.Create("http://localhost:5000/api/messages/get");
            request.Method = "POST";
            string postData = JsonConvert.SerializeObject(request_get);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/json";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Messages = JsonConvert.DeserializeObject<ObservableCollection<Message>>(responseFromServer);
            return Messages;
        }

        public async Task<ObservableCollection<Message>> Async_GetMessages(Message request_get)
        {
            var json = JsonConvert.SerializeObject(request_get);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5000/api/messages/get";
            var client = new HttpClient();
            Console.WriteLine("Response");
            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            Messages = JsonConvert.DeserializeObject<ObservableCollection<Message>>(result);
            Console.WriteLine(result.ToString());
            return Messages;
        }


        public async Task<string> Send_message_async(Message message)
        {
            var json = JsonConvert.SerializeObject(message);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5000/api/messages/add";
            var client = new HttpClient();
            //Console.WriteLine("Response");
            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            Message message_in = JsonConvert.DeserializeObject<Message>(result);
            Console.WriteLine(result.ToString());
            return "ok"; //TODO: обработчик ошибок;
        }
        public async Task<string> Get_User_async(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5000/api/users/getuser";
            var client = new HttpClient();
            //Console.WriteLine("Response");
            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            User user_in = JsonConvert.DeserializeObject<User>(result);
            Console.WriteLine(result.ToString());
            return "ok"; //TODO: обработчик ошибок;
        }
        public async Task<string> Add_User_async(User user)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5000/api/users/adduser";
            var client = new HttpClient();
            //Console.WriteLine("Response");
            var response = await client.PostAsync(url, data);

            string result = response.Content.ReadAsStringAsync().Result;
            User user_in = JsonConvert.DeserializeObject<User>(result);
            Console.WriteLine(result.ToString());
            return "ok"; //TODO: обработчик ошибок;
        }
    }
}
