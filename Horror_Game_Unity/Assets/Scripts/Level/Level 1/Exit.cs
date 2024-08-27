using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public ObjectiveManager objective_manager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("help " + objective_manager.can_exit);

        if (objective_manager.can_exit)
            LoadLevelSelection();
    }

    public void LoadLevelSelection()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Level_Selection_Menu");
    }
}
