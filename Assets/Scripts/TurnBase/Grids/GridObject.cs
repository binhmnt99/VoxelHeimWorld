using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class GridObject
    {
        private GridSystem gridSystem;
        private GridPosition gridPosition;
        private List<Units> unitList;

        public GridObject(GridSystem gridSystem, GridPosition gridPosition)
        {
            this.gridSystem = gridSystem;
            this.gridPosition = gridPosition;
            unitList = new List<Units>();
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

        public void AddUnit(Units units)
        {
            unitList.Add(units);
        }

        public void RemoveUnit(Units units)
        {
            unitList.Remove(units);
        }

        public List<Units> GetUnitList()
        {
            return unitList;
        }
    }

}
