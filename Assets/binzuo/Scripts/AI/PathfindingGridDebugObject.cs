using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace binzuo
{
    public class PathfindingGridDebugObject : GridDebugObject
    {
        [SerializeField] private TextMeshPro gCost;
        [SerializeField] private TextMeshPro hCost;
        [SerializeField] private TextMeshPro fCost;

        private PathNode pathNode;

        public override void SetGridObject(object _gridObject)
        {
            base.SetGridObject(_gridObject);
            pathNode = (PathNode)_gridObject;
        }

        protected override void Update() {
            base.Update();
            gCost.text = pathNode.GetGCost().ToString();
            hCost.text = pathNode.GetHCost().ToString();
            fCost.text = pathNode.GetFCost().ToString();
        }
    }

}
