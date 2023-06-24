using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class EnemyAI : MonoBehaviour
    {
        private enum State
        {
            WaitingForEnemyTurn,
            TalkingTurn,
            InAction
        }
        private State state;
        private float timer;
        void Awake()
        {
            state = State.WaitingForEnemyTurn;
        }
        // Start is called before the first frame update
        void Start()
        {
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                state = State.TalkingTurn;
                timer = 2f;
            }
        }

        private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
        {
            foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
            {
                if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
                {
                    return true;
                }
            }

            return false;
        }

        private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
        {
            EnemyAIAction bestEnemyAIAction = null;
            BaseAction bestBaseAction = null;

            foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
            {
                if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
                {
                    // Enemy cannot afford this action
                    continue;
                }

                if (bestEnemyAIAction == null)
                {
                    bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                    bestBaseAction = baseAction;
                }
                else
                {
                    EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                    if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                    {
                        bestEnemyAIAction = testEnemyAIAction;
                        bestBaseAction = baseAction;
                    }
                }

            }
            if (bestEnemyAIAction != null)
            {
                if (bestBaseAction != enemyUnit.GetAction<MoveAction>())
                {
                    if (enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
                    {
                        bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (enemyUnit.TrySpendMovePointsToTakeAction(enemyUnit.GetAction<MoveAction>().GetPositionListCount()))
                    {
                        bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            // if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
            // {
            //     bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            //     return true;
            // }
            // else
            // {
            //     return false;
            // }

        }

        private void SetStateTakingTurn()
        {
            timer = .5f;
            state = State.TalkingTurn;
        }

        // Update is called once per frame
        void Update()
        {
            if (TurnSystem.Instance.IsPlayerTurn())
            {
                return;
            }
            switch (state)
            {
                case State.WaitingForEnemyTurn:
                    break;
                case State.TalkingTurn:
                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        if (TryTakeEnemyAIAction(SetStateTakingTurn))
                        {
                            state = State.InAction;
                        }
                        else
                        {
                            TurnSystem.Instance.NextTurn();
                        }
                    }
                    break;
                case State.InAction:
                    break;
            }
        }
    }
}
