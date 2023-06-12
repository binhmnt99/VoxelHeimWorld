using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }

        public event EventHandler OnAnyUnitMovedGridPosition;
        [SerializeField] private Transform gridDebugObjectPrefab;

        [SerializeField] private int gridRow = 10;
        [SerializeField] private int gridColumn = 10;
        private float cellSize = 2f;
        private GridSystem<GridObject> gridSystem;

        void Awake()
        {
            if (Instance != null)
            {

                Destroy(gameObject);
                return;
            }
            Instance = this;
            gridSystem = new GridSystem<GridObject>(gridRow, gridColumn, cellSize,(GridSystem<GridObject> grid, GridPosition gridPosition) => new GridObject(grid, gridPosition));
            //gridSystem.CreateDebugObject(gridDebugObjectPrefab);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);
            AddUnitAtGridPosition(toGridPosition, unit);

            OnAnyUnitMovedGridPosition?.Invoke(this,EventArgs.Empty);
        }

        //public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            return gridSystem.GetGridPosition(worldPosition);
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return gridSystem.GetWorldPosition(gridPosition);
        }

        public int GetWidth()
        {
            return gridSystem.GetWidth();
        }

        public int GetHeight()
        {
            return gridSystem.GetHeight();
        }

        public bool IsValidGridPosition(GridPosition gridPosition)
        {
            return gridSystem.IsValidGridPosition(gridPosition);
        }

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnit();
        }
    }

}