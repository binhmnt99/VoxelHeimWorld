using System;
using System.Collections;
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

        private IEnumerator IdleActionHandle()
        {
            IdleAction idleAction = enemy.GetAction<IdleAction>();
            if (!enemy.TrySpendActionPointsToTakeAction(idleAction))
            {
                // Enemy cannot afford this action
                yield break;
            }
            yield return null;
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
            MoveAction moveAction = enemy.GetAction<MoveAction>();
            target = FindClosestUnit(targetList, enemy.GetWorldPosition());
            if (!IsValidActionGridPosition(moveAction.GetValidEnemyActionGridPositionList(enemy.GetGridPosition()), target.GetGridPosition()))
            {
                StartCoroutine(IdleActionHandle());
            }
            else
            {
                state = EnemyState.Chase;
            }
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
                StartCoroutine(MoveActionHandle());
            }
        }

        private IEnumerator MoveActionHandle()
        {
            MoveAction moveAction = enemy.GetAction<MoveAction>();
            List<GridPosition> validGridList = moveAction.GetValidActionGridPositionList();
            GridPosition nextGridPosition = GetRandomValue(validGridList);
            if (!moveAction.IsValidActionGridPosition(nextGridPosition))
            {
                yield break;
            }
            if (!enemy.TrySpendActionPointsToTakeAction(moveAction))
            {
                // Enemy cannot afford this action
                yield break;
            }
            moveAction.TakeAction(nextGridPosition, OnMoveActionComplete);
            yield return null;
        }

        private void OnShootComplete()
        {
            state = EnemyState.Chase;
        }
        private void EnemyAttackState()
        {
            StartCoroutine(AttackActionHandle());
        }

        private IEnumerator AttackActionHandle()
        {
            ShootAction shootAction = enemy.GetAction<ShootAction>();
            List<GridPosition> validGridList = shootAction.GetValidActionGridPositionList();
            GridPosition nextGridPosition = GetRandomValue(validGridList);
            if (!shootAction.IsValidActionGridPosition(target.GetGridPosition()))
            {
                yield break;
            }
            if (!enemy.TrySpendActionPointsToTakeAction(shootAction))
            {
                // Enemy cannot afford this action
                yield break;
            }
            shootAction.TakeAction(target.GetGridPosition(), OnShootComplete);
            yield return null;
        }
    }
}