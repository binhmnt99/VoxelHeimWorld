using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class PathfindingUpdate : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestroyed;
        }

        private void DestructibleCrate_OnAnyDestroyed(object sender, EventArgs e)
        {
            DestructibleCrate destructibleCrate = sender as DestructibleCrate;
            Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GetGridPosition(),true);
        }

        // Update is called once per frame
    }

}