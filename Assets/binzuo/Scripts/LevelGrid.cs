using UnityEngine;

namespace binzuo
{
    public class LevelGrid : Singleton<LevelGrid>
    {
        [SerializeField] private Transform gridDebugObjectPrefab;

        private GridSystem gridSystem;
        protected override void Awake()
        {
            base.Awake();
            gridSystem = new GridSystem(10, 10, 2.5f);
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


    }
}
