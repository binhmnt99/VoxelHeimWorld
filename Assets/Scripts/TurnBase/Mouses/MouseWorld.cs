using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class MouseWorld : MonoBehaviour
    {
        private static MouseWorld Instance;
        [SerializeField] private LayerMask layerMask;
        // Start is called before the first frame update
        void Awake()
        {
            Instance = this;
        }
        void Update()
        {
            transform.localPosition = GetPosition();
        }
        public static Vector3 GetPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.layerMask);
            return raycastHit.point;

        }
    }

}