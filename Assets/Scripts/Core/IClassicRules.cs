using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Core
{
    public class ClassicRules<T> : IRules<T>
         where T : IBoardPlayer
    {
        private T firstPlayer;
        private T secondPlayer;

        private IBoard board;
        CellSign firstTurn;

        public ClassicRules(IBoard board)
        {
            this.board = board;

            board.OnGameEnd += GameEnd;
        }

        private void GameEnd(CellSign winner)
        {
            if (firstPlayer.Sign == winner) firstPlayer.Score++;
            else if (secondPlayer.Sign == winner) secondPlayer.Score++;
        }

        public IBoard Board
        {
            get { return board; }
        }

        public T FirstPlayer
        {
            get { return firstPlayer; }
        }

        public T SecondPLayer
        {
            get { return secondPlayer; }
        }

        public CellSign FirstTurn
        {
            get { return firstTurn; }
        }

        public bool AddPlayer(T player)
        {
            if (firstPlayer == null)
            {
                firstPlayer = player;
                return true;
            }
            if (secondPlayer == null)
            {
                secondPlayer = player;
                return true;
            }

            return false;
        }

        public T OtherPlayer(T self)
        {
            if (firstPlayer.Equals(self)) return secondPlayer;
            if (secondPlayer.Equals(self)) return firstPlayer;
            return default(T);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if(firstPlayer != null)
                yield return firstPlayer;
            if(secondPlayer != null)
                yield return secondPlayer;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (firstPlayer != null)
                yield return firstPlayer;
            if (secondPlayer != null)
                yield return secondPlayer;
        }

        public void NewGame()
        {
            firstPlayer.Score = secondPlayer.Score = 0;
            NewRound();
        }

        public void NewRound()
        {
            board.Clear();
            firstTurn = UnityEngine.Random.value > 0.5f ? CellSign.Cross : CellSign.Nought;

            bool rand = UnityEngine.Random.value > 0.5f;
            firstPlayer.Sign = rand ? CellSign.Cross : CellSign.Nought;
            secondPlayer.Sign = !rand ? CellSign.Cross : CellSign.Nought;
        }

        public bool PlayersAll()
        {
            return firstPlayer != null && secondPlayer != null;
        }

        public void RemoveAllPlayers()
        {
            firstPlayer = secondPlayer = default(T);
        }
    }
}
