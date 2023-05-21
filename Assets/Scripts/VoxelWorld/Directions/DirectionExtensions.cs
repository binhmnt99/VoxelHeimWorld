using System;
using UnityEngine;
namespace Voxel
{
    public static class DirectionExtensions
    {
        public static Vector3Int GetVector(this Direction direction)
        {
            return direction switch
            {
                Direction.UP => Vector3Int.up,
                Direction.DOWN => Vector3Int.down,
                Direction.LEFT => Vector3Int.left,
                Direction.RIGHT => Vector3Int.right,
                Direction.FRONT => Vector3Int.forward,
                Direction.BACK => Vector3Int.back,
                _ => throw new Exception("Invalid input direction")
            };
        }
    }
}

