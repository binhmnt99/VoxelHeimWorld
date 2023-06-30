using System;
using System.Collections.Generic;

namespace TurnBase
{
    public class HexGridObject
    {
        private HexGridSystem<HexGridObject> gridSystem;
        private GridPosition gridPosition;
        private List<Unit> unitList;
        private IInteractable interactable;

        public HexGridObject(HexGridSystem<HexGridObject> gridSystem, GridPosition gridPosition)
        {
            this.gridSystem = gridSystem;
            this.gridPosition = gridPosition;
            unitList = new List<Unit>();
        }

        public override string ToString()
        {
            string unitString = "";
            foreach (var unit in unitList)
            {
                unitString += unit + "\n";
            }
            return gridPosition.ToString() + "\n" + unitString;
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }
        public void AddUnit(Unit unit)
        {
            unitList.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            unitList.Remove(unit);
        }

        public List<Unit> GetUnitList()
        {
            return unitList;
        }

        public bool HasAnyUnit()
        {
            return unitList.Count > 0;
        }

        public Unit GetUnit()
        {
            if (HasAnyUnit())
            {
                return unitList[0];
            }
            else
            {
                return null;
            }
        }

        public IInteractable GetInteractable()
        {
            return interactable;
        }

        public void SetInteractable(IInteractable interactable)
        {
            this.interactable = interactable;
        }
        public void ClearInteractable()
        {
            this.interactable = null;
        }

    }
}

