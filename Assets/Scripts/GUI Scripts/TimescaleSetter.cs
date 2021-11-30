using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimescaleSetter : MonoBehaviour
{
    public float timescaleToSet;
    public float maxTime = 10f;
    public UnityEvent onCancelPressed;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timescaleToSet;
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            onCancelPressed.Invoke();
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            if(Input.GetAxis("Vertical") < 0 && Time.timeScale <= 10)
                Time.timeScale += -Input.GetAxis("Vertical");
        }
        else
            Time.timeScale = timescaleToSet;
    }


}