using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridState : MonoBehaviour
{
    public static GridState instance = null;

    public Camera mainCamera;

    public Tilemap controlTileMap;
    public Tilemap obstacleTileMap;

    public List<PathNode> pathfinderGrid;

    public GameObject player;
    public List<GameObject> enemies;

    private Vector3 gridOffsetPosition = new Vector3(0.5f, 0.5f, 0f);

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        CreatePathfinderGrid();
    }

    public bool IsSpaceOccupied(Vector3 position)
    {
        var gridPosition = position - gridOffsetPosition;

        var hasObstacle = obstacleTileMap.HasTile(Vector3Int.RoundToInt(gridPosition));
        var result = IsSpaceOccupiedByEnemy(position);

        return hasObstacle || result.hasEnemy;
    }

    public (bool hasEnemy, GameObject enemy) IsSpaceOccupiedByEnemy(Vector3 position)
    {
        foreach (var enemy in enemies)
        {
            if (Utils.CheckVectorsAreAproxSame(enemy.transform.position, position))
            {
                return (true, enemy);
            }
        }

        return (false, null);
    }

    public bool WillSpaceBeEmpty(GameObject selfEnemy, Vector3 position)
    {
        bool positionEmpty = true;

        foreach (var enemy in enemies.Where(e => e != selfEnemy))
        {
            var enemyController = enemy.GetComponent<EnemyController>();
            positionEmpty = !Utils.CheckVectorsAreAproxSame(enemyController.GetNewPosition(), position);
        }

        return positionEmpty;
    }

    public void AttackEnemyInAttackRange(Vector3 playerPosition, Direction direction, int damage, int range = 1)
    {
        for (int i = 0; i < range; i++)
        {
            var checkPosition = Utils.GetPositionByDirection(direction, i + 1);
            var result = IsSpaceOccupiedByEnemy(playerPosition + checkPosition);

            if (result.hasEnemy)
            {
                var enemyController = result.enemy.GetComponent<EnemyController>();
                enemyController.LoseHP(damage);

                var hpLeft = enemyController.GetHP();
                if (hpLeft == 0)
                {
                    enemies.Remove(result.enemy);
                    Destroy(result.enemy);
                }
            }
        }
    }

    public bool IsEnemyInRange(Vector3 playerPosition, Direction direction, int damage, int range = 1)
    {
        for (int i = 0; i < range; i++)
        {
            var checkPosition = Utils.GetPositionByDirection(direction, i + 1);
            if(IsSpaceOccupiedByEnemy(playerPosition + checkPosition).hasEnemy)
            {
                return true;
            }
        }

        return false;
    }

    public void AlertPlayerMoved(Vector3 playerPosition)
    {
        foreach (var enemy in enemies)
        {
            var enemyController = enemy.GetComponent<EnemyController>();
            enemyController.MoveTowardsPlayer(playerPosition);
        }
    }

    public void CreateMap()
    {
        controlTileMap.CompressBounds();

        int countHorizontalSide = (int)(controlTileMap.size.x / 2f);
        int countVerticalSide = (int)(controlTileMap.size.y / 2f);


    }

    private void CreatePathfinderGrid()
    {
        pathfinderGrid = new List<PathNode>();
        controlTileMap.CompressBounds();

        // Get all tiles
        for (int x = controlTileMap.origin.x; x < controlTileMap.size.x; x++)
        {
            for (int y = controlTileMap.origin.y; y < controlTileMap.size.y; y++)
            {
                if (controlTileMap.HasTile(new Vector3Int(x, y, 0)))
                {
                    Vector3 position = new Vector3(x, y) + gridOffsetPosition;
                    pathfinderGrid.Add(new PathNode(position));
                }
            }
        }

        // Find neighbours
        foreach (var node in pathfinderGrid)
        {
            // N / NE / NW
            var northNodes = pathfinderGrid.Where(n => n.position.y == node.position.y + 1f);
            var north = northNodes.FirstOrDefault(n => n.position.x == node.position.x);
            var northEast = northNodes.FirstOrDefault(n => n.position.x == node.position.x + 1f);
            var northWest = northNodes.FirstOrDefault(n => n.position.x == node.position.x - 1f);

            if (north != null) node.AddUniqueNeighbour(north);
            if (northEast != null) node.AddUniqueNeighbour(northEast);
            if (northWest != null) node.AddUniqueNeighbour(northWest);

            // S / SE / SW
            var southNodes = pathfinderGrid.Where(n => n.position.y == node.position.y - 1f);
            var south = southNodes.FirstOrDefault(n => n.position.x == node.position.x);
            var southEast = southNodes.FirstOrDefault(n => n.position.x == node.position.x + 1f);
            var southWest = southNodes.FirstOrDefault(n => n.position.x == node.position.x - 1f);

            if (south != null) node.AddUniqueNeighbour(south);
            if (southEast != null) node.AddUniqueNeighbour(southEast);
            if (southWest != null) node.AddUniqueNeighbour(southWest);

            // E / W
            var east = pathfinderGrid.FirstOrDefault(n => n.position == new Vector3(node.position.x + 1, node.position.y));
            var west = pathfinderGrid.FirstOrDefault(n => n.position == new Vector3(node.position.x - 1, node.position.y));

            if (east != null) node.AddUniqueNeighbour(east);
            if (west != null) node.AddUniqueNeighbour(west);
        }
    }
}