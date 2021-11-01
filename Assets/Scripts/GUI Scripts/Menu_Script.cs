using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    //Opens the level defined by the string
    public void OpenLevel(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    //Opens the URL defined in the string in a web browser
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
    }

    //Closes the game task
    public void GameQuit()
    {
        Application.Quit();
        //Enables quitting when playing in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
