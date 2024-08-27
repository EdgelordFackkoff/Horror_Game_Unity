using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Death_Screen : MonoBehaviour
{
    [SerializeField] private Level level;

    //Awake
    void Awake()
    {
        //Reference Level
        level = GetComponentInParent<Level>();
    }

    // Restart Button
    public void restart_clicked()
    {
        //Reload scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Workaround
        level.player.allow_input = true;
    }

    //Main Menu Botton
    public void main_menu_clicked()
    {
        //Reload scene
        SceneManager.LoadScene("Main_Menu");
    }
}
