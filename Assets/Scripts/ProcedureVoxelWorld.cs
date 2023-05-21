using UnityEngine;
using Pathfinding;

namespace Voxel
{
    public class ProcedureVoxelWorld : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            AstarPath.active.Scan();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

