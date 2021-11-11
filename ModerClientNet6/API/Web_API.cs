
using ModernClientNet6.MVVM.Model;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModernClientNet6
{
    public class Web_API
    {
        static public ObservableCollection<User> Users = new ObservableCollection<User>();
        static public ObservableCollection<Message> Messages = new ObservableCollection<Message>();
        UserOut user_in = new UserOut();

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

        public UserOut GetUser(UserOut request_get)
        {
            WebRequest request = WebRequest.Create("http://localhost:5000/api/users/login");
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
            user_in = JsonConvert.DeserializeObject<UserOut>(responseFromServer);
            return user_in;
        }

        public async Task<ObservableCollection<Message>> Async_GetMessages(Message request_get)
        {
            var json = JsonConvert.SerializeObject(request_get);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5000/api/messages/get";
            var client = new HttpClient();
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
        public async Task<ObservableCollection<User>> Get_User_All_async()
        {

            var url = "http://localhost:5000/api/users/all";
            var client = new HttpClient();
            //Console.WriteLine("Response");
            var response = await client.GetAsync(url);

            string result = response.Content.ReadAsStringAsync().Result;
            Users = JsonConvert.DeserializeObject<ObservableCollection<User>>(result);
            Console.WriteLine(result.ToString());
            return Users; //TODO: обработчик ошибок;
        }

        public async Task<object> Get_User_async(UserOut user)
        {

            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            string result;
            var url = "http://localhost:5000/api/users/login";
            var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, data);
                result = response.Content.ReadAsStringAsync().Result;
                if ((int)response.StatusCode == 200)
                {
                    user_in = JsonConvert.DeserializeObject<UserOut>(result);
                    Console.WriteLine(result.ToString());
                }
                else return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException ex1 = (WebException)ex;
                    WebExceptionStatus status = ex1.Status;

                    if (status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)ex1.Response;
                        Console.WriteLine("Статусный код ошибки: {0} - {1}",
                                (int)httpResponse.StatusCode, httpResponse.StatusCode);
                        return (int)httpResponse.StatusCode;
                    }
                }
                else if (ex is HttpRequestException)
                {
                    return (string)"Сервер недоступен";
                }
            }
            return user_in;
            //TODO: обработчик ошибок;
        }


        public async Task<object> LogOut_User_async(UserOut user)
        {

            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            string result;
            var url = "http://localhost:5000/api/users/logout";
            var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, data);
                result = response.Content.ReadAsStringAsync().Result;
                if ((int)response.StatusCode == 200)
                {
                    user_in = JsonConvert.DeserializeObject<UserOut>(result);
                    Console.WriteLine(result.ToString());
                }
                else return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException ex1 = (WebException)ex;
                    WebExceptionStatus status = ex1.Status;

                    if (status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)ex1.Response;
                        Console.WriteLine("Статусный код ошибки: {0} - {1}",
                                (int)httpResponse.StatusCode, httpResponse.StatusCode);
                        return (int)httpResponse.StatusCode;
                    }
                }
                else if (ex is HttpRequestException)
                {
                    return (string)"Сервер недоступен";
                }
            }
            return user_in;
            //TODO: обработчик ошибок;
        }
        public async Task<object> Add_User_async(UserOut user)
        {
            var json = JsonConvert.SerializeObject(user);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            string result;
            var url = "http://localhost:5000/api/users/add";
            var client = new HttpClient();
            try
            {
                var response = await client.PostAsync(url, data);
                result = response.Content.ReadAsStringAsync().Result;
                if ((int)response.StatusCode == 200)
                {
                    user_in = JsonConvert.DeserializeObject<UserOut>(result);
                    Console.WriteLine(result.ToString());
                }
                else return (int)response.StatusCode;
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException ex1 = (WebException)ex;
                    WebExceptionStatus status = ex1.Status;

                    if (status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse httpResponse = (HttpWebResponse)ex1.Response;
                        Console.WriteLine("Статусный код ошибки: {0} - {1}",
                                (int)httpResponse.StatusCode, httpResponse.StatusCode);
                        return (int)httpResponse.StatusCode;
                    }
                }
                else if (ex is HttpRequestException)
                {
                    return (string)"Сервер недоступен";
                }
            }
            return user_in;
        }
    }
}
