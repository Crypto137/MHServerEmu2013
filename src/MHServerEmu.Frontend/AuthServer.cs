﻿using System.Net;
using Gazillion;
using Google.ProtocolBuffers;
using MHServerEmu.Core.Config;
using MHServerEmu.Core.Helpers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Network;
using MHServerEmu.Core.System;

namespace MHServerEmu.Frontend
{
    public class AuthServer
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly IdGenerator _sessionIdGenerator = new(IdType.Session);
        private readonly string _url;

        private CancellationTokenSource _cts;
        private HttpListener _listener;

        public AuthServer()
        {
            var config = ConfigManager.Instance.GetConfig<AuthConfig>();

            _url = $"http://{config.AuthAddress}:{config.AuthPort}/";
        }

        public async void Run()
        {
            // Reset CTS
            _cts?.Dispose();
            _cts = new();

            // Create an http server and start listening for incoming connections
            _listener = new HttpListener();
            _listener.Prefixes.Add(_url);
            _listener.Start();

            Logger.Info($"AuthServer is listening on {_url}...");

            while (true)
            {
                try
                {
                    // Wait for a connection, and handle the request
                    HttpListenerContext context = await _listener.GetContextAsync().WaitAsync(_cts.Token);
                    await HandleMessageAsync(context.Request, context.Response);
                    context.Response.Close();
                }
                catch (TaskCanceledException) { return; }       // Stop handling connections
                catch (Exception e)
                {
                    Logger.Error($"Run(): Unhandled exception: {e}");
                }
            }
        }

        private async Task HandleMessageAsync(HttpListenerRequest request, HttpListenerResponse response)
        {
            MessageBuffer messageBuffer = new(request.InputStream);
            await OnLoginDataPB(request, response, messageBuffer);
        }

        private async Task SendMessageAsync(IMessage message, HttpListenerResponse response, int statusCode = 200)
        {
            MessagePackageOut messagePackage = new(message);

            response.StatusCode = statusCode;
            response.KeepAlive = false;
            response.ContentType = "application/octet-stream";
            response.ContentLength64 = messagePackage.GetSerializedSize();

            CodedOutputStream cos = CodedOutputStream.CreateInstance(response.OutputStream);
            await Task.Run(() => { messagePackage.WriteTo(cos); cos.Flush(); });
        }

        private async Task<bool> OnLoginDataPB(HttpListenerRequest request, HttpListenerResponse response, MessageBuffer messageBuffer)
        {
            var loginDataPB = messageBuffer.Deserialize<FrontendProtocolMessage>() as LoginDataPB;
            if (loginDataPB == null) return Logger.WarnReturn(false, $"OnLoginDataPB(): Failed to retrieve message");

            Logger.Info($"Sending AuthTicket to the game client on {request.RemoteEndPoint}");

            var frontendConfig = ConfigManager.Instance.GetConfig<FrontendConfig>();

            AuthTicket ticket = AuthTicket.CreateBuilder()
                .SetSessionId(_sessionIdGenerator.Generate())
                .SetSessionToken(ByteString.Unsafe.FromBytes(CryptographyHelper.GenerateToken()))
                .SetSessionKey(ByteString.Unsafe.FromBytes(CryptographyHelper.GenerateAesKey()))
                .SetSuccess(true)
                .SetFrontendServer(frontendConfig.PublicAddress)
                .SetFrontendPort(frontendConfig.Port)
                .Build();

            await SendMessageAsync(ticket, response);
            return true;
        }
    }
}
