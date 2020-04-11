using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SD_SL : MonoBehaviour
{
    // 玩家 或 需要偵測陰影的物體
    public GameObject target;

    // 原向量 紀錄光源的軸心用
    public Vector3 oriVec;

    // 忽略的tag
    public string ignoreTag1;

    //偵測光源跟目標的距離
    private float disFromTarget;

    // ray的偵測距離
    private float rayDistance;

    // 光源
    public Transform light;

    // Start is called before the first frame update
    void Start()
    {
        if(ignoreTag1 == "")
        {
            ignoreTag1 = "Plane";
        }

        if(transform.parent != null)
        {
            light = transform.parent;
            rayDistance = light.GetComponent<Light>().range;
        }
        else
        {
            Debug.Log("Light is null!");
        }

        transform.rotation = transform.parent.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            ShadowDetect();
        }
        else
        {
            Debug.Log(transform.name + "'s target is null!");
        }


    }

    void ShadowDetect()
    {
        transform.forward =  (target.transform.position - transform.position);
        disFromTarget = Vector3.Distance(transform.position, target.transform.position);

        RaycastHit hit;
        Ray ray = new Ray(transform.position, target.transform.position - transform.position);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Vector3.Angle(transform.forward, light.forward) < light.GetComponent<Light>().spotAngle / 2
                            && disFromTarget < rayDistance) 
        {
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                if (hit.transform.gameObject != target && hit.transform.tag != ignoreTag1)
                {
                    Debug.Log("Player is in shadow");
                    Debug.Log("Player is under the " + hit.transform.name + " object");
                }
            }
        }
        
    }
}
