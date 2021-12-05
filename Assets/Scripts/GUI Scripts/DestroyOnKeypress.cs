using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnKeypress : MonoBehaviour
{
    static bool showTitleScreen = true;

    void Start()
    {
        if(!showTitleScreen)
            GameObject.Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            showTitleScreen = false;
            GameObject.Destroy(this.gameObject);
        }
    }
}
