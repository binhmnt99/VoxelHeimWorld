using System;
namespace binzuo
{
    public struct GridPosition : IEquatable<GridPosition>
    {
        public int x;
        public int z;
        public int floor;

        public GridPosition(int _x, int _z, int _floor)
        {
            this.x = _x;
            this.z = _z;
            this.floor = _floor;
        }

        public override bool Equals(Object obj) => obj is GridPosition gridPosition && x == gridPosition.x && z == gridPosition.z && floor == gridPosition.floor;

        public bool Equals(GridPosition other) => this == other;

        public override int GetHashCode() => HashCode.Combine(x,z, floor);

        public override string ToString() =>$"x: {x}; z: {z}; floor: {floor}";

        public static bool operator ==(GridPosition a, GridPosition b) => a.x == b.x && a.z == b.z && a.floor == b.floor;

        public static bool operator !=(GridPosition a, GridPosition b) => !(a == b);

        public static GridPosition operator +(GridPosition a, GridPosition b) => new GridPosition(a.x + b.x, a.z + b.z, a.floor + b.floor);

        public static GridPosition operator -(GridPosition a, GridPosition b) => new GridPosition(a.x - b.x, a.z - b.z, a.floor - b.floor);
    }
}

