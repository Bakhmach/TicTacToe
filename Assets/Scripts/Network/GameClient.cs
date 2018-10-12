using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using TicTacToe.Core;
using TicTacToe.UI;
using System;

namespace TicTacToe.Network
{

    public sealed class GameClient : BaseClient
    {
        public event System.Action<StartNewRoundMessage> OnNewRoundEvent = delegate { };
        public event System.Action<MakeTurnMessage> OnTurnEvent = delegate { };
        public event System.Action<EndRoundMessage> OnEndRoundEvent = delegate { };

        public void SendReadyMessage()
        {
            Send((short)MessageType.NewRound, new EmptyMessage());
        }

        public void SendTurnMesage(int index, CellSign sign)
        {
            Send((short)MessageType.MakeTurn,
                    new MakeTurnMessage
                    {
                        index = index,
                        whoseTurnWas = sign
                    });
        }

        protected override void RegisterHandles()
        {
            base.RegisterHandles();

            client.RegisterHandler((short)MessageType.NewRound, OnNewRound);
            client.RegisterHandler((short)MessageType.MakeTurn, OnTurn);
            client.RegisterHandler((short)MessageType.EndRound, OnEndRound);
        }

        private void OnNewRound(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<StartNewRoundMessage>();
            OnNewRoundEvent(msg);
        }

        private void OnTurn(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<MakeTurnMessage>();
            OnTurnEvent(msg);
        }

        private void OnEndRound(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<EndRoundMessage>();
            OnEndRoundEvent(msg);
        }
    }
}