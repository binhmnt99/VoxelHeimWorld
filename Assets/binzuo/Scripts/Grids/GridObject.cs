namespace binzuo
{
    public class GridObject
    {
        private GridSystem gridSystem;
        private GridPosition gridPosition;
        private Unit unit;
        private bool isOccupied;

        public GridObject(GridSystem _gridSystem, GridPosition _gridPosition)
        {
            this.gridSystem = _gridSystem;
            this.gridPosition = _gridPosition;
        }

        public override string ToString() => (isOccupied) ? unit.ToString() : gridPosition.ToString();
        

        public void SetUnit(Unit _unit)
        {
            this.unit = _unit;
            isOccupied = (unit != null) ? true : false;
        }

        public Unit GetUnit() => unit;

        public bool IsOccupiedObject() => isOccupied;
    }
}

