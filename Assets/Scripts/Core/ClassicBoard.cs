using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Core
{
    public class ClassicBoard : IBoard
    {
        public event Action<CellSign> OnGameEnd = delegate { };

        private const int columns = 3;
        private const int rows = 3;
        private CellSign winner = CellSign.None;

        private CellSign[] cells = new CellSign[columns * rows];

        private readonly int[,] winCells = {
            //horizontal
            {0, 1, 2 },
            {3, 4, 5 },
            {6, 7, 8 },
            //vertical
            {0, 3, 6 },
            {1, 4, 7 },
            {2, 5, 8 },
            //diagonals
            {0, 4, 8 },
            {2, 4, 6 }
        };

        public ClassicBoard() { }

        public int Columns
        {
            get { return columns; }
        }

        public int Rows
        {
            get { return rows; }
        }

        public CellSign Winner
        {
            get { return winner; }
        }

        public bool IsFullMarked
        {
            get
            {
                foreach (var cell in cells)
                    if (cell == CellSign.None) return false;

                return true;
            }
        }

        public void MakeTurn(int index, CellSign sign)
        {
            cells[index] = sign;
            CheckWinner();

            if(Winner != CellSign.None || IsFullMarked)
            {
                OnGameEnd(Winner);
            }
        }

        public void Clear()
        {
            for (int i = 0; i < cells.Length; ++i)
            {
                cells[i] = CellSign.None;
            }

            winner = CellSign.None;
        }

        void CheckWinner()
        {
            for(int i = 0; i < winCells.GetLength(0); ++i)
            {
                if(cells[winCells[i, 0]] == cells[winCells[i, 1]] &&
                   cells[winCells[i, 1]] == cells[winCells[i, 2]])
                {
                    if (cells[winCells[i, 0]] == CellSign.None) continue;
                    
                    winner = cells[winCells[i, 0]];
                    return;
                }
            }
        }
    }
}
