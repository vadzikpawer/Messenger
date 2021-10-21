
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Web_API api = new Web_API();

            Message request = new Message();

            request.To = "test";
            request.From = "test";

            Console.WriteLine(await api.Async_GetMessages(request));

            Console.ReadKey();
        }
    }
}
