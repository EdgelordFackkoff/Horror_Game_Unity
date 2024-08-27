using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadLevelSelection()
    {
        SceneManager.LoadScene("Level_Selection_Menu");
    }

    public void LoadSettings()
    {
        SceneManager.LoadScene("Settings_Menu");
    }

    public void QuitGame()
    {
        // Only works in build not editor
        Application.Quit();
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main_Menu");
    }

    ///////////////////////////////////////////////////////////////////////////
    // REMEMBER TO UPDATE TO SCENE NAMES
    // ALSO REMEMBER TO ADD YOUR SCENE TO THE BUILD IN:
    // File -> Build Settings -> Add Open Scenes(click with your scene open)
    public void LoadLevel1()
    {
        SceneManager.LoadScene("Forgotten Library"); // Level_1
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Castle");
    }

    public void LoadLevel3()
    {
        SceneManager.LoadScene("Level_3");
    }
    ///////////////////////////////////////////////////////////////////////////
}
