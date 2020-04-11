using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SD_DLPL : MonoBehaviour
{
    // 是否為太陽
    public bool isSun = false;

    // 玩家 或 需要偵測陰影的物體
    public GameObject target;

    // 距離
    private float distance;
    // ray的偵測距離
    private float rayDistance;

    //是否在室內
    private bool targetIsIndoor;

    // 忽略的tag
    public string ignoreTag1;

    // 偵測光源跟目標的距離
    private float disFromTarget;
    // Start is called before the first frame update
    void Start()
    {
        if(ignoreTag1 == "")
        {
            ignoreTag1 = "Plane";
        }
        
        if (isSun)
        {
            distance = 10000.0f;
            rayDistance = distance * 10.0f;
        }
        else
        {
            distance = GetComponent<Light>().range;
            rayDistance = distance;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            targetIsIndoor = target.GetComponent<Indoor_Detect>().isIndoor;
            ShadowDetect();
        }
        else
        {
            Debug.Log(transform.name + "'s target is null!");
        }
    }

    // 偵測影子
    void ShadowDetect()
    {

        Vector3 rayDirection = -(transform.position - target.transform.position);
       
        if (isSun)
        {
            transform.position = transform.parent.rotation * new Vector3(0.0f, 0.0f, -distance);
        }
        else
        {
            disFromTarget = Vector3.Distance(transform.position, target.transform.position); 
        }

        // ray的部分
        RaycastHit hit;
        Ray ray = new Ray(transform.position, rayDirection);

        // 畫ray
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // if判斷式 = ray所需要忽略的物體
            if (hit.transform.gameObject != target && hit.transform.tag != ignoreTag1)
            {
                if (isSun)
                {
                    if (!targetIsIndoor)
                    {
                    Debug.Log("Player is in shadow");
                    Debug.Log("Player is under the " + hit.transform.name + " object");
                    }
                    
                }
                // 不是太陽光都沒差
                else    
                {
                    if(disFromTarget < rayDistance)
                    {
                        Debug.Log("Player is in shadow");
                        Debug.Log("Player is under the " + hit.transform.name + " object");
                    }
                }

            }
        }
    }
}