using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnKeypress : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
            GameObject.Destroy(this.gameObject);
    }
}
