using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;

namespace Advertising_platforms
{
    internal class Program
    {
        private static Dictionary<string, List<string>> AdStorage = new Dictionary<string, List<string>>();

        static void Main(string[] args)
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Сервер запущен на http://localhost:5000/");

            while (true)
            {
                var context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(o => HandleRequest(context));
            }
        }
        private static void HandleRequest(HttpListenerContext context)
        {
            string path = context.Request.Url.AbsolutePath.ToLower();
            string responseText = "";

            try
            {
                if (path == "/upload" && context.Request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    {
                        string fileContent = reader.ReadToEnd();
                        LoadData(fileContent);
                    }

                    responseText = "{\"message\":\"Data uploaded successfully\"}";
                    context.Response.StatusCode = 200;
                }
                else if (path == "/search" && context.Request.HttpMethod == "GET")
                {
                    string location = context.Request.QueryString["location"];
                    if (string.IsNullOrEmpty(location))
                    {
                        context.Response.StatusCode = 400;
                        responseText = "{\"error\":\"Missing location parameter\"}";
                    }
                    else
                    {
                        var platforms = SearchPlatforms(location);
                        responseText = "[\"" + string.Join("\",\"", platforms) + "\"]";
                        context.Response.StatusCode = 200;
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                    responseText = "{\"error\":\"Not found\"}";
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                responseText = "{\"error\":\"" + ex.Message + "\"}";
            }

            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        private static void LoadData(string fileContent)
        {
            var newData = new Dictionary<string, List<string>>();
            var lines = fileContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                var platform = parts[0].Trim();
                var locations = parts[1].Split(',', (char)StringSplitOptions.RemoveEmptyEntries);

                foreach (var loc in locations)
                {
                    var normalized = loc.Trim();
                    if (!newData.ContainsKey(normalized))
                        newData[normalized] = new List<string>();
                    newData[normalized].Add(platform);
                }
            }

            AdStorage = newData;
        }

        private static List<string> SearchPlatforms(string location)
        {
            var result = new HashSet<string>();
            var parts = location.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = parts.Length; i > 0; i--)
            {
                var prefix = "/" + string.Join("/", parts.Take(i));
                if (AdStorage.TryGetValue(prefix, out var platforms))
                {
                    foreach (var p in platforms)
                        result.Add(p);
                }
            }

            return result.ToList();
        }
    }
}

