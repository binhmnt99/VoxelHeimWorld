using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TurnBase
{
    public class UnitManager : Singleton<UnitManager>
    {
        private List<Unit> unitList;
        private List<Unit> friendlyUnitList;
        private List<Unit> enemyUnitList;

        protected override void Awake()
        {
            base.Awake();
            unitList = new List<Unit>();
            friendlyUnitList = new List<Unit>();
            enemyUnitList = new List<Unit>();
        }
        void Start()
        {
            Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
            Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        }

        private void Unit_OnAnyUnitDead(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            Pathfinding.Instance.SetIsWalkableGridPosition(unit.GetGridPosition(),true);
            unitList.Remove(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Remove(unit);
                if (enemyUnitList.Count < 1)
                {
                    SceneManager.LoadScene(2);
                }
            }
            else
            {
                friendlyUnitList.Remove(unit);
                if (friendlyUnitList.Count < 1)
                {
                    SceneManager.LoadScene(3);
                }
            }
        }

        private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
        {
            Unit unit = sender as Unit;
            Pathfinding.Instance.SetIsWalkableGridPosition(unit.GetGridPosition(),false);
            unitList.Add(unit);
            if (unit.IsEnemy())
            {
                enemyUnitList.Add(unit);
            }
            else
            {
                friendlyUnitList.Add(unit);
                //Debug.Log(unit.GetGridPosition());
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