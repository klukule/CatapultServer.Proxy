using System;
using System.IO;
using Catapult.Protocol;
using ProtoBuf;
using ProtoBuf.Meta;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CatapultServer.Proxy
{
    public class FGLoginServerProxy : WebSocketBehavior
    {
        private readonly WebSocket _proxyClient;
        public FGLoginServerProxy()
        {
            // Create connection to real server
            _proxyClient = new WebSocket("wss://login-prod.fallguys.oncatapult.com/ws")
            {
                SslConfiguration =
                {
                    ServerCertificateValidationCallback = (s, c, ch, spe) => true,
                    EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12
                }
            };

            // Handle server -> client messages
            _proxyClient.OnMessage += (s, e) =>
            {
                Message message = null;
                var messageId = -1;
                // Deserialize message
                {
                    using var ms = new MemoryStream(e.RawData);
                    message = Serializer.Deserialize<Message>(ms);
                }

                // Extract Field header containing message ID - used when specific message type is not yet implemented
                {
                    using var ms = new MemoryStream(e.RawData);
                    using var state = ProtoReader.State.Create(ms, RuntimeTypeModel.Default);
                    messageId = state.ReadFieldHeader();
                }

                Console.WriteLine("LOGIN -> CLIENT | Id = {0} Type = {1}", messageId.ToString().PadLeft(8, ' '), message.GetTypeName());
                // Print raw message data in hex
                //Console.WriteLine(BitConverter.ToString(e.RawData));

                // Print more human readable message body
                //Console.WriteLine(Tools.JsonPrettify(message));

                //Console.WriteLine("==============================================");
                var rawData = e.RawData;
                Send(rawData);
            };
            _proxyClient.Connect();
        }
        /// <summary>
        /// Handles Client -> Server messages
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
            Message message = null;
            var messageId = -1;
            // Deserialize message
            {
                using var ms = new MemoryStream(e.RawData);
                message = Serializer.Deserialize<Message>(ms);
            }

            // Extract Field header containing message ID - used when specific message type is not yet implemented
            {
                using var ms = new MemoryStream(e.RawData);
                using var state = ProtoReader.State.Create(ms, RuntimeTypeModel.Default);
                messageId = state.ReadFieldHeader();
            }

            Console.WriteLine("CLIENT -> LOGIN | Id = {0} Type = {1}", messageId.ToString().PadLeft(8, ' '), message.GetTypeName());
            // Print raw message data in hex
            //Console.WriteLine(BitConverter.ToString(e.RawData));

            // Print more human readable message body
            //Console.WriteLine(Tools.JsonPrettify(message));

            //Console.WriteLine("==============================================");
            var rawData = e.RawData;
            _proxyClient.Send(rawData);
        }
    }
}
