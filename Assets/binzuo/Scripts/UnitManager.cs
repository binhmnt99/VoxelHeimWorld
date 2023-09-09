using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    using System;
    using System.Collections.Generic;

    public class UnitManager : Singleton<UnitManager>
    {
        private List<Unit> unitList;
        [SerializeField] private List<Unit> friendlyUnitList;
        [SerializeField] private List<Unit> enemyUnitList;

        protected override void Awake()
        {
            base.Awake();
            unitList = new();
            friendlyUnitList = new();
            enemyUnitList = new();
        }

        private void Start()
        {
            Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        }

        private void Unit_OnAnyUnitDead(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            //Pathfinding.Instance.SetWalkableGridPosition(unit.GetGridPosition(), true);
            unitList.Remove(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Remove(unit);
            }
            else
            {
                friendlyUnitList.Remove(unit);
                if (friendlyUnitList.Count > 0)
                {
                    UnitActionSystem.Instance.SetSelectedUnit(friendlyUnitList[0]);
                }
            }
        }

        private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            //Pathfinding.Instance.SetWalkableGridPosition(unit.GetGridPosition(), false);
            unitList.Add(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Add(unit);
            }
            else
            {
                friendlyUnitList.Add(unit);
            }
        }

        public List<Unit> GetUnitList()
        {
            return unitList;
        }
        public List<Unit> GetFriendlyUnitList()
        {
            return friendlyUnitList;
        }
        public List<Unit> GetEnemyUnitList()
        {
            return enemyUnitList;
        }
    }
}
