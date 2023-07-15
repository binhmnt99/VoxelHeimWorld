using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : Singleton<UnitManager>
{
    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    [SerializeField] private int friendlyCount = 0;
    [SerializeField] private int enemyCount = 0;

    protected override void Awake()
    {
        base.Awake();
        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        unitList.Remove(unit);
        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
            enemyCount--;
            if (enemyUnitList.Count < 1)
            {
                SceneManager.LoadScene(2);
            }
        }
        else
        {
            friendlyUnitList.Remove(unit);
            enemyCount--;
            if (friendlyUnitList.Count < 1)
            {
                SceneManager.LoadScene(3);
            }
        }
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        unitList.Add(unit);
        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
            enemyCount++;
        }
        else
        {
            friendlyUnitList.Add(unit);
            friendlyCount++;
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
