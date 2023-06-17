using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class MouseWorld : MonoBehaviour
    {
        private static MouseWorld Instance;

        private Ray ray;
        private RaycastHit hit;
        [SerializeField] private LayerMask layerMask;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }

        public static Vector3 GetPosition()
        {
            Instance.ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            Physics.Raycast(Instance.ray, out Instance.hit, float.MaxValue, Instance.layerMask);
            return Instance.hit.point;
        }
    }

}