using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : LiveObject
{
    public Image arrow;

    private Vector3 playerPosition;
    private Pathfinder pathfinder;

    private List<Vector3> path;

    private ArrowController arrowController;

    protected override void Awake()
    {
        base.Awake();
        playerPosition = Vector3.zero;
    }

    protected virtual void Start()
    {
        pathfinder = new Pathfinder(gameObject);
        path = new List<Vector3>();

        arrowController = new ArrowController(arrow, this.gameObject, mainCamera);
    }

    protected override void Update()
    {
        base.Update();

        arrowController.SetArrowPositionInScreen();

        if (moving)
        {
            newPosition = Utils.RoundVector3(newPosition);
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * timerToMove);

            if (Utils.CompareVectors(transform.position, newPosition, 0.0001f))
            {
                transform.position = newPosition;
                moving = false;
            }
        }

        if (path != null && path.Count > 0)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i], path[i + 1], Color.black, 0.25f, false);
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * timerToMove);
    }

    public void MoveTowardsPlayer(Vector3 inPlayerPosition)
    {
        playerPosition = inPlayerPosition;
        moving = true;

        GetDirectionTowardsPlayer();
    }

    private void GetDirectionTowardsPlayer()
    {
        if (path == null || path.Count == 0 || path.Last() != playerPosition)
        {
            path = pathfinder.FindPath(transform.position, playerPosition);
        }

        if (path != null && path.Count > 1)
        {
            path.RemoveAt(0);
            newPosition = path.First();

            if (newPosition == playerPosition)
            {
                moving = false;
            }
        }
        else
        {
            moving = false;
        }

        var directionVector = newPosition - transform.position;
        var result = Utils.GetRotationBasedOnDirection(directionVector, direction);
        newRotation = result.rotation;
        direction = result.direction;
    }
}