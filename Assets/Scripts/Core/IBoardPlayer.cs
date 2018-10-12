using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TicTacToe.Core {

    public interface IBoardPlayer {
        int Score { get; set; }
        CellSign Sign { get; set; }
    }
}
