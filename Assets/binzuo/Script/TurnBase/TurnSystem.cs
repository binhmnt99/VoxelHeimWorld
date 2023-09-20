using System;
using UnityEngine;

namespace binzuo
{
    public class TurnSystem : Singleton<TurnSystem>
    {
        [SerializeField] private int turnNumber = 1;
        [SerializeField] private bool isPlayerTurn = true;

        public event EventHandler OnTurnChanged;

        public void NextTurn()
        {
            turnNumber++;
            isPlayerTurn = !isPlayerTurn;
            OnTurnChanged?.Invoke(this,EventArgs.Empty);
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

