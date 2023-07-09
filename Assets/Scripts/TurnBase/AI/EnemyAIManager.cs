using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class EnemyAIManager : MonoBehaviour
    {
        private enum State
        {
            WaitingForEnemyTurn,
            TakingTurn,
        }

        [SerializeField] private State state;
        private float timer;

        private void Awake()
        {
            state = State.WaitingForEnemyTurn;
        }

        private void Start()
        {
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        private void Update()
        {
            if (TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }

            switch (state)
            {
                case State.WaitingForEnemyTurn:
                    break;
                case State.TakingTurn:
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        if (TryTakeEnemyAIAction())
                        {
                            TurnSystem.Instance.NextTurn();
                        }
                    }
                    break;
            }
        }

        private bool TryTakeEnemyAIAction()
        {
            foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
            {
                if (enemyUnit.GetActionPoints() != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                state = State.TakingTurn;
                timer = 2f;
            }
        }
    }
}
