using UnityEngine;

/// <summary>
/// Class that controls the crosshair sprite
/// </summary>
public class CrosshairController : MonoBehaviour
{
    /// <summary>
    /// Vector3 that holds the position of the mouse
    /// </summary>
    private Vector3 _mousePosition;

    /// <summary>
    /// Bool used to show / hide the mouse cursor
    /// </summary>
    private bool _showCursor;

    /// <summary>
    /// Start with the mouse cursor hidden
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        _showCursor = false;
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
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(_mousePosition.x, _mousePosition.y, 0f);
    }

    /// <summary>
    /// Function that show / hide the mouse cursor when escape is pressed
    /// </summary>
    private void ShowHideCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _showCursor = !_showCursor;

        Cursor.visible = _showCursor;
    }
}
