using UnityEngine;
using UnityEngine.UI;

public class LiveObject : MonoBehaviour
{
    public Text hpText;
    public Camera mainCamera;

    //protected FighterType type;
    protected Direction direction;

    [SerializeField]
    protected float timerToMove = 10f;
    protected bool moving = false;
    protected Vector3 newPosition;
    protected Quaternion newRotation;

    [SerializeField]
    protected Vector3 hpTextScreenOffset = new Vector3(0f, 0.3f, 0f);
    protected int hpMaxValue = 100;
    protected int hpValue;
    protected string hpTextFormat = "{0}/{1}";

    protected virtual void Awake()
    {
        //type = FighterType.Square;
        direction = Direction.W;

        newPosition = transform.position;
        newRotation = transform.rotation;

        hpValue = hpMaxValue;
    }

    protected virtual void Update()
    {
        hpText.transform.position = mainCamera.WorldToScreenPoint(transform.position + hpTextScreenOffset);
        hpText.text = string.Format(hpTextFormat, hpValue, hpMaxValue);
    }

    protected virtual void OnDestroy()
    {
        Destroy(hpText);
    }

    public Vector3 GetNewPosition()
    {
        return newPosition;
    }

    public int GetHP()
    {
        return hpValue;
    }

    public void LoseHP(int value)
    {
        hpValue -= value;

        if (hpValue < 0)
        {
            hpValue = 0;
        }
    }
}