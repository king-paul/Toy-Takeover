using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMainMenu : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            //SceneManager.UnloadSceneAsync("Title Screen");
            SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);            
        }
    }
}
