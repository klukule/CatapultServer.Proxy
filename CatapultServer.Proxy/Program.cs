using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Catapult.Protocol;
using ProtoBuf;
using WebSocketSharp.Server;

namespace CatapultServer.Proxy
{
    class Program
    {
        static void Main(string[] args)
        {
            // Build serializer and check for errors
            Serializer.PrepareSerializer<Message>();

            // Start login and gateway servers
            var loginServer = new WebSocketServer(IPAddress.Parse("127.0.0.1"), 443, true) // Bind login server to 127.0.0.1
            {
                SslConfiguration = { ServerCertificate = new X509Certificate2("F:\\Temp\\CertGen\\certificate.pfx", "falling123") } // TODO: Assign custom trusted certificate here
            };
            loginServer.AddWebSocketService<FGLoginServerProxy>("/ws");
            loginServer.Start();

            var gatewayServer = new WebSocketServer(IPAddress.Parse("192.168.1.6"), 443, true) // TODO: Assign localhost IP which is different from 127.0.0.1
            {
                SslConfiguration = { ServerCertificate = new X509Certificate2("F:\\Temp\\CertGen\\certificate.pfx", "falling123") } // TODO: Assign custom trusted certificate here
            };
            gatewayServer.AddWebSocketService<FGGatewayServerProxy>("/ws");
            gatewayServer.Start();

            Console.WriteLine("Running");

            Console.ReadKey(true);

            loginServer.Stop();
            gatewayServer.Stop();
        }
    }
}
