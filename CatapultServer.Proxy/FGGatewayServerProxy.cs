using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Catapult.Protocol;
using FallGuys.Gateway.Protocol.Client.Matchmaking;
using FallGuys.Gateway.Protocol.Client.PlayerDisconnected;
using ProtoBuf;
using ProtoBuf.Meta;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace CatapultServer.Proxy
{
    public class FGGatewayServerProxy : WebSocketBehavior
    {
        private readonly WebSocket _proxyClient;
        public FGGatewayServerProxy()
        {
            // Create connection to real server
            _proxyClient = new WebSocket("wss://gateway-prod.fallguys.oncatapult.com/ws")
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

                Console.WriteLine("GATEWAY -> CLIENT | Id = {0} Type = {1}", messageId.ToString().PadLeft(8, ' '), message.GetTypeName());

                // Spoof Match found response and join custom UNet server
                /*if (message is Message<MatchFoundResponse> mfr)
                {
                    var originalIp = mfr.Body.ServerIp;
                    var originalPort = mfr.Body.ServerPort;
                    mfr.Body.ServerAddress = "127.0.0.1:5556";
                    mfr.Body.ServerIp = "127.0.0.1";
                    mfr.Body.ServerPort = "5556";
                    Console.WriteLine("Starting UNet proxy server");
                    // Program.GameServer.Start(originalIp, ushort.Parse(originalPort), ushort.Parse(mfr.Body.ServerPort)); // Start UNet proxy to capture messages from original server to client
                    Console.WriteLine("UNet proxy server started");
                    Console.WriteLine(Tools.JsonPrettify(mfr));
                    Console.WriteLine("==============================================");
                    using var ms = new MemoryStream();
                    Serializer.Serialize(ms, mfr);
                    Send(ms.ToArray());
                }
                else if (message is Message<GameServerDisconnectNotification> mgsdn)
                {
                }
                else*/
                {
                    // Print raw message data in hex
                    //Console.WriteLine(BitConverter.ToString(e.RawData));

                    // Print more human readable message body
                    //Console.WriteLine(Tools.JsonPrettify(message));

                    //Console.WriteLine("==============================================");
                    var rawData = e.RawData;
                    Send(rawData);
                }
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

            Console.WriteLine("CLIENT -> GATEWAY | Id = {0} Type = {1}", messageId.ToString().PadLeft(8, ' '), message.GetTypeName());
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
