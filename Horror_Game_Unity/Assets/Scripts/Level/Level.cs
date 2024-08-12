using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Game State")]
    //Initially false
    public bool paused = false;

    [Header("Exposure")]
    public int exposure_level;
    public float exposure_amount;

    [Header("Input")]
    public float mouse_sensitivity = 2.0f;
    public string horizontal_move_input = "Horizontal";
    public string vertical_move_input = "Vertical";
    public KeyCode sprint_key = KeyCode.LeftShift;
    public KeyCode jump_key = KeyCode.Space;

    [Header("Global/Shared Variables")]
    public float gravity = 9.18f;

    //Lock Cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //Unlock Cursor
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        //Later code here for other things
    }

    //Pause
    public void game_pause()
    {
        //later
    }

    //Unpause
    public void game_unpause()
    {
        //Later
    }
}
