using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestThrowItemsModule : MonoBehaviour
{
    //一般變數: 開頭小寫，單字分隔開頭大寫 Ex:myFirstName

    //一般函式(function):開頭小寫，單字分隔開頭大寫 Ex:myFirstFunc();

    //常數:開頭底線+小寫，單字分隔開頭大寫 Ex:_myFirstName

    //類別:開頭大寫，單字分隔開頭大寫 Ex:class MyFirstFamily { };

    // 丟出去的速度
    [SerializeField] private float throwingSpeed = 0.0f;

    // 要丟的物品
    [SerializeField] private GameObject throwedItem;
    // 要丟物品的位置
    [SerializeField] private Transform throwedItemPos;
    // 要丟的物品的剛體
    private Rigidbody throwedItemRb;

    
    //main camera
    [SerializeField] private Camera mainCam = null;
    //人物身上的freeLookCam攝影機
    [SerializeField] private CinemachineFreeLook freeLookCam;
    

    // 攝影機方向
    Vector3 camFor = Vector3.zero;

    // 是否瞄準中
    private bool isTakingAim = false;
    public bool IsTakingAim { get { return isTakingAim; } set { isTakingAim = value; } }

    void Start()
    {
        mainCam = Camera.main;
        throwedItemRb = throwedItem.GetComponent<Rigidbody>();
    }

    void Update()
    {        
        takingAim();
        throwing();
    }
      
    /// <summary>
    /// 瞄準
    /// </summary>
    private void takingAim()
    {
        // 按 T 瞄準
        if (Input.GetKeyDown(KeyCode.T))
        {
            isTakingAim = true;
            camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
            camFor.y = 0;
            transform.forward = camFor;

        }
    }


    private void throwing()
    {
        
        if (isTakingAim && Input.GetMouseButton(0))
        {
            Vector3 dir = transform.forward;

            isTakingAim = false;
            
            camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
            throwedItem.transform.position = throwedItemPos.position;
            dir = camFor.normalized;
            throwedItemRb.velocity = dir * throwingSpeed;
        }
    }
}
