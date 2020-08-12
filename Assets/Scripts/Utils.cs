using UnityEngine;

public static class Utils
{
    public static bool CompareVectors(Vector3 v1, Vector3 v2, float offset)
    {
        return Vector3.SqrMagnitude(v1 - v2) < offset;
    }

    public static Vector3 RoundVector3(Vector3 v)
    {
        Vector3 vector = Vector3.zero;

        vector.x = Mathf.Round(v.x * 10f) / 10f;
        vector.y = Mathf.Round(v.y * 10f) / 10f;

        return vector;
    }

    public static bool CheckVectorsAreAproxSame(Vector3 v1, Vector3 v2)
    {
        return (int)v1.x == (int)v2.x && (int)v1.y == (int)v2.y;
    }

    public static (Quaternion rotation, Direction direction) GetRotationBasedOnDirection(Vector3 directionVector, Direction currentDirection)
    {
        if (directionVector.x > 0)
        {
            if (directionVector.y > 0)
            {
                currentDirection = Direction.NE;
            }
            else if (directionVector.y < 0)
            {
                currentDirection = Direction.SE;
            }
            else
            {
                currentDirection = Direction.E;
            }
        }
        else if (directionVector.x < 0)
        {
            if (directionVector.y > 0)
            {
                currentDirection = Direction.NW;
            }
            else if (directionVector.y < 0)
            {
                currentDirection = Direction.SW;
            }
            else
            {
                currentDirection = Direction.W;
            }
        }
        else
        {
            if (directionVector.y > 0)
            {
                currentDirection = Direction.N;
            }
            else if (directionVector.y < 0)
            {
                currentDirection = Direction.S;
            }
        }

        float rotationZ = (float)currentDirection;
        return (Quaternion.Euler(0f, 0f, rotationZ), currentDirection);
    }

    public static Vector3 GetPositionByDirection(Direction direction, int range = 1)
    {
        switch (direction)
        {
            case Direction.N:
                return Vector3.up * range;
            case Direction.S:
                return Vector3.down * range;
            case Direction.E:
                return Vector3.right * range;
            case Direction.W:
                return Vector3.left * range;
            case Direction.NE:
                return (Vector3.up + Vector3.right) * range;
            case Direction.NW:
                return (Vector3.up + Vector3.left) * range;
            case Direction.SE:
                return (Vector3.down + Vector3.right) * range;
            case Direction.SW:
                return (Vector3.down + Vector3.left) * range;
            default:
                return Vector3.zero;
        }
    }
}