using UnityEngine;

namespace binzuo
{
    public class MousePosition : Singleton<MousePosition>
    {
        [SerializeField] private LayerMask mouseHitLayerMask;
        private void Update()
        {
            transform.position = MousePosition.GetPoint();
        }

        public static Vector3 GetPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.mouseHitLayerMask);
            return raycastHit.point;
        }
    }

}
