using UnityEngine;

/// <summary>
/// Class that controls the crosshair sprite
/// </summary>
public class CrosshairController : MonoBehaviour
{
    /// <summary>
    /// Vector3 that holds the position of the mouse
    /// </summary>
    private Vector3 mousePosition;

    /// <summary>
    /// Bool used to show / hide the mouse cursor
    /// </summary>
    private bool showCursor;

    /// <summary>
    /// Start with the mouse cursor hidden
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        showCursor = false;
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
        ShowHideCursor();
    }

    /// <summary>
    /// Function that makes the crosshair sprite follow the mouse position
    /// </summary>
    private void FollowMouse()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);
    }

    /// <summary>
    /// Function that show / hide the mouse cursor when escape is pressed
    /// </summary>
    private void ShowHideCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            showCursor = !showCursor;

        Cursor.visible = showCursor;
    }
}
