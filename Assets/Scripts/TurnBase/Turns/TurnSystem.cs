using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class TurnSystem : MonoBehaviour
    {
        public static TurnSystem Instance { get; private set; }

        public event EventHandler OnTurnChanged;
        private int turnNumber = 1;
        public bool isPlayerTurn = true;

        void Awake()
        {
            if (Instance != null)
            {

                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
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
