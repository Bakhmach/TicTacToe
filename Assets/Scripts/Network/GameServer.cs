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
        public const int playerMax = 2;

        private List<NetPlayer> players = new List<NetPlayer>(playerMax);
        private IBoard board = new ClassicBoard();

        public override void ShutDown()
        {
            players.Clear();
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
            //check if all players ready for next round
            bool allReady = true;
            for(int i = 0; i < players.Count; ++i)
            {
                if(players[i].Connection.connectionId == netMsg.conn.connectionId)
                {
                    players[i].Ready = true;
                }

                if (!players[i].Ready) allReady = false;
            }

            if (players.Count == playerMax && allReady) StartNewRound();
        }

        private void OnMakeTurn(NetworkMessage netMsg)
        {
            var msg = netMsg.ReadMessage<MakeTurnMessage>();

            board.MakeTurn(msg.index, msg.whoseTurnWas);
            SendAll((short)MessageType.MakeTurn, msg);

            if(board.Winner != CellSign.None || board.IsFullMarked)
            {
                //Setup score and ready status
                foreach(var player in players)
                {
                    if (player.Sign == board.Winner) player.Score++;
                    player.Ready = false;
                }

                //Send end round message for all players
                for (int i = 0; i < players.Count; ++i)
                {
                    Send(players[i].Connection.connectionId, (short)MessageType.EndRound, new EndRoundMessage
                    {
                        winner = board.Winner,
                        yourScore = players[i].Score,
                        enemyScore = players[(i + 1) % playerMax].Score
                    });
                }
            }
        }

        protected override void OnConnected(NetworkMessage netMsg)
        {
            if (players.Count > playerMax)
            {
                netMsg.conn.Disconnect();
                return;
            }

            players.Add(new NetPlayer(netMsg.conn));
        }

        private void StartNewRound()
        {
            board.Clear();

            CellSign firstTurn = UnityEngine.Random.value > 0.5f ? CellSign.Cross : CellSign.Nought;

            bool rand = UnityEngine.Random.value > 0.5f;
            players[0].Sign = rand ? CellSign.Cross : CellSign.Nought;
            players[1].Sign = !rand ? CellSign.Cross : CellSign.Nought;

            for (int i = 0; i < players.Count; ++i)
            {
                Send(players[i].Connection.connectionId, (short)MessageType.NewRound,
                    new StartNewRoundMessage
                    {
                        yourSign = players[i].Sign,
                        whoseTurn = firstTurn,
                        yourScore = players[i].Score,
                        enemyScore = players[(i + 1) % playerMax].Score
                    });
            }
        }
    }
}
