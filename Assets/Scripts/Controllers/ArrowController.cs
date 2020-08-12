using UnityEngine;
using UnityEngine.UI;

public class ArrowController
{
    private Image arrow;
    private GameObject objectToFollow;
    private Camera camera;

    private Renderer objectRenderer;
    private Vector3 sizeOfScreen;

    private Vector3 arrowOffset = new Vector3(12f, 12f);

    public ArrowController(Image arrow, GameObject objectToFollow, Camera camera)
    {
        this.arrow = arrow;
        this.objectToFollow = objectToFollow;
        this.camera = camera;

        objectRenderer = this.objectToFollow.GetComponent<Renderer>();
        sizeOfScreen = new Vector3(this.camera.pixelWidth, this.camera.pixelHeight);
    }

    public void SetArrowPositionInScreen()
    {
        if (objectRenderer.isVisible)
        {
            arrow.enabled = false;
        }
        else
        {
            arrow.enabled = true;
            arrow.transform.position = CalculatePositionInEdgeOfScreen();
        }
    }

    private Vector3 CalculatePositionInEdgeOfScreen()
    {
        Vector3 position = camera.WorldToScreenPoint(objectToFollow.transform.position);

        if (position.x < 0)
        {
            position.x = arrowOffset.x;
        }
        else if (position.x > sizeOfScreen.x)
        {
            position.x = sizeOfScreen.x - arrowOffset.x;
        }

        if (position.y < 0)
        {
            position.y = arrowOffset.y;
        }
        else if (position.y > sizeOfScreen.y)
        {
            position.y = sizeOfScreen.y - arrowOffset.y;
        }

        return position;
    }
}