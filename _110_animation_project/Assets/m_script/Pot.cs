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
    // 影子模組
    private ShadowModule shadowModule = null;


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
        

        if (type == "fire")
        {
            if(collision.transform.tag == "Torch")
            {                
                collision.transform.GetChild(0).gameObject.SetActive(true);
                Light lightObject = collision.transform.GetComponentInChildren<Light>();
                shadowModule.addNewLightToLights(lightObject.gameObject);
                Destroy(gameObject);
            }
            else
            {
                Vector3 firePosition = transform.position;
                firePosition.y -= 0.5f;
                // 火焰
                GameObject prefab = Instantiate(firePartical, firePosition, firePartical.transform.rotation);
                // 火焰的光源
                Light lightObject = prefab.GetComponentInChildren<Light>();
                // 新增火焰的光源至shadowModule，以便追蹤
                shadowModule.addNewLightToLights(lightObject.gameObject);
                Destroy(gameObject);
            }
            
        }
        else if(type == "water")
        {
            Destroy(gameObject);
        }                        
        
    }


    void OnTriggerEnter(Collider collider)
    {
        
        if (type == "water")
        {
            if (collider.gameObject.tag == "Fire")
            {
                // 產生煙
                GameObject prefab = Instantiate(smokePartical, transform.position, smokePartical.transform.rotation);

                // 碰撞火焰的物件
                Light lightObject = collider.gameObject.GetComponentInChildren<Light>();
                // 刪除光源
                shadowModule.deleteLightsObject(lightObject.gameObject);


                Destroy(collider.gameObject);
                Destroy(gameObject);
            }    
            else if(collider.gameObject.tag == "Torch")
            {
                //Debug.Log("Trigger = " + collider.gameObject.name);
                // 產生煙
                Light lightObject = collider.transform.GetComponentInChildren<Light>();
                shadowModule.deleteLightsObject(lightObject.gameObject);

                GameObject prefab = Instantiate(smokePartical, transform.position, smokePartical.transform.rotation);
                collider.transform.GetChild(0).gameObject.SetActive(false);
                Destroy(gameObject);
            }
        }

        
    }

    /// <summary>
    /// 設 shadow module
    /// </summary>
    /// <param name="module"> shadow module </param>
    public void setShadowModule(ShadowModule module)
    {
        shadowModule = module;
    }


}
