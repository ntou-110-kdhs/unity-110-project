using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowItemsModule : MonoBehaviour
{
    //一般變數: 開頭小寫，單字分隔開頭大寫 Ex:myFirstName

    //一般函式(function):開頭小寫，單字分隔開頭大寫 Ex:myFirstFunc();

    //常數:開頭底線+小寫，單字分隔開頭大寫 Ex:_myFirstName

    //類別:開頭大寫，單字分隔開頭大寫 Ex:class MyFirstFamily { };

    //components
    private PlayerController playerController;
    private CharacterController charController;
    private ShadowModule shadowModule;
    private Push_Module pushModule;
    private EnemyManager enemyManager;


    // 要丟出去的 "Prefab"
    private GameObject throwPrefab = null;
    // 丟出去的速度
    [SerializeField] private float throwingSpeed = 0.0f;
    // 要丟的物品
    [SerializeField] private List<GameObject> throwedItems;
    // 要丟的物品的圖片
    [SerializeField] private List<Sprite> throwedSprites;
    // 丟物品的UI
    [SerializeField] private Image throwItemUI;
    // 目前丟東西的順位
    private int throwIndex = 0;
    // 要丟物品的位置
    [SerializeField] private Transform throwedItemPos;

    
    //main camera
    private Camera mainCam = null;
    //人物身上的freeLookCam攝影機
    [SerializeField] private CinemachineFreeLook freeLookCam;
    //FreeLook Cam Look at 的 head position
    [SerializeField] private Transform headPosition; 
        

    //人物的animateController
    private PlayerAnimateController animateController;

    // 攝影機方向
    [SerializeField] Vector3 camFor = Vector3.zero;

    // 是否瞄準中
    private bool isTakingAim = false;
    public bool IsTakingAim { get { return isTakingAim; } set { isTakingAim = value; } }

    private bool isResetCam = false;

    void Start()
    {
        //獲取component
        playerController = this.GetComponent<PlayerController>();
        if (playerController == null) Debug.LogError("player Controller is not attatched");
        charController = this.GetComponent<CharacterController>();
        if (charController == null) Debug.LogError("character Controller is not attatched");
        shadowModule = GetComponent<ShadowModule>();
        if (shadowModule == null) Debug.LogError("Shadow Module is not attatched");
        pushModule = GetComponent<Push_Module>();
        enemyManager = FindObjectOfType<EnemyManager>();

        mainCam = Camera.main;

        //取得animateController
        animateController = GetComponent<PlayerAnimateController>();


        throwItemUI.sprite = throwedSprites[0];
    }

    void Update()
    {
        changeIndex();
        takingAim();
        throwing();
    }
      
    /// <summary>
    /// 瞄準
    /// </summary>
    private void takingAim()
    {
        // 按 T 瞄準
        if (Input.GetKeyDown(KeyCode.T) && charController.isGrounded && !pushModule.IsPushingObject && !shadowModule.IsShadowing && !playerController.IsShooting && !playerController.IsShooting && !this.IsTakingAim)
        {
            //呼叫動畫控制器 開始瞄準動畫
            animateController.throwingObjectTrigger();
            isTakingAim = true;
            isResetCam = false;
            camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
            camFor.y = 0;
            transform.forward = camFor;


            // 生成物品
            throwPrefab = Instantiate(throwedItems[throwIndex]);
            // 設定parent
            throwPrefab.transform.parent = throwedItemPos;
            // 設定位置
            throwPrefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            // 設定 kinematic
            throwPrefab.GetComponent<Rigidbody>().isKinematic = true;

        }
    }

    /// <summary>
    /// 當動畫撥放到要丟出物品時呼叫
    /// </summary>
    private void throwItemOut()
    {
        Vector3 dir = transform.forward;
        //isTakingAim = false;
        
        camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
        dir = camFor.normalized;
        // 設定 kinematic
        throwPrefab.GetComponent<Rigidbody>().isKinematic = false;
        // 設定 parent 
        throwPrefab.transform.parent = null;
        // 設定速度
        throwPrefab.GetComponent<Rigidbody>().velocity = dir * throwingSpeed;
        enemyManager.setAllEnemiesCheckTarget(throwPrefab.transform);
        // 設影子模組  偵測火焰燈光用
        if (throwPrefab.GetComponent<Pot>() != null)
        {
            throwPrefab.GetComponent<Pot>().setShadowModule(shadowModule);

        }

    }

    /// <summary>
    /// 物品投擲結束(動畫中呼叫) isTakingAim重設
    /// </summary>
    private void throwItemOutFinished()
    {
        isTakingAim = false;


        //resetCamera();
        //結束時呼叫 取消TRIGGER
        animateController.resetAllTrigger();
    }


    /// <summary>
    /// 重新設置camera
    /// </summary>
    private void resetCamera()
    {
        isResetCam = true;
    }

    /// <summary>
    /// 更改丟的東西
    /// </summary>
    private void changeIndex()
    {
        // 按 Q 切換
        if (Input.GetKeyDown(KeyCode.Q))
        {
            throwIndex++;
            throwIndex = throwIndex % 3;
            throwItemUI.sprite = throwedSprites[throwIndex];
            if (isTakingAim)
            {
                // 銷毀prefab
                Destroy(throwPrefab);
                // 生成物品
                throwPrefab = Instantiate(throwedItems[throwIndex]);
                // 設定parent
                throwPrefab.transform.parent = throwedItemPos;
                // 設定位置
                throwPrefab.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
                // 設定 kinematic
                throwPrefab.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
    }
    /// <summary>
    /// 丟東西
    /// </summary>
    private void throwing()
    {
        camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
        
        if(isResetCam)
        {
            // 攝影機歸位
            Vector3 smoothPos = Vector3.zero;



            headPosition.localPosition = Vector3.Lerp(headPosition.localPosition, new Vector3(0.0f, 1.56f, 0.0f), Time.deltaTime * 5);
            

            freeLookCam.m_Orbits[0].m_Height = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Height, 4.0f, Time.deltaTime);
            freeLookCam.m_Orbits[0].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Radius, 1.75f, Time.deltaTime);
            freeLookCam.m_Orbits[1].m_Height = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Height, 2.5f, Time.deltaTime);
            freeLookCam.m_Orbits[1].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Radius, 3.0f, Time.deltaTime);
        }
        else if (isTakingAim)
        {

            headPosition.localPosition = new Vector3(0.415f, 1.666f, -0.465f);
            freeLookCam.m_Orbits[0].m_Height = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Height, 2, Time.deltaTime * 5);
            freeLookCam.m_Orbits[0].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[0].m_Radius, 1, Time.deltaTime * 5);
            freeLookCam.m_Orbits[1].m_Height = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Height, 2, Time.deltaTime * 5);
            freeLookCam.m_Orbits[1].m_Radius = Mathf.Lerp(freeLookCam.m_Orbits[1].m_Radius, 1, Time.deltaTime * 5);

            if (Input.GetMouseButton(0))
            {
                //呼叫動畫控制器 開始投擲動畫
                animateController.throwingObject();
            }
            else if (Input.GetKeyDown(KeyCode.T))
            {
                //isTakingAim = false;
            }
            else
            {
                camFor.y = 0.0f;
                Quaternion targetRotation = Quaternion.LookRotation(camFor, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f/*每一frame轉向 5.0 度*/);
            }
        }


    }
}
