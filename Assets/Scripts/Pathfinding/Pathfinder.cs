using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder
{
    private List<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;

    private GameObject enemy;

    private const int MOVE_DIAGONAL_COST = 14;
    private const int MOVE_STRAIGHT_COST = 10;

    public Pathfinder(GameObject enemy)
    {
        grid = GridState.instance.pathfinderGrid;
        this.enemy = enemy;
    }

    public List<Vector3> FindPath(Vector3 initialPosition, Vector3 endPosition)
    {
        PathNode startNode = grid.First(x => Utils.CheckVectorsAreAproxSame(x.position, initialPosition));
        PathNode endNode = grid.First(x => Utils.CheckVectorsAreAproxSame(x.position, endPosition));

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        foreach (var node in grid)
        {
            node.gCost = int.MaxValue;
            node.CalculateFCost();
            node.cameFrom = null;
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            PathNode currentNode = GetNodeLowestFCost(openList);

            if (currentNode == endNode)
            {
                return RecontructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in currentNode.neighbourNodes)
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!GridState.instance.WillSpaceBeEmpty(enemy, neighbourNode.position))
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFrom = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDistance = (int)Mathf.Abs(a.position.x - b.position.x);
        int yDistance = (int)Mathf.Abs(a.position.y - b.position.y);
        int remaining = (int)Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetNodeLowestFCost(List<PathNode> list)
    {
        PathNode lowestFCostNode = list.First();

        foreach (var node in list)
        {
            if (node.fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = node;
            }
        }

        return lowestFCostNode;
    }

    private List<Vector3> RecontructPath(PathNode currentNode)
    {
        List<Vector3> path = new List<Vector3> { currentNode.position };
        PathNode node = currentNode;

        while (node.cameFrom != null)
        {
            path.Add(node.cameFrom.position);
            node = node.cameFrom;
        }

        path.Reverse();
        return path;
    }
}