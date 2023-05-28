using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckBlock : MonoBehaviour
{
    private RaycastHit hit;
    [SerializeField] float length;
    [SerializeField] private LayerMask layerMask;
    public Vector3Int blockPos {get; private set;}
    // Start is called before the first frame update
    void Start()
    {

    }

    private Vector3Int GetBlockPos(RaycastHit hit)
    {
        Vector3 pos = new Vector3(
             GetBlockPositionIn(hit.point.x, hit.normal.x),
             GetBlockPositionIn(hit.point.y, hit.normal.y),
             GetBlockPositionIn(hit.point.z, hit.normal.z)
             );

        return Vector3Int.RoundToInt(pos);
    }

    private float GetBlockPositionIn(float pos, float normal)
    {
        if (Mathf.Abs(pos % 1) == 0.5f)
        {
            pos -= (normal / 2);
        }
        return (float)pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, length, layerMask))
        {
            blockPos = GetBlockPos(hit);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * length);
    }
}
