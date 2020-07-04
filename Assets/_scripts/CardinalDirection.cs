using System;
using UnityEngine;

public enum CardinalDirection
{
    None, Up, Down, Left, Right
}
public static class CardinalDirectionHelper
{
    public static Vector3 ToVector3(CardinalDirection direction)
    {
        switch ( direction )
        {
            case CardinalDirection.None: return Vector3.zero;
            case CardinalDirection.Up: return Vector3.up;
            case CardinalDirection.Down: return Vector3.down;
            case CardinalDirection.Left: return Vector3.left;
            case CardinalDirection.Right: return Vector3.right;
            default: throw new Exception( "not possible" );
        }
    }
    public static float? ToRotationDegrees(CardinalDirection direction)
    {
        switch ( direction )
        {
            case CardinalDirection.None: return null;
            case CardinalDirection.Up: return 270;
            case CardinalDirection.Down: return 90;
            case CardinalDirection.Left: return 180;
            case CardinalDirection.Right: return 0;
            default: throw new Exception( "not possible" );
        }
    }
}
