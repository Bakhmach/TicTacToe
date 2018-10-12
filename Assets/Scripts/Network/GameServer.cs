using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using TicTacToe.Core;
using System;

namespace TicTacToe.Network
{
    public class NetPlayer : IBoardPlayer
    {
        private readonly NetworkConnection connection;

        public NetworkConnection Connection { get { return connection; } }
        public CellSign Sign { get; set; }
        public int Score { get; set; }
        public bool Ready { get; set; }

        public NetPlayer(NetworkConnection conn)
        {
            connection = conn;
            Score = 0;
            Ready = false;
            Sign = CellSign.None;
        }
    }

    public sealed class GameServer : BaseServer
    {
        private IRules<NetPlayer> rules = new ClassicRules<NetPlayer>(new ClassicBoard());

        public override void ShutDown()
        {
            rules.RemoveAllPlayers();
            base.ShutDown();
        }

        protected override void RegisterHandles()
        {
            base.RegisterHandles();

            NetworkServer.RegisterHandler((short)MessageType.NewRound, OnPlayerReady);
            NetworkServer.RegisterHandler((short)MessageType.MakeTurn, OnMakeTurn);
        }

        private void OnPlayerReady(NetworkMessage netMsg)
        {
            bool allReady = true;
            foreach (var player in rules)
            {
                if (player.Connection.connectionId == netMsg.conn.connectionId)
                {
                    player.Ready = true;
                }

                if (!player.Ready) allReady = false;
            }

            if (rules.PlayersAll() && allReady)
            {
                rules.NewGame();
                StartNewRound();
            }
        }

        private void OnMakeTurn(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<MakeTurnMessage>();

            rules.Board.MakeTurn(msg.index, msg.whoseTurnWas);
            SendAll((short)MessageType.MakeTurn, msg);

            if(rules.Board.Winner != CellSign.None || rules.Board.IsFullMarked)
            {
                foreach (var player in rules)
                {
                    player.Ready = false;

                    Send(player.Connection.connectionId, (short)MessageType.EndRound, new EndRoundMessage
                    {
                        winner = rules.Board.Winner,
                        yourScore = player.Score,
                        enemyScore = rules.OtherPlayer(player).Score
                    });
                }
            }
        }

        protected override void OnConnected(NetworkMessage netMsg)
        {
            if (!rules.AddPlayer(new NetPlayer(netMsg.conn)))
            {
                netMsg.conn.Disconnect();
                return;
            }
        }

        private void StartNewRound()
        {
            rules.NewRound();

            foreach (var player in rules)
            {
                Send(player.Connection.connectionId, (short)MessageType.NewRound,
                    new StartNewRoundMessage
                    {
                        yourSign = player.Sign,
                        whoseTurn = rules.FirstTurn,
                        yourScore = player.Score,
                        enemyScore = rules.OtherPlayer(player).Score
                    });
            }
        }
    }
}
