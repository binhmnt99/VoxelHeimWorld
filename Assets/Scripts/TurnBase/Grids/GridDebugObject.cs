using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBase
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] private TextMeshPro textMeshPro;
        private GridObject gridObject;

        public void SetGridObject(GridObject gridObject)
        {
            this.gridObject = gridObject;
        }

        void Update()
        {
            textMeshPro.text = gridObject.ToString();
        }
    }
}

