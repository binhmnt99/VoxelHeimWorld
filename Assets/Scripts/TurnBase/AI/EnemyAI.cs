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
            TakingTurn,
            Busy,
        }

        [SerializeField]private State state;
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
                        if (TryTakeEnemyAIAction(SetStateTakingTurn))
                        {
                            state = State.Busy;
                        }
                        else
                        {
                            // No more enemies have actions they can take, end enemy turn
                            TurnSystem.Instance.NextTurn();
                        }
                    }
                    break;
                case State.Busy:
                    break;
            }
        }

        private void SetStateTakingTurn()
        {
            timer = 0.5f;
            state = State.TakingTurn;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                state = State.TakingTurn;
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

            if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
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
}

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// namespace TurnBase
// {
//     public class EnemyAI : MonoBehaviour
//     {
//         private enum State
//         {
//             WaitingForEnemyTurn,
//             TakingTurn,
//             Busy,
//         }

//         [SerializeField] private State state;
//         private float timer;
//         private List<Coroutine> enemyTurnCoroutines;

//         private void Awake()
//         {
//             state = State.WaitingForEnemyTurn;
//             enemyTurnCoroutines = new List<Coroutine>();
//         }

//         private void Start()
//         {
//             TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
//         }

//         private void Update()
//         {
//             if (TurnSystem.Instance.IsPlayerTurn())
//             {
//                 return;
//             }

//             switch (state)
//             {
//                 case State.WaitingForEnemyTurn:
//                     break;
//                 case State.TakingTurn:
//                     timer -= Time.deltaTime;
//                     if (timer <= 0f)
//                     {
//                         StartCoroutine(TakeEnemyTurn(SetStateTakingTurn));
//                     }
//                     // else
//                     // {
//                     //     // No more enemies have actions they can take, end enemy turn
//                     //     TurnSystem.Instance.NextTurn();
//                     // }
//                     break;
//                 case State.Busy:
//                     break;
//             }
//         }

//         private IEnumerator TakeEnemyTurn(Action onEnemyAIActionComplete)
//         {
//             List<Unit> enemyUnits = UnitManager.Instance.GetEnemyUnitList();
//             enemyTurnCoroutines.Clear();

//             foreach (Unit enemyUnit in enemyUnits)
//             {
//                 Coroutine enemyTurnCoroutine = StartCoroutine(TakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete));
//                 enemyTurnCoroutines.Add(enemyTurnCoroutine);
//             }

//             // Wait for all enemy turns to complete
//             yield return new WaitForSeconds(0.1f); // Adjust the delay if necessary

//             while (enemyTurnCoroutines.Count > 0)
//             {
//                 for (int i = enemyTurnCoroutines.Count - 1; i >= 0; i--)
//                 {
//                     if (enemyTurnCoroutines[i] == null)
//                     {
//                         enemyTurnCoroutines.RemoveAt(i);
//                     }
//                 }
//                 yield return null;
//             }

//             // All enemy turns have completed
//             onEnemyAIActionComplete?.Invoke();
//         }

//         private void SetStateTakingTurn()
//         {
//             timer = 0.5f;
//             state = State.TakingTurn;
//         }

//         private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
//         {
//             if (!TurnSystem.Instance.IsPlayerTurn())
//             {
//                 state = State.TakingTurn;
//                 timer = 2f;
//             }
//         }

//         private IEnumerator TakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
//         {
//             EnemyAIAction bestEnemyAIAction = null;
//             BaseAction bestBaseAction = null;

//             foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
//             {
//                 if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
//                 {
//                     // Enemy cannot afford this action
//                     continue;
//                 }

//                 if (bestEnemyAIAction == null)
//                 {
//                     bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
//                     bestBaseAction = baseAction;
//                 }
//                 else
//                 {
//                     EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
//                     if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
//                     {
//                         bestEnemyAIAction = testEnemyAIAction;
//                         bestBaseAction = baseAction;
//                     }
//                 }
//             }

//             if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
//             {
//                 yield return StartCoroutine(TakeActionCoroutine(bestBaseAction, bestEnemyAIAction.gridPosition, onEnemyAIActionComplete));
//             }

//             onEnemyAIActionComplete?.Invoke();
//             state = State.WaitingForEnemyTurn;
//             TurnSystem.Instance.NextTurn();
//         }

//         private IEnumerator TakeActionCoroutine(BaseAction baseAction, GridPosition gridPosition, Action onComplete)
//         {
//             bool actionCompleted = false;

//             // Define a separate callback to mark completion
//             void ActionCompleteCallback()
//             {
//                 actionCompleted = true;
//                 onComplete?.Invoke(); // Invoke the provided onComplete callback
//             }

//             // Call the action and provide the separate callback
//             baseAction.TakeAction(gridPosition, ActionCompleteCallback);

//             // Wait until the action is completed
//             while (!actionCompleted)
//             {
//                 yield return null;
//             }
//         }
//     }
// }
