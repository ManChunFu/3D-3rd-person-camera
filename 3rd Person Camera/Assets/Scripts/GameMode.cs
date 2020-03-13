using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

public class GameMode : MonoBehaviour
{
    public bool lockCursor = true;
    public Transform player = default;

    private void Awake()
    {
        Assert.IsNotNull(player, "Do you forget to assign the player?");
    }
    private void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (player != null)
        {
            if (player.position.y < -5.0f)
            { SceneManager.LoadScene(0); }
        }
    }
}
