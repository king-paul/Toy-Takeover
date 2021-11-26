using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Menu Screens")]
    public GameObject mainMenu;
    public GameObject optionsScreen;

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
            GoToMainMenu();

    }    

    public void SetSelectedButton(GameObject button)
    {
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void LoadScene(string scene)
    {
        //Opens the level defined by the string
        SceneManager.LoadScene(scene);
    }

    public void OpenURL(string url)
    {
        //Opens the URL defined in the string in a web browser
        Application.OpenURL(url);
    }

    public void GotToOptions()
    {
        mainMenu.SetActive(false);
        optionsScreen.SetActive(true);

        GetComponent<GameOptions>().InitGUI();
    }

    public void GoToMainMenu()
    {
        optionsScreen.SetActive(false);
        mainMenu.SetActive(true);        
    }

    public void QuitGame()
    {
        //Closes the game task
        Application.Quit();
        //Enables quitting when playing in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
