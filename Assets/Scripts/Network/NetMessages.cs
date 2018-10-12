using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TicTacToe.Core;

namespace TicTacToe.Network
{
    public enum MessageType
    {
        NewRound = MsgType.Highest + 1,
        MakeTurn,
        EndRound,
    }


    public class StartNewRoundMessage : MessageBase
    {
        public CellSign yourSign;
        public CellSign whoseTurn;
        public int yourScore;
        public int enemyScore;
    }

    public class MakeTurnMessage : MessageBase
    {
        public CellSign whoseTurnWas;
        public int index;
    }

    public class EndRoundMessage : MessageBase
    {
        public CellSign winner;
        public int yourScore;
        public int enemyScore;
    }
}
