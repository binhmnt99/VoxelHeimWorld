using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class ShootAction : BaseAction
    {
        public static event EventHandler<OnShootEventArgs> OnAnyShoot;

        public event EventHandler<OnShootEventArgs> OnShoot;

        public class OnShootEventArgs : EventArgs
        {
            public Unit targetUnit;
            public Unit shootingUnit;
        }

        private enum State
        {
            Aiming,
            Shooting,
            Cooloff,
        }

        [SerializeField] private LayerMask obstaclesLayerMask;
        [SerializeField] private LayerMask unitLayerMask;
        [SerializeField] private int maxShootDistance = 7;
        [SerializeField] private float aimSpeed = 15f;
        private float stateTimer;
        private Unit targetUnit;
        private bool canShootBullet;

        private State state;
        void Update()
        {
            if (!isActive)
            {
                return;
            }
            if (targetUnit == null)
            {
                state = State.Cooloff;
            }
            stateTimer -= Time.deltaTime;
            switch (state)
            {
                case State.Aiming:
                    Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * aimSpeed);
                    break;
                case State.Shooting:
                    if (canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case State.Cooloff:

                    break;
            }
            if (stateTimer <= 0f)
            {
                NextState();
            }
        }

        private void NextState()
        {
            switch (state)
            {
                case State.Aiming:
                    state = State.Shooting;
                    float shootingStateTime = .1f;
                    stateTimer = shootingStateTime;
                    break;
                case State.Shooting:
                    state = State.Cooloff;
                    float coolOffStateTime = .5f;
                    stateTimer = coolOffStateTime;
                    break;
                case State.Cooloff:
                    ActionComplete();
                    break;
            }
        }

        private void Shoot()
        {
            OnAnyShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit
            });

            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit
            });

            targetUnit.Damage(4);
        }


        // private void UnitAnimator_OnShootAnim(object sender, EventArgs e)
        // {
        //     OnAnyShoot?.Invoke(this, new OnShootEventArgs
        //     {
        //         targetUnit = targetUnit,
        //         shootingUnit = unit
        //     });
        //     targetUnit.Damage(2);
        // }

        public override string GetActionName()
        {
            return "Shoot";
        }

        public override int GetActionPointsCost()
        {
            return 2;
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            GridPosition unitGridPosition = unit.GetGridPosition();
            return GetValidActionGridPositionList(unitGridPosition);

        }
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        List<GridPosition> blockGridList = new List<GridPosition>();

        public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
        {
            validGridPositionList.Clear();
            for (int x = -maxShootDistance; x <= maxShootDistance; x++)
            {
                for (int z = -maxShootDistance; z <= maxShootDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                    if (!HexLevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    if (!HexLevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    Unit targetUnit = HexLevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                    if (unit.GetGridPosition() == testGridPosition)
                    {
                        continue;
                    }

                    Vector3 unitWorldPosition = HexLevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 shootDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    float unitShoulderHeight = 1.1f;
                    if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDirection,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask
                    ))
                    {
                        //Block by Obstacles
                        continue;
                    }

                    //Vector3 unitWorldPosition = HexLevelGrid.Instance.GetHexGridSystem().GetWorldPosition(unitGridPosition);
                    Vector3 testWorldPosition = HexLevelGrid.Instance.GetHexGridSystem().GetWorldPosition(testGridPosition);
                    float distance = Vector3.Distance(unitWorldPosition, testWorldPosition);
                    if (distance >= maxShootDistance)
                    {
                        continue;
                    }

                    //Vector3 testWorldPosition = HexLevelGrid.Instance.GetWorldPosition(testGridPosition);
                    Vector3 testDirection = (testWorldPosition - unit.GetWorldPosition()).normalized;
                    distance = Vector3.Distance(unit.GetWorldPosition(), testWorldPosition);
                    RaycastHit[] hits = Physics.RaycastAll(unit.GetWorldPosition(), testDirection, distance, unitLayerMask);
                    foreach (var hit in hits)
                    {
                        GridPosition testGrid = HexLevelGrid.Instance.GetGridPosition(hit.collider.transform.position);
                        //Debug.Log("When " + validGrid + " and distance " + distance + " Hit Grit " + testGrid);

                        if (testGrid != testGridPosition && !blockGridList.Contains(testGridPosition))
                        {
                            //Debug.Log("Remove blocked Grid " + validGrid);
                            blockGridList.Add(testGridPosition);
                        }
                    }
                    validGridPositionList.Add(testGridPosition);
                }
            }
            foreach (GridPosition gridPosition in blockGridList)
            {
                //Debug.Log("Remove blocked Grid " + gridPosition);
                validGridPositionList.Remove(gridPosition);
            }
            blockGridList.Clear();
            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            targetUnit = HexLevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            //Debug.Log(gridPosition);

            state = State.Aiming;
            float aimingStateTime = 1f;
            stateTimer = aimingStateTime;

            canShootBullet = true;
            ActionStart(onActionComplete);
        }

        public Unit GetTargetUnit()
        {
            return targetUnit;
        }

        public int GetShootDistance()
        {
            return maxShootDistance;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            Unit targetUnit = HexLevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f)
            };
        }

        public int GetTargetCountAtPosition(GridPosition gridPosition)
        {
            return GetValidActionGridPositionList(gridPosition).Count;
        }
    }
}
