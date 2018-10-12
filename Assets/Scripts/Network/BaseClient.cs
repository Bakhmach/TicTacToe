using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TicTacToe.Network
{
    public abstract class BaseClient : MonoBehaviour
    {
        public event NetworkMessageDelegate OnConnect = delegate { };
        public event NetworkMessageDelegate OnDisconnect = delegate { };

        [Header("Network Settings")]
        public string ip = "localhost";
        public int port = 7070;

        protected NetworkClient client;

        public bool IsRuning { get; private set; }

        public void Connect()
        {
            client = new NetworkClient();

            var config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableFragmented);
            config.AddChannel(QosType.UnreliableFragmented);

            client.Configure(config, 1);

            RegisterHandles();
            client.Connect(ip, port);

            IsRuning = true;
        }

        public void ShutDown()
        {
            client.Shutdown();
            IsRuning = false;
        }

        protected void Send(short msgType, MessageBase msg)
        {
            client.Send(msgType, msg);
        }

        protected virtual void RegisterHandles()
        {
            client.RegisterHandler(MsgType.Connect, OnConnect);
            client.RegisterHandler(MsgType.Disconnect, OnDisconnect);
        }

    }
}
