using System;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class LevelGrid : Singleton<LevelGrid>
    {
        public event EventHandler OnAnyUnitMoveGridPosition;
        public const float FLOOR_HEIGHT = 5f;
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 2.5f;
        [SerializeField] private int floorAmount = 1;

        private List<GridSystem<GridObject>> gridSystemList;
        protected override void Awake()
        {
            base.Awake();
            gridSystemList = new();
            for (int floor = 0; floor < floorAmount; floor++)
            {
                var gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT, (GridSystem<GridObject> gridSystem, GridPosition gridPosition) => new(gridSystem, gridPosition));
                //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
                gridSystemList.Add(gridSystem);
            }
        }

        private void Start()
        {
            Pathfinding.Instance.Setup(width, height, cellSize, floorAmount);
        }
        private GridSystem<GridObject> GetGridSystem(int floor) => gridSystemList[floor];

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            return gridObject.GetUnit();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);

            AddUnitAtGridPosition(toGridPosition, unit);

            OnAnyUnitMoveGridPosition?.Invoke(this, EventArgs.Empty);
        }
        public int GetFloor(Vector3 worldPosition) => Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);

        public GridPosition GetGridPosition(Vector3 worldPosition) => GetGridSystem(GetFloor(worldPosition)).GetGridPosition(worldPosition);

        public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => (gridPosition.floor < 0 || gridPosition.floor >= floorAmount) ? false : GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);

        public int GetWidth() => GetGridSystem(0).GetWidth();

        public int GetHeight() => GetGridSystem(0).GetHeight();

        public int GetFloorAmount() => floorAmount;

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            return gridObject.IsOccupied();
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            return gridObject.GetInteractable();
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            gridObject.SetInteractable(interactable);
        }
    }
}
