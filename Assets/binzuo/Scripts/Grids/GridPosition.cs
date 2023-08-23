using System;
namespace binzuo
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int z;

        public GridPosition(int _x, int _z)
        {
            this.x = _x;
            this.z = _z;
        }

        public override bool Equals(Object obj) => obj is GridPosition gridPosition && x == gridPosition.x && z == gridPosition.z;

        public bool Equals(GridPosition other) => this == other;

        public override int GetHashCode() => HashCode.Combine(x,z);

        public override string ToString() =>$"x: {x}; z: {z}";

        public static bool operator ==(GridPosition a, GridPosition b) => a.x == b.x && a.z == b.z;

        public static bool operator !=(GridPosition a, GridPosition b) => !(a == b);
    }
}

