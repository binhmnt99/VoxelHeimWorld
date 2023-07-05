using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class HexLevelGrid : MonoBehaviour
    {
        public static HexLevelGrid Instance { get; private set; }


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

        private HexGridSystem<HexGridObject> gridSystem;


        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("There's more than one HexLevelGrid! " + transform + " - " + Instance);
                Destroy(gameObject);
                return;
            }
            Instance = this;

            gridSystem = new HexGridSystem<HexGridObject>(width, height, cellSize,
                    (HexGridSystem<HexGridObject> g, GridPosition gridPosition) => new HexGridObject(g, gridPosition));
            //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }

        private void Start()
        {
            HexPathfinding.Instance.SetUp(width, height, cellSize);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
            HexPathfinding.Instance.SetIsWalkableGridPosition(gridPosition,false);
        }

        public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnitList();
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            HexPathfinding.Instance.SetIsWalkableGridPosition(gridPosition,true);
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
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
            });
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

        public int GetWidth() => gridSystem.GetWidth();

        public int GetHeight() => gridSystem.GetHeight();

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.HasAnyUnit();
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnit();
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetInteractable();
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.SetInteractable(interactable);
        }

        public void ClearInteractableAtGridPosition(GridPosition gridPosition)
        {
            HexGridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.ClearInteractable();
        }
    }

}