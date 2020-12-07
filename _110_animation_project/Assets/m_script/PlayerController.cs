using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一般變數: 開頭小寫，單字分隔開頭大寫 Ex:myFirstName

//一般函式(function):開頭小寫，單字分隔開頭大寫 Ex:myFirstFunc();

//常數:開頭底線+小寫，單字分隔開頭大寫 Ex:_myFirstName

//類別:開頭大寫，單字分隔開頭大寫 Ex:class MyFirstFamily { };

public class PlayerController : MonoBehaviour
{
    //人物的animateController
    private PlayerAnimateController animateController;
    //人物身上的freeLookCam攝影機
    [SerializeField] private CinemachineFreeLook freeLookCam;
    //main camera
    [SerializeField]
    private Camera mainCam = null;

    /**********繩索射出*********/
    [SerializeField]
    private GameObject crossbowInHand;       //角色手中的十字弓   也會用此物件位置  偵測與目標物之間是否有阻擋物
    [SerializeField]
    private GameObject crossbowAside;        //角色側面的十字弓

    private GameObject shootingTarget;       //射擊的物件     由crossbowTargeting判定    找到後  將傳遞給ropeDrawLine.crossbowShootSrart
    private GameObject tempShootTarget;      //儲存玩家瞄準的物件     避免在做動畫途中換動鏡頭導致無法成功射出       在tiedRopeAnimationEnd重置
    private GameObject[] allShootingTargetArray;//所有十字弓射擊目標的物件
    private Transform tiedObjectInRange = null;//是否有TAG 為Rope_Tied_Object的物件在角色周圍
    private Rope_DrawLine ropeDrawLine = null;
    private float lastTargetDistance = 0;   //紀錄之前可射擊物體距離  當前物體的距離需大於之前可射擊物體才可取代   目的是避免重疊物體  crossbowTargeting取用  在tiedRopeAnimationEnd歸0
    private bool isAbleToShoot = false;
    public bool IsAbleToShoot { get { return isAbleToShoot; } set { isAbleToShoot = value; } }    //角色是否可以進行射箭    ableToShoot() 進行調整
    private bool isShooting = false;        //角色是否正在射擊      避免同時射擊多個目標
    public bool IsShooting { get { return isShooting; } set { isShooting = value; } }
    /**********繩索射出*********/

    /************戰鬥***********/
    //
    public bool Isblocking { get { return isblocking; } set { isblocking = value; } }    //角色是否正在防禦
    private bool isblocking = false;        //角色是否正在防禦
    /************戰鬥***********/

    /************暗殺***********/
    private AssassinModule assassinModule = null;
    /************暗殺***********/

    /**********物理性質*********/
    [SerializeField] private float charSpeed = 6.0f;
    [SerializeField] private float charJumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;
    /**********物理性質*********/
    //是否可以移動
    private bool isMovable = true;
    public bool IsMovable { get { return isMovable; } set { isMovable = value; } }

    //人物移動方向
    private Vector3 moveDirection = Vector3.zero;
    public Vector3 MoveDirection { get { return moveDirection; } set { moveDirection = value; } }

    ////人物碰撞體
    //private Collider charCollider;
    //人物轉向(目標)向量
    private Quaternion targetRotation;
    //人物操控
    private CharacterController charController;

    /********鍵鼠操控變數*******/
    //水平鍵(A,D)有按與否
    private float inputHor;
    //垂直鍵(W,S)有按與否
    private float inputVer;
    //滑鼠水平(X軸)移動
    private float mouseX;
    //滑鼠垂直(Y軸)移動
    private float mouseY;
    //跳躍間隔
    float jumpTimeOffSet = -10;
    /********鍵鼠操控變數*******/


    private ShadowModule shadowModule;
    private Push_Module pushModule;
    private ThrowItemsModule throwModule;

    // Start is called before the first frame update
    void Start()
    {

        //載入人物操控
        charController = GetComponent<CharacterController>();

        //影子模組初始化
        shadowModule = GetComponent<ShadowModule>();
        shadowModule.init();

        //推移物件初始化
        pushModule = GetComponent<Push_Module>();

        //丟東西模組
        throwModule = GetComponent<ThrowItemsModule>();

        //取得animateController
        animateController = GetComponent<PlayerAnimateController>();
        /**********繩索射出*********/
        //取得Rope_DrawLine
        if (ropeDrawLine == null) ropeDrawLine = GameObject.Find("Crossbow_rope_start").GetComponent<Rope_DrawLine>();
        if (crossbowInHand == null) crossbowInHand = GameObject.Find("Crossbow_in_hand");
        if (crossbowAside == null) crossbowAside = GameObject.Find("Crossbow_on_side");
        //取得所有十字弓射擊目標的物件
        if (allShootingTargetArray == null)
        {
            allShootingTargetArray = GameObject.FindGameObjectsWithTag("Crossbow_Target");
            if (allShootingTargetArray == null) Debug.Log("Crossbow does not have any available target");
        }
        /**********繩索射出*********/
        //取得freelook攝影機，要用名字找太暴力，所以用掛的比較好
        if (freeLookCam == null)
        {
            freeLookCam = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();
        }
        //取得主攝影機
        if (mainCam == null)
        {
            mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        }


        // 取得暗殺模組
        assassinModule = GetComponentInChildren<AssassinModule>();


    }
    // Update is called once per frame
    void Update()
    {
        //物件推移
        Ray rayObject = new Ray(transform.position + new Vector3(0.0f, 1.25f, 0.0f), transform.forward); //用於判斷前方是否有可推動物體
        //Debug.DrawRay(transform.position + new Vector3(0.0f, 1.25f, 0.0f), transform.forward*1.5f,Color.green);
        RaycastHit hit;

        //繩索射出  設置RAYCAST   用於探測是否有物體在玩家與物體間
        Ray rayBeforeShoot;
        RaycastHit beforeShootHit;

        // 偵測影子
        shadowModule.shadowDetect();
        // 印出你踩在哪個影子上
        //printWhatShadowsIn();


        /**********潛入影子*********/
        if (charController.isGrounded && shadowModule.IsInShadow && !shadowModule.IsLighted && !pushModule.IsPushingObject && !throwModule.IsTakingAim)
        {
            if (Input.GetKeyDown(KeyCode.E) && !shadowModule.IsShadowing)
            {
                animateController.jumpIntoShadow();
            }
        }
        /**********潛入影子*********/


        /**********繩索射出*********/
        if (Input.GetKeyDown(KeyCode.F) && charController.isGrounded && !pushModule.IsPushingObject && isAbleToShoot && tiedObjectInRange != null && shootingTarget != null && IsShooting == false && !throwModule.IsTakingAim)
        {
            rayBeforeShoot = new Ray(crossbowInHand.transform.position, shootingTarget.transform.position - crossbowInHand.transform.position);
            Debug.DrawLine(crossbowInHand.transform.position, shootingTarget.transform.position, Color.red);
            //設置RAYCAST   用於探測是否有物體在玩家與物體間

            if (Physics.Raycast(rayBeforeShoot, out beforeShootHit, Vector3.Distance(crossbowInHand.transform.position, shootingTarget.transform.position)))
            {
                Debug.Log(beforeShootHit.transform);
                if (beforeShootHit.transform.tag == ("Crossbow_Target"))
                {
                    tempShootTarget = shootingTarget;           //在按下F時  直接紀錄目標物件
                    crossBowShoot();
                }
            }

        }
        crossbowTargeting();
        /**********繩索射出*********/


        if (!isMovable)
        {
            //不可移動時
            /*moveDirection.y -= gravity * Time.deltaTime;
            charController.Move(moveDirection * Time.deltaTime);*/
        }
        else if (shadowModule.IsShadowing)
        {
            //潛行後移動的模組
            freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
            animateController.notPushingObject();                                              //呼叫此函式  更改ANIMATION中的BOOL值
            shadowModule.shadowMove();
        }
        else if (pushModule.IsPushingObject)
        {
            //推移物品移動模組
            freeLookCam.m_RecenterToTargetHeading.m_enabled = false;
            pushingObjectAnimation();                                              //呼叫此函式  更改ANIMATION中的BOOL值
            pushModule.dragMove();
        }
        else if (throwModule.IsTakingAim)
        {

        }
        else if (assassinModule.IsAssassinReady)
        {

        }
        else
        {
            //移動模組
            freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
            animateController.notPushingObject();                                              //呼叫此函式  更改ANIMATION中的BOOL值
            move();
        }
    }

    /// <summary>
    /// 移動
    /// </summary>
    public void move()
    {
        //水平鍵(A,D)有按與否
        inputHor = Input.GetAxis("Horizontal");
        //垂直鍵(W,S)有按與否
        inputVer = Input.GetAxis("Vertical");
        //滑鼠水平(X軸)移動
        mouseX = Input.GetAxis("Mouse X");
        //滑鼠垂直(Y軸)移動
        mouseY = Input.GetAxis("Mouse Y");
        //角色在落地時啟動
        if (charController.isGrounded)
        {
            // 滑鼠 有動 與 方向鍵 有按著的時候才會啟動
            if ((mouseX != 0 || mouseY != 0) && (inputHor != 0) || inputVer != 0)
            {
                //以freeLookCam pos與freeLookCam本身pos的向量 更改角色forward方向
                Vector3 camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;

                //Debug.Log("Camera.LookAt.position : "+ Camera.LookAt.position);
                //Debug.Log("Camera.transform.position : " + Camera.transform.position);
                //Debug.Log("camFor : " + camFor);
                camFor.y = 0.0f;
                //Debug.Log("camFor : "+ camFor);
                targetRotation = Quaternion.LookRotation(camFor, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f/*每一frame轉向 5.0 度*/);
            }
            // We are grounded, so recalculate
            // move direction directly from axes       
            //前進方向local coord.轉world coord.
            moveDirection = transform.TransformDirection(new Vector3(inputHor, 0, inputVer)/*.normalized*/);
            //以charSpeed 的速率前進
            moveDirection *= charSpeed;

            //按空白鍵時啟動
            if (Input.GetButton("Jump"))
            {
                if (Time.time - jumpTimeOffSet >= 0.75)
                {
                    jumpTimeOffSet = Time.time;
                    moveDirection.y = charJumpSpeed;
                }

                
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        //給予重力
        moveDirection.y -= gravity * Time.deltaTime;
        charController.Move(moveDirection * Time.deltaTime);
    }


    public void stopPushing()   //會被pushable_Item調用  用於停止推動狀態
    {
        pushModule.IsPushingObject = false;
    }

    /// <summary>
    /// 攻擊
    /// </summary>
    public void attack()
    {

    }

    /// <summary>
    /// 防守
    /// </summary>
    public void defend()
    {

    }

    /// <summary>
    /// 閃躲
    /// </summary>
    public void dodge()
    {

    }

    //潛入影子動畫結束
    public void playerControllerJIS()
    {
        shadowModule.transformToShadow(true);
        if (!shadowModule.IsInShadow)
        {
            //shadowModule.IsShadowing = false;
            shadowModule.transformToShadow(false);
        }

        //想做啥
        //transformToShadow();
    }


    
    public void assassinReady()
    {
        assassinModule.assassinReady();
    }

    public void assassinFinish()
    {
        assassinModule.assassinFinish();
    }

    /// <summary>
    /// 丟東西
    /// </summary>
    public void throwItem()
    {

    }

    /**********玩家格檔*********/
    void player_blocking()  //防禦
    {
        //  gameObject.BroadcastMessage("activate_block");
        Isblocking = true;
    }
    /**********玩家格檔*********/

    /**********繩索射出*********/
    public void shootAnimationEnd()
    {
        crossbowInHand.GetComponent<MeshRenderer>().enabled = false;
        crossbowInHand.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        crossbowAside.GetComponent<MeshRenderer>().enabled = true;
        crossbowAside.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

        ropeDrawLine.ropeInHand();

        if (tiedObjectInRange == null) Debug.Log("nothing is in tied range");
        else
        {
            Quaternion targetRotation = Quaternion.LookRotation(tiedObjectInRange.transform.position - this.transform.position, Vector3.up);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = targetRotation;
        }


    }

    //獲取角色射出的LINERENDERER的ARRAY  並傳遞至tiedObjectInRange(在範圍內的綁繩子之物件)   且重置lastTargetDistance為0
    public void tiedRopeAnimationEnd()
    {
        tiedObjectInRange.GetComponent<Rope_Tied_Object>().getLineRendererPoints(ropeDrawLine.ropeTiedToObject());
        lastTargetDistance = 0;
        tempShootTarget = null;
        IsShooting = false;
        charController.enabled = true;
    }


    //配合動畫  取消側邊十字弓的MESH  與啟用手中十字弓的MESH 
    public void takingCrossBow()
    {
        //啟用手中十字弓的MESH   會再shootAnimationEnd中停用
        crossbowInHand.GetComponent<MeshRenderer>().enabled = true;
        crossbowInHand.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

        //停用側邊十字弓的MESH   
        crossbowAside.GetComponent<MeshRenderer>().enabled = false;
        crossbowAside.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        ropeDrawLine.crossbowInHand();
    }

    //在綁繩的固體物件中的Rope_Tied_Manager_Script中調用   代表進入綁繩物體範圍  並且接收該物件
    public void ableToShoot(Transform inRangeTarget)
    {
        tiedObjectInRange = inRangeTarget;
        isAbleToShoot = true;
    }

    //在綁繩的固體物件中的Rope_Tied_Manager_Script中調用   代表離開綁繩物體範圍
    public void unAbleToShoot()
    {
        tiedObjectInRange = null;
        isAbleToShoot = false;
    }

    //讓動畫開始到一定程度再撥放繩子效果     
    public void crossbowShootSrart()
    {
        ropeDrawLine.getDestination(tempShootTarget);
        ropeDrawLine.crossbowShootRope();
    }

    //計算十字弓與目標的角度  並傳遞
    public void crossBowShoot()
    {
        IsShooting = true;
        float distanceBetweenTarget = 0;
        float y = 0;
        charController.enabled = false;

        if (tempShootTarget != null)
        {
            Vector3 tempRotation = new Vector3(tempShootTarget.transform.position.x - ropeDrawLine.transform.position.x, 0, tempShootTarget.transform.position.z - ropeDrawLine.transform.position.z);
            this.transform.rotation = Quaternion.LookRotation(tempRotation, Vector3.up);
            distanceBetweenTarget = Vector3.Distance(new Vector3(ropeDrawLine.transform.position.x, 0, ropeDrawLine.transform.position.z), new Vector3(tempShootTarget.transform.position.x, 0, tempShootTarget.transform.position.z));
            y = tempShootTarget.transform.position.y - ropeDrawLine.transform.position.y;
            //Debug.Log("ropeDrawLine=" + ropeDrawLine.transform.position.y);
            //Debug.Log("tempShootTarget=" + tempShootTarget.transform.position.y);
            //Debug.Log("angleBetweenTarget=" + angleBetweenTarget);
        }
        //tempShootTarget (ropeDrawLine.transform.position);
        animateController.playShootCrossbowAnimation(distanceBetweenTarget, y);
    }


    //使玩家藉由MainCamera瞄準欲射擊的物件 取得所有可被射擊的物件後進行判斷  會隨著距離改變所需要的精度
    public void crossbowTargeting()
    {
        bool foundTarget = false;           //判斷在範圍內是否有找到物件    用於重製shootingTarget為NULL
        lastTargetDistance = 0;
        if (allShootingTargetArray != null)
        {
            for (int i = 0; i < allShootingTargetArray.Length; i++)
            {
                Vector3 screenPos = mainCam.WorldToScreenPoint(allShootingTargetArray[i].transform.position);
                float targetDis = Vector3.Distance(this.transform.position, allShootingTargetArray[i].transform.position);
                if (targetDis <= 25)
                {
                    if (screenPos.x >= 350 && screenPos.x <= 750 && screenPos.y >= 50 && screenPos.y <= 450 && targetDis >= lastTargetDistance)
                    {
                        shootingTarget = allShootingTargetArray[i];
                        lastTargetDistance = targetDis;
                        foundTarget = true;
                    }
                }
                else if (targetDis <= 50)
                {
                    if (screenPos.x >= 450 && screenPos.x <= 650 && screenPos.y >= 150 && screenPos.y <= 300 && targetDis >= lastTargetDistance)
                    {
                        shootingTarget = allShootingTargetArray[i];
                        lastTargetDistance = targetDis;
                        foundTarget = true;
                    }
                }
                else if (targetDis <= 90)
                {
                    if (screenPos.x >= 500 && screenPos.x <= 600 && screenPos.y >= 200 && screenPos.y <= 250 && targetDis >= lastTargetDistance)
                    {
                        shootingTarget = allShootingTargetArray[i];
                        lastTargetDistance = targetDis;
                        foundTarget = true;
                    }
                }
                //Debug.Log("target is " + targetDis);
                //Debug.Log("target is " + screenPos.x + " pixels from the left");
                //Debug.Log("target is " + screenPos.y + " pixels from the bottom");
            }
        }
        else shootingTarget = null;

        if (foundTarget == false) shootingTarget = null;                //若範圍內沒找到物件  重置shootingTarget
    }
    /**********繩索射出*********/

    /**********推移物品*********/
    public void pushingObjectAnimation()
    {
        animateController.pushingObject();
    }
    /**********推移物品*********/

}