using UnityEngine;

public class MapNode
{
    Vector3 screenPosition { get; }

    public MapNode(Vector3 position)
    {
        screenPosition = position;
    }
}