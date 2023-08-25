using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace binzuo
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Unit unit;

        private void Start() {

        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.T))
            {
                GridSystemVisual.Instance.HideAllGridVisual();
                GridSystemVisual.Instance.ShowAllGridVisual(unit.GetMoveAction().GetValidActionGridPositionList());
            }
        }
    }

}
