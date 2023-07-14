using UnityEngine;

public class MousePosition : Singleton<MousePosition>
{
    [SerializeField] private LayerMask laneLayerMask;

    public Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Instance.laneLayerMask);
        return raycastHit.point;
    }
}