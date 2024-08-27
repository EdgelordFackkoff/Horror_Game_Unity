using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Pause_Screen : MonoBehaviour
{
    [SerializeField] private Level level;

    //Awake
    void Awake()
    {
        //Reference Level
        level = GetComponentInParent<Level>();
    }

    //Main Menu Botton
    public void main_menu_clicked()
    {
        //Reload scene
        level.game_unpause();
        SceneManager.LoadScene("Main_Menu");
    }

    // Restart Button
    public void restart_clicked()
    {
        level.game_unpause();
        level.LockCursor();
        //Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Workaround
        level.player.allow_input = true;
    }

    //Main Menu Botton
    public void continue_clicked()
    {
        //Enable input
        level.player.allow_input = true;
        //Unpause Level
        level.game_unpause();
        level.LockCursor();
        level.paused = false;
    }
}
