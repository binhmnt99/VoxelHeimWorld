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

//         [SerializeField]private State state;
//         private float timer;

//         private void Awake()
//         {
//             state = State.WaitingForEnemyTurn;
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
//                         if (TryTakeEnemyAIAction(SetStateTakingTurn))
//                         {
//                             state = State.Busy;
//                         }
//                         else
//                         {
//                             // No more enemies have actions they can take, end enemy turn
//                             TurnSystem.Instance.NextTurn();
//                         }
//                     }
//                     break;
//                 case State.Busy:
//                     break;
//             }
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

//         private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
//         {
//             foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
//             {
//                 if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete))
//                 {
//                     return true;
//                 }
//             }

//             return false;
//         }

//         private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
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
//                 bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
//                 return true;
//             }
//             else
//             {
//                 return false;
//             }
//         }
//     }
// }
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class EnemyAI : MonoBehaviour
    {
        private enum EnemyState
        {
            Idle,
            Chase,
            Attack,
            Flee,
            Aggro
        }

        [SerializeField] private EnemyState state;
        [SerializeField] private float healthThreshold = .3f;
        private Unit enemy;
        [SerializeField] private Unit target;

        private bool isAggro;
        private bool isFlee;

        private List<Unit> targetList;

        private void Awake()
        {
            isAggro = false;
            isFlee = false;
            state = EnemyState.Idle;
            enemy = this.GetComponent<Unit>();
        }
        private void Start()
        {
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }
        private void Update()
        {
            // return if this is not enemy turn
            if (TurnSystem.Instance.IsPlayerTurn())
            {
                targetList = UnitManager.Instance.GetFriendlyUnitList();
                //If onDamaged, change current state to aggro
                if (!isAggro)
                {
                    CheckAggro();
                }
                if (!isFlee)
                {
                    //Health Below 30% => Flee
                    CheckHealthBelowThreshold();
                }
                return;
            }

            switch (state)
            {
                case EnemyState.Idle:
                    //If onDamaged below 30% hp, change current state to flee
                    //If onChasingRange, change current state to chase
                    EnemyIdleState();
                    break;
                case EnemyState.Chase:
                    //If onAttackRange, change current state to attack
                    //If outOfAttackRange, continue state chase
                    EnemyChaseState();
                    break;
                case EnemyState.Attack:
                    //If target die, change current state to Idle
                    EnemyAttackState();
                    break;
                case EnemyState.Flee:
                    //If notOnDamaged , change current state to aggro
                    break;
                case EnemyState.Aggro:
                    //If onAttackRange, change current state to attack
                    //If outOfAttackRange, change current state to chase
                    break;
            }
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            if (!TurnSystem.Instance.IsPlayerTurn())
            {
                if (state == EnemyState.Aggro)
                {
                    return;
                }
                state = EnemyState.Idle;
                isAggro = false;
            }
        }

        private void CheckHealthBelowThreshold()
        {
            if (enemy.GetHealthNormalized() <= healthThreshold)
            {
                state = EnemyState.Flee;
                isFlee = true;
            }
        }

        private Unit FindClosestUnit(List<Unit> unitList, Vector3 enemy)
        {
            float closestDistance = float.MaxValue;
            Unit closestUnit = unitList[0];

            foreach (Unit unit in unitList)
            {
                float distance = Vector3.Distance(unit.GetWorldPosition(), enemy);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestUnit = unit;
                }
            }

            return closestUnit;
        }

        private bool IsValidActionGridPosition(List<GridPosition> gridPositionList, GridPosition gridPosition)
        {
            List<GridPosition> validGridPositionList = gridPositionList;
            return validGridPositionList.Contains(gridPosition);
        }

        private void CheckChasing()
        {
            MoveAction moveAction = enemy.GetAction<MoveAction>();
            target = FindClosestUnit(targetList, enemy.GetWorldPosition());
            if (!IsValidActionGridPosition(moveAction.GetValidEnemyActionGridPositionList(enemy.GetGridPosition()), target.GetGridPosition()))
            {
                IdleAction idleAction = enemy.GetAction<IdleAction>();
                if (idleAction.IsValidActionGridPosition(target.GetGridPosition()))
                {
                    return;
                }
                if (!enemy.TrySpendActionPointsToTakeAction(idleAction))
                {
                    // Enemy cannot afford this action
                    return;
                }
                return;
            }
            else
            {
                state = EnemyState.Chase;
            }
        }

        private void CheckAggro()
        {
            foreach (Unit targetUnit in targetList)
            {
                ShootAction shootAction = targetUnit.GetAction<ShootAction>();
                shootAction.OnShoot += ShootAction_OnShoot;
                shootAction.OnShoot -= ShootAction_OnShoot;
            }
        }

        private void ShootAction_OnShoot(object sender, ShootAction.OnShootEventArgs e)
        {
            if (e.targetUnit == enemy)
            {
                target = e.shootingUnit;
                state = EnemyState.Aggro;
                isAggro = true;
            }
        }

        private void EnemyIdleState()
        {
            //onChasingRange, change current state to chase
            CheckChasing();
        }

        private void OnMoveActionComplete()
        {
            state = EnemyState.Chase;
        }

        private T GetRandomValue<T>(List<T> list)
        {
            if (list.Count == 0)
            {
                Debug.LogWarning("The list is empty!");
                return default(T);
            }

            int randomIndex = UnityEngine.Random.Range(0, list.Count);
            T randomValue = list[randomIndex];
            return randomValue;
        }

        private void EnemyTakeMoveAction()
        {
            MoveAction moveAction = enemy.GetAction<MoveAction>();
            List<GridPosition> validGridList = moveAction.GetValidActionGridPositionList();
            GridPosition nextGridPosition = GetRandomValue(validGridList);
            if (!moveAction.IsValidActionGridPosition(nextGridPosition))
            {
                return;
            }
            if (!enemy.TrySpendActionPointsToTakeAction(moveAction))
            {
                // Enemy cannot afford this action
                return;
            }
            moveAction.TakeAction(nextGridPosition, OnMoveActionComplete);
        }
        private void EnemyChaseState()
        {
            //If onAttackRange, change current state to attack
            //If outOfAttackRange, continue state chase
            ShootAction shootAction = enemy.GetAction<ShootAction>();
            target = FindClosestUnit(targetList, enemy.GetWorldPosition());
            if (shootAction.IsValidActionGridPosition(target.GetGridPosition()))
            {
                state = EnemyState.Attack;
            }
            else
            {
                EnemyTakeMoveAction();
            }
        }

        private void OnShootComplete()
        {
            state = EnemyState.Chase;
        }
        private void EnemyAttackState()
        {
            ShootAction shootAction = enemy.GetAction<ShootAction>();
            List<GridPosition> validGridList = shootAction.GetValidActionGridPositionList();
            GridPosition nextGridPosition = GetRandomValue(validGridList);
            if (!shootAction.IsValidActionGridPosition(target.GetGridPosition()))
            {
                return;
            }
            if (!enemy.TrySpendActionPointsToTakeAction(shootAction))
            {
                // Enemy cannot afford this action
                return;
            }
            shootAction.TakeAction(target.GetGridPosition(), OnShootComplete);
        }
    }
}