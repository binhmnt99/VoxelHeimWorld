using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class Testing : MonoBehaviour
    {
        [SerializeField] private Unit unit;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log(gridSystem.GetGridPosition(MouseWorld.GetPosition()));
            if (Input.GetKeyDown(KeyCode.T))
            {
                //GridSystemVisual.Instance.HideAllGridPosition();
                //GridSystemVisual.Instance.ShowGridPositionList(unit.GetMoveAction().GetValidActionGridPositionsList());
            }
        }
    }

}
