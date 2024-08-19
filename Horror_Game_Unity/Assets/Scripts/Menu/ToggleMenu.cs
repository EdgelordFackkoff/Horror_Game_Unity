using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ToggleMenu : MonoBehaviour
{
    public GameObject UI;
    public Player player;
    public EventSystem eventSystem;

    private void Start()
    {
        UI.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UI.SetActive(!UI.activeSelf);
            UIChecker();
        }

        //if (eventSystem.IsPointerOverGameObject())
        //{
        //    Debug.Log("yep");
        //}
    }

    public void ContinueGame()
    {
        UI.SetActive(false);
        UIChecker();
    }

    private void UIChecker()
    {
        if (UI.activeSelf)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            player.allow_input = false;
        }
        else if (!UI.activeSelf)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            player.allow_input = true;
        }
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        player.allow_input = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainMenuFromGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        player.allow_input = false;
        SceneManager.LoadScene("Main_Menu");
    }
}
