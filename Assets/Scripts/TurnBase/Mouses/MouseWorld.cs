using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBase
{
    public class MouseWorld : MonoBehaviour
    {
        private static MouseWorld instance;

        private Ray ray;
        private RaycastHit hit;
        [SerializeField] private LayerMask layerMask;
        // Start is called before the first frame update
        void Awake()
        {
            instance = this;
        }

        public static Vector3 GetPosition()
        {
            instance.ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(instance.ray, out instance.hit, float.MaxValue, instance.layerMask);
            return instance.hit.point;
        }
    }

}