using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Script: MonoBehaviour
{
    public void OpenLevel(string scene)
    {
        //Opens the level defined by the string
        SceneManager.LoadScene(scene);
    }

    public void OpenURL(string url)
    {
        //Opens the URL defined in the string in a web browser
        Application.OpenURL(url);
    }

    public void GameQuit()
    {
        //Closes the game task
        Application.Quit();
        //Enables quitting when playing in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
