using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class ShootAction : BaseAction
    {
        private enum ShootState
        {
            AIMING,
            SHOOTING,
            COOLOFF
        }

        public event EventHandler<OnShootEventArgs> OnShoot;

        public class OnShootEventArgs : EventArgs
        {
            public Unit targetUnit;
            public Unit shootingUnit;
        }

        private ShootState state;
        private float stateTimer;
        [SerializeField] private int maxShootDistance = 10;

        [SerializeField] private float aimSpeed = 10f;

        [SerializeField] private LayerMask obstaclesMaskLayer;

        private Unit targetUnit;
        private bool canShootBullet;


        //public List<GameObject> weapon;

        void Update()
        {
            if (!isActive)
            {
                return;
            }

            stateTimer -= Time.deltaTime;
            switch (state)
            {
                case ShootState.AIMING:
                    Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * aimSpeed);
                    break;
                case ShootState.SHOOTING:
                    if (canShootBullet)
                    {
                        Shoot();
                        canShootBullet = false;
                    }
                    break;
                case ShootState.COOLOFF:

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
                case ShootState.AIMING:
                    state = ShootState.SHOOTING;
                    float shootingStateTime = .1f;
                    stateTimer = shootingStateTime;
                    break;
                case ShootState.SHOOTING:
                    state = ShootState.COOLOFF;
                    float coolOffStateTime = .5f;
                    stateTimer = coolOffStateTime;
                    break;
                case ShootState.COOLOFF:
                    ActionComplete();
                    break;
            }
        }

        private void Shoot()
        {
            OnShoot?.Invoke(this, new OnShootEventArgs
            {
                targetUnit = targetUnit,
                shootingUnit = unit
            });
            targetUnit.Damage(4);
        }

        public override string GetActionName()
        {
            return "Shoot";
        }

        public override List<GridPosition> GetValidActionGridPositionList()
        {
            GridPosition unitGridPosition = unit.GetGridPosition();
            return GetValidActionGridPositionList(unitGridPosition);
        }
        public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
        {
            List<GridPosition> validGridPositionList = new List<GridPosition>();
            for (int x = -maxShootDistance; x <= maxShootDistance; x++)
            {
                for (int z = -maxShootDistance; z <= maxShootDistance; z++)
                {
                    GridPosition offsetGridPosition = new GridPosition(x, z);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxShootDistance)
                    {
                        continue;
                    }

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                    {
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    if (targetUnit.IsEnemy() == unit.IsEnemy())
                    {
                        continue;
                    }
                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 shootDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                    float unitShoulderHeight = 1.5f;
                    if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDirection,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesMaskLayer
                    ))
                    {
                        //Block by Obstacles
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);


                }
            }
            return validGridPositionList;
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

            state = ShootState.AIMING;
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
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
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
