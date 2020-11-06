using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一般變數: 開頭小寫，單字分隔開頭大寫 Ex:myFirstName

//一般函式(function):開頭小寫，單字分隔開頭大寫 Ex:myFirstFunc();

//常數:開頭底線+小寫，單字分隔開頭大寫 Ex:_myFirstName

//類別:開頭大寫，單字分隔開頭大寫 Ex:class MyFirstFamily { };

public class Pot : MonoBehaviour
{
    // Pot 的種類
    [SerializeField] string type = "";

    // 碰撞體
    private BoxCollider collider = null;

    // 火焰特效
    [SerializeField] private GameObject firePartical = null;
    // 煙
    [SerializeField] private GameObject smokePartical = null;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision = " + collision.gameObject.name);
        if(type == "fire")
        {
            Vector3 firePosition = transform.position;
            firePosition.y -= 0.5f;
            GameObject prefab = Instantiate(firePartical, firePosition, firePartical.transform.rotation);            
            Destroy(gameObject);
        }
        else if(type == "water")
        {
            Destroy(gameObject);
        }                        
    }


    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Trigger = " + collider.gameObject.name);
        if (type == "water")
        {
            if (collider.gameObject.tag == "Fire")
            {
                GameObject prefab = Instantiate(smokePartical, transform.position, smokePartical.transform.rotation);
                Destroy(collider.gameObject);
                Destroy(gameObject);
            }            
        }

        
    }


}
