using TMPro;
using UnityEngine;

public class PlayerController : LiveObject
{
    public TextMeshProUGUI attackText;

    public GameObject map;

    private bool mapIsOpen;

    protected override void Awake()
    {
        base.Awake();

        mapIsOpen = false;
    }

    protected virtual void Start()
    {
        attackText.alpha = 0f;
    }

    protected override void Update()
    {
        base.Update();

        map.SetActive(mapIsOpen);

        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, mainCamera.transform.position.z);

        if (GridState.instance.IsEnemyInRange(transform.position, direction, 20, 1))
        {
            attackText.alpha = 1f;
        }
        else
        {
            attackText.alpha = 0f;
        }

        if (!moving)
        {
            InputMovement();
            Attack();
        }
        else
        {
            Movement();
        }

        Rotation();
    }

    private void InputMovement()
    {
        var horizontalMovement = (int)Input.GetAxisRaw("Horizontal");
        var verticalMovement = (int)Input.GetAxisRaw("Vertical");

        if (horizontalMovement != 0f || verticalMovement != 0f)
        {
            var directionVector = new Vector3(horizontalMovement, verticalMovement);
            newPosition = Utils.RoundVector3(transform.position + directionVector);
            var result = Utils.GetRotationBasedOnDirection(directionVector, direction);
            newRotation = result.rotation;
            direction = result.direction;

            if (!GridState.instance.IsSpaceOccupied(newPosition))
            {
                moving = true;
                GridState.instance.AlertPlayerMoved(newPosition);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            mapIsOpen = true;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            mapIsOpen = false;
        }
    }

    private void Movement()
    {
        newPosition = Utils.RoundVector3(newPosition);
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * timerToMove);

        if (Utils.CompareVectors(transform.position, newPosition, 0.0001f))
        {
            transform.position = newPosition;
            moving = false;
        }
    }

    private void Rotation()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * timerToMove);
    }

    private void Attack()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            GridState.instance.AttackEnemyInAttackRange(transform.position, direction, 20, 1);
            GridState.instance.AlertPlayerMoved(newPosition);
        }
    }
}