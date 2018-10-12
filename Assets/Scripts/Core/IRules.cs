using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Core
{
    public interface IRules<T> : IEnumerable<T>
        where T : IBoardPlayer
    {
        IBoard Board { get; }
        T FirstPlayer { get; }
        T SecondPLayer { get; }
        CellSign FirstTurn { get; }

        bool AddPlayer(T player);
        T OtherPlayer(T self);

        bool PlayersAll();
        void RemoveAllPlayers();

        void NewGame();
        void NewRound();
    }
}
