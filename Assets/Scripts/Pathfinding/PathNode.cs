using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public Vector3 position { get; }

    public List<PathNode> neighbourNodes { get; }

    public PathNode cameFrom { get; set; }

    public int gCost { get; set; }
    public int hCost { get; set; }
    public int fCost { get; set; }

    public PathNode(Vector3 nodePosition)
    {
        position = nodePosition;
        neighbourNodes = new List<PathNode>();
    }

    public void AddUniqueNeighbour(PathNode neighbour)
    {
        if (!neighbourNodes.Contains(neighbour))
        {
            neighbourNodes.Add(neighbour);
        }
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }
}