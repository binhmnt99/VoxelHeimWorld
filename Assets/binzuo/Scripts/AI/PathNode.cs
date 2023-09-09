using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class PathNode
    {
        private GridPosition gridPosition;
        private int gCost;
        private int hCost;
        private int fCost;
        private PathNode cameFromPathNode;

        public PathNode(GridPosition gridPosition)
        {
            this.gridPosition = gridPosition;
        }

        public override string ToString()
        {
            return gridPosition.ToString();
        }

        public int GetGCost()
        {
            return gCost;
        }

        public int GetHCost()
        {
            return hCost;
        }

        public int GetFCost()
        {
            return fCost;
        }

        public void SetGCost(int gCost)
        {
            this.gCost = gCost;
        }

        public void SetHCost(int hCost)
        {
            this.hCost = hCost;
        }

        public void CalculateFCost()
        {
            fCost = gCost + hCost;
        }

        public void ResetCameFormPathNode()
        {
            cameFromPathNode = null;
        }

        public void SetCameFormPathNode(PathNode pathNode)
        {
            cameFromPathNode = pathNode;
        }

        public PathNode GetCameFormPathNode()
        {
            return cameFromPathNode;
        }

        public GridPosition GetGridPosition()
        {
            return gridPosition;
        }
    }
}

