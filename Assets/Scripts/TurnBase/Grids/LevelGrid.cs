using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class LevelGrid : Singleton<LevelGrid>
    {
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

        private GridSystem<GridObject> gridSystem;


        protected override void Awake()
        {
            base.Awake();
            gridSystem = new GridSystem<GridObject>(width, height, cellSize,
                    (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }

        private void Start()
        {
            Pathfinding.Instance.SetUp(width, height, cellSize);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            Debug.Log("Add unit");
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
            if (gridObject.GetUnit())
            {
                Debug.Log(gridObject.GetUnit());
            }
            Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            Debug.Log("Remove unit");
            Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            if (gridObject.GetUnit())
            {
                Debug.Log(gridObject.GetUnit());
            }
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            Debug.Log("UnitMovedGridPosition");
            RemoveUnitAtGridPosition(fromGridPosition, unit);

            AddUnitAtGridPosition(toGridPosition, unit);

            OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs
            {
                unit = unit,
                fromGridPosition = fromGridPosition,
                toGridPosition = toGridPosition,
            });
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

        public int GetWidth() => gridSystem.GetWidth();

        public int GetHeight() => gridSystem.GetHeight();
        public float GetCellSize() => gridSystem.GetCellSize();

        public GridSystem<GridObject> GetHexGridSystem()
        {
            return gridSystem;
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

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetInteractable();
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.SetInteractable(interactable);
        }

        public void ClearInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.ClearInteractable();
        }
    }

}