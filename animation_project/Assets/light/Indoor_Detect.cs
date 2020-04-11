using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indoor_Detect : MonoBehaviour
{
    public GameObject outdoorArea;

    public bool isIndoor = true;


    // Start is called before the first frame update
    void Start()
    {
        if(outdoorArea == null)
        {
            Debug.Log("outdoorArea is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        isIndoor = true;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, -transform.up);
        if(Physics.Raycast(ray, out hit, 20))
        {
            if(hit.transform.gameObject != gameObject)
            {
                if(hit.transform.gameObject == outdoorArea)
                {
                    isIndoor = false;
                }
            }
        }



    }
}
