using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class TurnSystem : Singleton<TurnSystem>
    {
        public event EventHandler OnTurnChanged;
        private int turnNumber = 1;
        public bool isPlayerTurn = true;

        public void NextTurn()
        {
            turnNumber++;
            isPlayerTurn = !isPlayerTurn;
            OnTurnChanged?.Invoke(this, EventArgs.Empty);
        }

        public int GetTurnNumber()
        {
            return turnNumber;
        }

        public bool IsPlayerTurn()
        {
            return isPlayerTurn;
        }
    }
}
