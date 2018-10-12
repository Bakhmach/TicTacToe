using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Core
{
    public enum CellSign { None, Cross, Nought }

    public interface IBoard
    {
        event System.Action<CellSign> OnGameEnd;

        int Columns { get; }
        int Rows { get; }
        CellSign Winner { get; }
        bool IsFullMarked { get; }

        void MakeTurn(int index, CellSign sign);
        void Clear();
    }
}
