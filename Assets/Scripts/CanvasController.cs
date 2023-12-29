using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject canvasToShow; // Assign the canvas you want to show

    public void ShowCanvas()
    {
        if (canvasToShow != null)
        {
            canvasToShow.SetActive(true);
        }
    }

    public void HideCanvas()
    {
        if (canvasToShow != null)
        {
            canvasToShow.SetActive(false);
        }
    }
}
