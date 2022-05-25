using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FaceNetCore4
{
    class WebServer
    {
        public void server()
        {
            HttpListener linstener = new HttpListener();
            linstener.Prefixes.Add("http://localhost:8081/");
            linstener.Start();
            Console.WriteLine("Listening...");
            Task task = Task.Factory.StartNew(() =>
            {
                while (linstener.IsListening)
                {
                    HttpListenerContext context = linstener.GetContext();
                    Console.WriteLine("Http requesting...");
                    HttpListenerRequest request = context.Request;
                    HttpListenerResponse response = context.Response;
                    Console.WriteLine("Http responseting...");
                    string responseString = "<html><title>Http server in C#</title><body><p>Hello world!</p></body></html>";
                    using (StreamWriter writer = new StreamWriter(response.OutputStream))
                    {
                        writer.WriteLine(responseString);
                    }
                }
            });
            task.Wait();
        }

    }
}
