using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    //public Button playButton;
    //public Button feedBackButton;
    //public Button quitButton;

    public void Update()
    {
        //if(playButton.OnPointerEnter())
        
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
