using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }
        [SerializeField] private Transform gridDebugObjectPrefab;

        private GridSystem gridSystem;

        void Awake()
        {
            if (Instance != null)
            {

                Destroy(gameObject);
                return;
            }
            Instance = this;
            gridSystem = new GridSystem(10, 10, 2);
            gridSystem.CreateDebugObject(gridDebugObjectPrefab);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Units units)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(units);
        }

        public List<Units> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Units units)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.RemoveUnit(units);
        }

        public void UnitMoveGridPosition(Units units, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, units);
            AddUnitAtGridPosition(toGridPosition, units);
        }

        //public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return gridSystem.GetGridPosition(worldPosition);
        }
    }

}