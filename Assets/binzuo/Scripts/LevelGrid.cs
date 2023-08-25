using UnityEngine;

namespace binzuo
{
    public class LevelGrid : Singleton<LevelGrid>
    {
        [SerializeField] private Transform gridDebugObjectPrefab;
        [SerializeField] private int width = 10;
        [SerializeField] private int height = 10;
        [SerializeField] private float cellSize = 2.5f;

        private GridSystem gridSystem;
        protected override void Awake()
        {
            base.Awake();
            gridSystem = new GridSystem(width, height, cellSize);
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
        }

        public void SetUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.SetUnit(unit);
        }

        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.GetUnit();
        }

        public void ClearUnitAtGridPosition(GridPosition gridPosition)
        { 
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.SetUnit(null);
        }

        public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            ClearUnitAtGridPosition(fromGridPosition);
            SetUnitAtGridPosition(toGridPosition, unit);
        }

        public GridPosition GetGridPosition(Vector3 worldPosition) => gridSystem.GetGridPosition(worldPosition);

        public Vector3 GetWorldPosition(GridPosition gridPosition) => gridSystem.GetWorldPosition(gridPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

        public int GetWidth() => gridSystem.GetWidth();

        public int GetHeight() => gridSystem.GetHeight();

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            return gridObject.IsOccupiedObject();
        }
    }
}
