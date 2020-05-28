using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirWall : MonoBehaviour
{

    private BoxCollider boxCollider;
    public Vector3 pos { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!boxCollider.isTrigger)
        {
            transform.position = pos;

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.transform.GetComponent<AirWall>())
        {
            Debug.Log("test");
        }
        
    }

  
}
