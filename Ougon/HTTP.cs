using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Ougon.Data;

namespace Ougon
{
    class HTTPServer
    {
        public static HttpListener listener = new HttpListener();
        public static string url = "http://localhost:1234/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData =
            "<!DOCTYPE>"
            + "<html>"
            + "  <head>"
            + "    <title>HttpListener Example</title>"
            + "  </head>"
            + "  <body>"
            + "    <p>Page Views: {0}</p>"
            + "    <form method=\"post\" action=\"shutdown\">"
            + "      <input type=\"submit\" value=\"Shutdown\" {1}>"
            + "    </form>"
            + "  </body>"
            + "</html>";

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                var result = new List<Struct>();
                var structs = new List<Type>
                {
                    typeof(Match),
                    typeof(Sequence),
                    typeof(GameCharacter),
                    typeof(Frame),
                    typeof(Hitbox),
                    typeof(SequenceHeader)
                };

                foreach (var type in structs)
                {
                    var resultFields = new List<Field>();

                    var fields = type.GetFields();
                    foreach (FieldInfo field in fields)
                    {
                        var offsetAttribute = field.GetCustomAttribute<FieldOffsetAttribute>();
                        var offset = offsetAttribute != null ? offsetAttribute.Value : -1;

                        var resultField = new Field
                        {
                            name = field.Name,
                            offset = offset,
                            type = field.FieldType.ToString()
                        };
                        resultFields.Add(resultField);
                    }

                    var resultStruct = new Struct
                    {
                        name = type.Name,
                        fields = resultFields.ToArray()
                    };

                    result.Add(resultStruct);
                }

                var resultJson = JsonSerializer.Serialize(result);

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(resultJson);
                resp.ContentType = "application/json";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public static Task Start()
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            return HandleIncomingConnections();
        }
    }

    class Field
    {
        public string name { get; set; }
        public int offset { get; set; }
        public string type { get; set; }
    }

    class Struct
    {
        public string name { get; set; }
        public Field[] fields { get; set; }
    }
}
