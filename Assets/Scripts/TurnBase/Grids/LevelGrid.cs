using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }

        public const float FLOOR_HEIGHT = 3f;
        public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;
        public class OnAnyUnitMovedGridPositionEventArgs : EventArgs
        {
            public Unit unit;
            public GridPosition fromGridPosition;
            public GridPosition toGridPosition;
        }


        [SerializeField] private Transform gridDebugObjectPrefab;

        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private float cellSize;
        [SerializeField] private int floorAmount;

        private List<GridSystemHex<GridObject>> gridSystemList;


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystemList = new List<GridSystemHex<GridObject>>();
            for (int floor = 0; floor < floorAmount; floor++)
            {
                GridSystemHex<GridObject> gridSystem = new GridSystemHex<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT,
                (GridSystemHex<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
                gridSystemList.Add(gridSystem);
            }
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }

        void Start()
        {
            Pathfinding.Instance.SetUp(width, height, cellSize);
        }

        private GridSystemHex<GridObject> GetGridSystem(int floor)
        {
            return gridSystemList[floor];
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            return gridObject.GetUnitList();
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

            OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
            {
                unit = unit,
                fromGridPosition = fromGridPosition,
                toGridPosition = toGridPosition,
            }
);
        }

        public int GetFloor(Vector3 worldPosition)
        {
            if (floorAmount > 1)
            {
                return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
            }
            return 0;
        }

        public GridPosition GetGridPosition(Vector3 worldPosition)
        {
            int floor = GetFloor(worldPosition);
            return GetGridSystem(floor).GetGridPosition(worldPosition);
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);

        public int GetWidth() => GetGridSystem(0).GetWidth();

        public int GetHeight() => GetGridSystem(0).GetHeight();

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            return gridObject.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
            return gridObject.GetUnit();
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