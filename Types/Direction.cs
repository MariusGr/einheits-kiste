using System.Collections.Generic;

namespace EinheitsKiste
{
    public enum Direction
    {
        None,
        Right,
        Left,
        Up,
        Down,
    }

    public enum Orientation
    {
        None,
        Horizontal,
        Vertical,
    }

    public static class Utils
    {
        public static Dictionary<Direction, Direction> DirectionInverse = new()
        {
            { Direction.Left, Direction.Right },
            { Direction.Right, Direction.Left },
            { Direction.Up, Direction.Down },
            { Direction.Down, Direction.Up },
            { Direction.None, Direction.None },
        };

        public static Dictionary<Direction, Orientation> DirectionToOrientation = new()
        {
            { Direction.Left, Orientation.Horizontal },
            { Direction.Right, Orientation.Horizontal },
            { Direction.Up, Orientation.Vertical },
            { Direction.Down, Orientation.Vertical },
            { Direction.None, Orientation.None },
        };

        public static bool IsVertical(Direction direction) => DirectionToOrientation[direction] == Orientation.Vertical;
        public static bool IsHorizontal(Direction direction) => DirectionToOrientation[direction] == Orientation.Horizontal;
    }
}
