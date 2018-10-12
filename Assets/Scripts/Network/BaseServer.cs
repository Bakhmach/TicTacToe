using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace TicTacToe.Network
{
    public abstract class BaseServer : MonoBehaviour
    {
        public event NetworkMessageDelegate OnConnect = delegate { };
        public event NetworkMessageDelegate OnDisconnect = delegate { };

        [Header("Network Settings")]
        public string ip = "localhost";
        public int port = 7070;
        public int maxConnections = 2;

        public bool IsRuning { get; private set; }

        public bool Listen()
        {
            Setup();
            RegisterHandles();            
            bool isListen = NetworkServer.Listen(ip, port);

            IsRuning = isListen;
            return isListen;
        }

        public virtual void ShutDown()
        {
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
            IsRuning = false;
        }

        private void Setup()
        {
            var config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableFragmented);
            config.AddChannel(QosType.UnreliableFragmented);
            NetworkServer.Configure(config, 2);

            Application.runInBackground = true;
        }

        protected void SendAll(short msgType, MessageBase msg)
        {
            NetworkServer.SendToAll(msgType, msg);
        }

        protected void Send(int connId, short msgType, MessageBase msg)
        {
            NetworkServer.SendToClient(connId, msgType, msg);
        }

        protected virtual void RegisterHandles()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnConnected);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnDisconnected);
        }

        protected virtual void OnConnected(NetworkMessage netMsg)
        {
            OnConnect(netMsg);
        }
        protected virtual void OnDisconnected(NetworkMessage netMsg)
        {
            OnDisconnect(netMsg);
        }
    }
}
