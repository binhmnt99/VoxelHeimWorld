using System.Collections.Generic;
using System.ComponentModel;

namespace binzuo
{
    public class GridObject
    {
        private GridSystem<GridObject> gridSystem;
        private GridPosition gridPosition;
        private List<Unit> unitList;
        private IInteractable interactable;

        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            this.gridSystem = gridSystem;
            this.gridPosition = gridPosition;
            unitList = new List<Unit>();
        }

        public override string ToString()
        {
            string unitString = "";
            foreach (Unit unit in unitList)
            {
                unitString += unit + "\n";
            }

            return gridPosition.ToString() + "\n" + unitString;
        }

        public void AddUnit(Unit unit) => unitList.Add(unit);

        public void RemoveUnit(Unit unit) => unitList.Remove(unit);
        
        public List<Unit> GetUnitList() => unitList;

        public Unit GetUnit()
        {
            if (IsOccupied())
            {
                return unitList[0];
            }
            return null;
        }

        public bool IsOccupied() => unitList.Count > 0;

        public IInteractable GetInteractable()
        {
            return interactable;
        }

        public void SetInteractable(IInteractable interactable)
        {
            this.interactable = interactable;
        }

    }
}

