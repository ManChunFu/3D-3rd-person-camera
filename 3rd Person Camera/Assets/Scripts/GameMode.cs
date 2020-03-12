using UnityEngine;

public class GameMode : MonoBehaviour
{
    public bool lockCursor = true;

    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
