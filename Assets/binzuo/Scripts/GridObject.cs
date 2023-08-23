using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class GridObject
    {
        private GridSystem gridSystem;
        private GridPosition gridPosition;

        public GridObject(GridSystem _gridSystem, GridPosition _gridPosition)
        {
            this.gridSystem = _gridSystem;
            this.gridPosition = _gridPosition;
        }
    }
}

