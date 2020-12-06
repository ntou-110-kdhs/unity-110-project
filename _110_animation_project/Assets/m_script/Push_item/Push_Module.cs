using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//放在玩家身上的推移函式
//有取用shadowModule;
public class Push_Module : MonoBehaviour
{
    private PlayerController playerController;
    private CharacterController charController;
    private ShadowModule shadowModule;
    private ThrowItemsModule throwModule;

    //人物的animateController
    private PlayerAnimateController animateController;
    //人物身上的freeLookCam攝影機
    [SerializeField] private CinemachineFreeLook freeLookCam;



    /**********推移物件*********/
    [SerializeField]
    private bool isPushingObject = false;
    public bool IsPushingObject { get { return isPushingObject; } set { isPushingObject = value; } }
    private bool isFirstIn = true;       //確認是否為第一次進入推動狀態  會在FORWARD為0時 重置
    public bool IsFirstIn { get { return isFirstIn; } set { isFirstIn = value; } }
    private GameObject pushedObject = null;  //紀錄推動中的物件  若因角色因素取消連接  會使用到
    public GameObject PushedObject { get { return pushedObject; } set { pushedObject = value; } }
    private float pushTime = -10;           //按下前進的時間  大於一定時間才會開始移動
    public float PushTime { get { return pushTime; } set { pushTime = value; } }

    /**********推移物件*********/

    /********鍵鼠操控變數*******/
    //水平鍵(A,D)有按與否
    private float inputHor;
    //垂直鍵(W,S)有按與否
    private float inputVer;
    //滑鼠水平(X軸)移動
    private float mouseX;
    //滑鼠垂直(Y軸)移動
    private float mouseY;
    /********鍵鼠操控變數*******/

    /**********物理性質*********/
    [SerializeField] private float charSpeed = 6.0f;
    [SerializeField] private float charJumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;
    /**********物理性質*********/

    //人物移動方向
    private Vector3 moveDirection = Vector3.zero;



    // Start is called before the first frame update
    void Start()
    {
        playerController = this.GetComponent<PlayerController>();
        if (playerController == null) Debug.LogError("player Controller is not attatched");
        charController = this.GetComponent<CharacterController>();
        if (charController == null) Debug.LogError("character Controller is not attatched");
        shadowModule = GetComponent<ShadowModule>();
        if (shadowModule == null) Debug.LogError("Shadow Module is not attatched");

        //丟東西模組
        throwModule = GetComponent<ThrowItemsModule>();
        if (freeLookCam == null)
        {
            freeLookCam = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();
        }

        //取得animateController
        animateController = GetComponent<PlayerAnimateController>();
    }

    // Update is called once per frame
    void Update()
    {
        //物件推移
        Ray rayObject = new Ray(transform.position + new Vector3(0.0f, 1.25f, 0.0f), transform.forward); //用於判斷前方是否有可推動物體
        //Debug.DrawRay(transform.position + new Vector3(0.0f, 1.25f, 0.0f), transform.forward*1.5f,Color.green);
        RaycastHit hit;


        /**********推移物品*********/
        if (Physics.Raycast(rayObject, out hit, 1.75f))
        {
            Debug.Log(hit.transform.gameObject.name);
            //擊中Movable物件 且在地面 且目前沒有推動物件 且不在影子狀態中 才可以推動物體
            if (hit.transform.GetComponent<FixedJoint>() != null)       //當物體有FixedJoint時
            {
                Debug.Log("asdad");
                if (hit.transform.tag == ("Movable") && Input.GetKeyDown(KeyCode.F) && charController.isGrounded && !isPushingObject && !shadowModule.IsShadowing && !playerController.IsShooting && !throwModule.IsTakingAim)
                {
                    //更改角色的面向  以及位置
                    //為此  必須先關閉角色控制器
                    //位置為 raycast打到的點的x,z 與角色的y座標  在加上打到的點之法向量x,y*1.2(後退的效果)
                    //轉向則是法向量的x,z的法向量
                    charController.enabled = false;
                    Vector3 tempVector = new Vector3(hit.point.x, this.transform.position.y, hit.point.z) + new Vector3((float)1.2 * hit.normal.x, 0, (float)1.2 * hit.normal.z);
                    this.transform.position = tempVector;
                    this.transform.rotation = Quaternion.LookRotation(new Vector3(hit.normal.x * -1, 0, hit.normal.z * -1));
                    charController.enabled = true;


                    /*
                    float distance = Vector3.Distance(hit.point, this.transform.position);
                    Debug.Log(hit.normal);
                    if (distance < 1) charController.Move(-1*this.transform.forward / 3);
                    else if (distance >= 1 && distance < 1.3) charController.Move(-1 * this.transform.forward / 4);
                    else if (distance >= 1.3 && distance < 1.5) charController.Move(-1 * this.transform.forward / 5);
                    else if (distance >= 1.5 && distance < 2) charController.Move( this.transform.forward / 5);
                    */
                    hit.transform.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();        //連接物體
                    pushedObject = hit.transform.gameObject;                                         //紀錄物體
                    isFirstIn = true;
                    isPushingObject = true;

                }
                else if (hit.transform.tag == ("Movable") && Input.GetKeyDown(KeyCode.F) && charController.isGrounded && isPushingObject == true && !shadowModule.IsShadowing && !throwModule.IsTakingAim)   //再次點擊 取消
                {
                    if (pushedObject != null) pushedObject.GetComponent<FixedJoint>().connectedBody = null;
                    pushedObject = null;
                    isPushingObject = false;

                }
                else if (!charController.isGrounded)                                                            //角色離開地面時取消
                {
                    if (pushedObject != null) pushedObject.GetComponent<FixedJoint>().connectedBody = null;
                    pushedObject = null;
                    isPushingObject = false;

                }
            }

        }
        else                                            //RAYCAST 一離開推動的物體  就取消連接
        {

            if (pushedObject != null && pushedObject.GetComponent<FixedJoint>() != null) pushedObject.GetComponent<FixedJoint>().connectedBody = null;
            pushedObject = null;
            isPushingObject = false;


        }
        /**********推移物品*********/
    }


    public void dragMove()             //拖拉物體時的移動
    {
        //水平鍵(A,D)有按與否
        inputHor = Input.GetAxis("Horizontal");
        //垂直鍵(W,S)有按與否
        inputVer = Input.GetAxis("Vertical");
        //滑鼠水平(X軸)移動
        mouseX = Input.GetAxis("Mouse X");
        //滑鼠垂直(Y軸)移動
        mouseY = Input.GetAxis("Mouse Y");

        float dirZ = inputVer;
        float dirX = inputHor;
        //float dirY = 0;


        //避免滑行  但除了落下的力
        moveDirection.x = 0;
        moveDirection.z = 0;

        // 移動
        if (inputHor != 0 || inputVer != 0)
        {

            //計算CAMERA與玩家間的角度
            float camAngle = 0;


            Vector3 objectForward = this.transform.forward;
            Vector3 camDir = Vector3.zero;
            camDir = freeLookCam.transform.position - this.transform.position;
            camDir.y = 0;
            objectForward.y = 0;
            camAngle = Vector3.Angle(objectForward, camDir);
            //計算CAMERA與玩家間的角度

            //若向前速度 -0.1<=速度<=0.1  重置isFirstIn
            if (animateController.forward >= -0.1 && animateController.forward <= 0.1) isFirstIn = true;

            if (camAngle <= 70)             //在前方
            {
                /*************配合動畫************/
                //若速度>0.1  或 <-0.1  且為第一次進入      紀錄開始推動時的時間    
                if ((inputVer * -1 > 0.1 || inputVer * -1 < -0.1) && isFirstIn)
                {
                    pushTime = Time.time;
                    isFirstIn = false;
                }
                //Debug.Log("T-P="+(Time.time - pushTime)+"T="+Time.time+"p="+pushTime);

                //若推動時間小於1秒  停止移動
                if (Time.time - pushTime < 1f)
                {
                    moveDirection = transform.TransformDirection(new Vector3(0, 0, 0));
                    //Debug.Log(pushTime);
                }
                else
                {
                    moveDirection = transform.TransformDirection(new Vector3(0, 0, inputVer * -1));
                }
                /*************配合動畫************/


                //控制FORWARD 變數
                if (inputVer * -1 > 0.1) animateController.addForward();
                else if (inputVer * -1 < -0.1) animateController.minusForward();
                else animateController.setToZero();
                //Debug.Log("forward");
                //控制FORWARD 變數
            }
            else if (camAngle > 70 && camAngle <= 115)
            {
                float rightAngle = 0;
                Vector3 objectRight = this.transform.right; //給予物體右側向量
                objectRight.y = 0;
                rightAngle = Vector3.Angle(objectRight, camDir);

                if (rightAngle <= 90)       //在右側
                {
                    /*************配合動畫************/
                    //若速度>0.1  或 <-0.1  且為第一次進入      紀錄開始推動時的時間    
                    if ((inputHor > 0.1 || inputHor < -0.1) && isFirstIn)
                    {
                        pushTime = Time.time;
                        isFirstIn = false;
                    }
                    //Debug.Log("T-P="+(Time.time - pushTime)+"T="+Time.time+"p="+pushTime);

                    //若推動時間小於1秒  停止移動
                    if (Time.time - pushTime < 1f)
                    {
                        moveDirection = transform.TransformDirection(new Vector3(0, 0, 0));
                        //Debug.Log(pushTime);
                    }
                    else
                    {
                        moveDirection = transform.TransformDirection(new Vector3(0, 0, inputHor));
                    }
                    /*************配合動畫************/




                    //控制FORWARD 變數
                    if (inputHor > 0.1) animateController.addForward();
                    else if (inputHor < -0.1) animateController.minusForward();
                    else animateController.setToZero();
                    //控制FORWARD 變數
                    //Debug.Log("right");
                }
                else                        //在左側
                {
                    /*************配合動畫************/
                    //若速度>0.1  或 <-0.1  且為第一次進入      紀錄開始推動時的時間    
                    if ((inputHor * -1 > 0.1 || inputHor * -1 < -0.1) && isFirstIn)
                    {
                        pushTime = Time.time;
                        isFirstIn = false;
                    }
                    //Debug.Log("T-P="+(Time.time - pushTime)+"T="+Time.time+"p="+pushTime);

                    //若推動時間小於1秒  停止移動
                    if (Time.time - pushTime < 1f)
                    {
                        moveDirection = transform.TransformDirection(new Vector3(0, 0, 0));
                        //Debug.Log(pushTime);
                    }
                    else
                    {
                        moveDirection = transform.TransformDirection(new Vector3(0, 0, inputHor * -1));
                    }
                    /*************配合動畫************/




                    //控制FORWARD 變數
                    if (inputHor * -1 > 0.1) animateController.addForward();
                    else if (inputHor * -1 < -0.1) animateController.minusForward();

                    else animateController.setToZero();
                    //控制FORWARD 變數
                    Debug.Log("left");


                }
            }
            else                            //在後方
            {
                /*************配合動畫************/
                //若速度>0.1  或 <-0.1  且為第一次進入      紀錄開始推動時的時間    
                if ((inputVer > 0.1 || inputVer < -0.1) && isFirstIn)
                {
                    pushTime = Time.time;
                    isFirstIn = false;
                }
                //Debug.Log("T-P="+(Time.time - pushTime)+"T="+Time.time+"p="+pushTime);

                //若推動時間小於1秒  停止移動
                if (Time.time - pushTime < 1f)
                {
                    moveDirection = transform.TransformDirection(new Vector3(0, 0, 0));
                    //Debug.Log(pushTime);
                }
                else
                {
                    moveDirection = transform.TransformDirection(new Vector3(0, 0, inputVer));
                }
                /*************配合動畫************/




                //控制FORWARD 變數
                if (inputVer > 0.1) animateController.addForward();
                else if (inputVer < -0.1) animateController.minusForward();
                else animateController.setToZero();
                //控制FORWARD 變數
                Debug.Log("backward");

            }

            moveDirection *= charSpeed;
        }
        //重置FORWARD
        else animateController.setToZero();


        moveDirection.y -= gravity * Time.deltaTime;
        charController.Move(moveDirection * Time.deltaTime);
    }




}
