using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Transform gridDebugObjectPrefab;

        private GridSystem gridSystem;

        private void Start() {
            gridSystem = new GridSystem(10,10,2.5f);
            gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
            print(new GridPosition(5,7));
        }

        private void Update() {
            print(gridSystem.GetGridPosition(MousePosition.GetPoint()));
        }
    }

}
