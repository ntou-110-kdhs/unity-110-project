using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class outputKeyboardInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ClearConsole());
        float input_H = Input.GetAxis("Horizontal");
        float input_V = Input.GetAxis("Vertical");
        if(input_H > 0)
        {
            Debug.Log("D:右");
        }
        else if (input_H < 0)
        {
            Debug.Log("A:左");
        }
        if (input_V > 0)
        {
            Debug.Log("W:上");
        }
        else if (input_V < 0)
        {
            Debug.Log("S:下");
        }
    }
    IEnumerator ClearConsole()
    {
        // wait until console visible
        while (!Debug.developerConsoleVisible)
        {
            yield return null;
        }
        yield return null; // this is required to wait for an additional frame, without this clearing doesn't work (at least for me)
        Debug.ClearDeveloperConsole();
    }
}
