using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一般變數: 開頭小寫，單字分隔開頭大寫 Ex:myFirstName

//一般函式(function):開頭小寫，單字分隔開頭大寫 Ex:myFirstFunc();

//常數:開頭底線+小寫，單字分隔開頭大寫 Ex:_myFirstName

//類別:開頭大寫，單字分隔開頭大寫 Ex:class MyFirstFamily { };

public class TestPlayerController : MonoBehaviour
{
    //人物的animateController
    private PlayerAnimateController animateController;
    //人物身上的freeLookCam攝影機
    private CinemachineFreeLook freeLookCam;

    /**********推移物件*********/
    private bool isPushingObject = false;
    GameObject pushedObject = null;  //推動中的物件
    /**********推移物件*********/

    /**********影子偵測*********/
    //你人是否站在影子上
    [SerializeField] private bool isInShadow = false;
    //光源的陣列
    private List<GameObject> lights = new List<GameObject>();
    //一個光源對一個物件所製造出的影子  陣列
    private Dictionary<GameObject, GameObject> lightsWithShadows = new Dictionary<GameObject, GameObject>();
    /**********影子偵測*********/


    /**********潛入影子*********/
    //你人是否"進入"影子內
    [SerializeField] private bool isShadowing = false;
    //人物身上的mesh物件陣列
    private List<GameObject> meshs = new List<GameObject>();
    //水圈圈粒子物件
    [SerializeField] private GameObject ripple;
    //按著E 進入影子的延遲
    private float delayCount = 0;
    /**********潛入影子*********/


    /**********潛行移動*********/
    // 是否爬牆
    private bool isClimbing = false;
    // 飛行延遲
    private int shadowOutCount = 0;




    
    /**********潛行移動*********/


    /**********物理性質*********/
    private float charSpeed = 6.0f;
    private float charJumpSpeed = 8.0f;
    private float gravity = 20.0f;
    /**********物理性質*********/


    //人物移動方向
    private Vector3 moveDirection = Vector3.zero;
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
    /********鍵鼠操控變數*******/


    // update
    private Vector3 dir = Vector3.zero;
    private Transform shadowOwner;
    private Transform shadowOwnerLight;
    private Vector3 shadowPos = Vector3.zero;
    private Vector3 shadowMoveDir = Vector3.zero;
    [SerializeField] private Transform showPosCube;
    private bool isWall;

    // Start is called before the first frame update
    void Start()
    {
        //airWall = GameObject.Find("AirWall").transform;

        //載入人物操控
        charController = GetComponent<CharacterController>();

        //載入所有光源
        findAllLightsInScene();
        //載入人物身上所有mesh物件
        findAllMeshsInScene();
        //取得animateController
        animateController = GetComponent<PlayerAnimateController>();
        //取得freelook攝影機，要用名字找太暴力，所以用掛的比較好
        if (freeLookCam == null)
        {
            freeLookCam = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();
        }
        //取得ripple物件，潛入影子後的特效
        if (ripple == null)
        {
            ripple = transform.Find("Ripple").gameObject;
        }

        
    }

    // Update is called once per frame
    void Update()
    {

        //物件推移
        Ray rayObject = new Ray(transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.forward); //用於判斷前方是否有可推動物體
        //Debug.DrawRay(transform.position + new Vector3(0.0f, 0.5f, 0.0f), transform.forward,Color.green);
        RaycastHit hit;

        // 偵測影子
        shadowDetect();
        // 印出你踩在哪個影子上
        //printWhatShadowsIn();


        /**********潛入影子*********/
        if (charController.isGrounded)
        {
            if (Input.GetKey(KeyCode.E) && !isShadowing && isInShadow && !isPushingObject)
            {
                if (Time.time - delayCount > 0.20f)
                {
                    animateController.jumpIntoShadow();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && isInShadow && charController.isGrounded && !isPushingObject)
            {
                isShadowing = !isShadowing;
                transformToShadow();
                delayCount = Time.time;
            }
        }
        /**********潛入影子*********/

        /**********推移物品*********/
        if (Physics.Raycast(rayObject, out hit, 1.2f))
        {
            //擊中Movable物件 且在地面 且目前沒有推動物件 且不在影子狀態中 才可以推動物體
            if (hit.transform.tag == ("Movable") && Input.GetKeyDown(KeyCode.F) && charController.isGrounded && isPushingObject == false && !isShadowing)
            {
                //靠太進  讓自己往後退
                float distance = Vector3.Distance(hit.point, this.transform.position);
                if (distance < 0.8) transform.position -= transform.forward / 4;

                hit.transform.GetComponent<FixedJoint>().connectedBody = this.GetComponent<Rigidbody>();        //連接物體
                pushedObject = hit.transform.gameObject;                                         //紀錄物體
                isPushingObject = true;
            }
            else if (hit.transform.tag == ("Movable") && Input.GetKeyDown(KeyCode.F) && charController.isGrounded && isPushingObject == true && !isShadowing)   //再次點擊 取消
            {
                if (pushedObject != null) pushedObject.GetComponent<FixedJoint>().connectedBody = null;
                pushedObject = null;
                isPushingObject = false;
            }
            else if (!charController.isGrounded)                                                            //離開地面時取消
            {
                if (pushedObject != null) pushedObject.GetComponent<FixedJoint>().connectedBody = null;
                pushedObject = null;
                isPushingObject = false;
            }
        }
        else
        {
            if (pushedObject != null) pushedObject.GetComponent<FixedJoint>().connectedBody = null;
            pushedObject = null;
            isPushingObject = false;
        }
        /**********推移物品*********/

        if (isShadowing)
        {
            //潛行後移動的模組
            freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
            shadowMove();
        }
        else
        {
            //移動模組
            freeLookCam.m_RecenterToTargetHeading.m_enabled = true;
            move();
        }



        // update

        
    }
    private void shadowDetect()
    {
        isInShadow = false;
        Vector3 playerPos = transform.position;
        playerPos.y += 0.5f;
        for (int i = 0; i < lights.Count; i++)
        {
            float distance = 0.0f;
            Light lightCompnent = lights[i].GetComponent<Light>();

            if (lightCompnent.type.ToString() == "Directional")
            {
                //Debug.Log("Directional Light");
                // 太陽位置設定
                // 假設太陽距離(很遠)
                float sunDis = 10000.0f;
                Vector3 sunPos = lights[i].transform.rotation * new Vector3(0.0f, 0.0f, -sunDis);

                // 人和太陽實際距離
                distance = Vector3.Distance(sunPos, playerPos);                

                // ray 設定
                // ray 起點 => 太陽位置， 方向 => 玩家位置 - 太陽位置
                Ray ray = new Ray(sunPos, (playerPos - sunPos));
                RaycastHit hit;
                Debug.DrawRay(ray.origin, ray.direction, Color.red);

                // 光線擋到物體不可以是玩家
                if (Physics.Raycast(ray, out hit, distance) && hit.transform != transform)
                {
                    //Debug.Log("Directional light make you in shadow");
                    if (lightsWithShadows[lights[i]] != hit.transform.gameObject)
                    {
                        lightsWithShadows[lights[i]] = hit.transform.gameObject;
                    }
                    isInShadow = true;
                }
                else
                {
                    lightsWithShadows[lights[i]] = null;
                }                
            }
            else
            {
                distance = Vector3.Distance(lights[i].transform.position, playerPos);
                // Point light 的判定
                if (lightCompnent.type.ToString() == "Point")
                {
                    Ray ray = new Ray(lights[i].transform.position, (playerPos - lights[i].transform.position));
                    RaycastHit hit;
                    Debug.DrawRay(ray.origin, ray.direction, Color.red);
                    //Debug.Log("SpotLight");
                    // 判定有沒有在光線範圍內
                    // 判定光線有沒被物體檔到
                    // 光線擋到物體不可以是玩家
                    if (distance <= lightCompnent.range && Physics.Raycast(ray, out hit, distance) && hit.transform != transform)
                    {
                        if (lightsWithShadows[lights[i]] != hit.transform.gameObject)
                        {
                            lightsWithShadows[lights[i]] = hit.transform.gameObject;
                        }
                        isInShadow = true;
                    }
                    else
                    {
                        lightsWithShadows[lights[i]] = null;
                    }
                }
                // Spot light 的判定 
                else
                {
                    Ray ray = new Ray(lights[i].transform.position, (playerPos - lights[i].transform.position));
                    RaycastHit hit;
                    Debug.DrawRay(ray.origin, ray.direction, Color.red);
                    Vector3 dir = playerPos - lights[i].transform.position;
                    float angle = Vector3.Angle(dir, lights[i].transform.forward);
                    // 判定有沒有在光線範圍內
                    // 判定光線有沒有被物體檔到
                    // 光線擋到物體不可以是玩家
                    if (distance <= lightCompnent.range && angle <= lightCompnent.spotAngle / 2 && Physics.Raycast(ray, out hit, distance) && hit.transform != transform)
                    {
                        //Debug.Log("Spot light make you in shadow");
                        if (lightsWithShadows[lights[i]] != hit.transform.gameObject)
                        {
                            lightsWithShadows[lights[i]] = hit.transform.gameObject;
                        }
                        isInShadow = true;
                    }
                    else
                    {
                        lightsWithShadows[lights[i]] = null;
                    }
                }
            }
            //Debug.Log(lights[i].name);
        }
    }


    










    // update
    private void shadowObjectLocalPos()
    {
        Vector3 lightPos = Vector3.zero;
        float dis = 0;
        if (shadowOwnerLight.GetComponent<Light>().type.ToString() == "Directional")
        {
            //Debug.Log("Directional Light");
            // 太陽位置設定
            // 假設太陽距離(很遠)
            float sunDis = 10000.0f;
            lightPos = shadowOwnerLight.rotation * new Vector3(0.0f, 0.0f, -sunDis);
            // 常數
            dis = 100;
        }
        else
        {            
            dis = shadowOwnerLight.GetComponent<Light>().range - Vector3.Distance(lightPos, shadowOwner.position); 
        }
        Ray ray = new Ray(shadowOwner.position, shadowOwner.position - lightPos);                     
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, dis))
        {
            if(hit.transform != transform)
            {                
                shadowMoveDir = shadowPos - hit.point;
                shadowPos = hit.point;
                             
            }

        }
        if (isInShadow)
        {
            dir = transform.position - shadowPos;
        }

    }




















    // update
    private void setShadowsGameObject()
    {
        foreach (KeyValuePair<GameObject, GameObject> i in lightsWithShadows)
        {
            if (i.Value != null)
            {

                shadowOwner = i.Value.transform;
                shadowOwnerLight = i.Key.transform;                                
            }
        }        
    }
    // update
    private void shadowMove()
    {
        showPosCube.position = transform.position;
        if (isInShadow)
        {
            setShadowsGameObject();
        }

        shadowObjectLocalPos();
        //水平鍵(A,D)有按與否
        inputHor = Input.GetAxis("Horizontal");
        //垂直鍵(W,S)有按與否
        inputVer = Input.GetAxis("Vertical");
        //滑鼠水平(X軸)移動
        mouseX = Input.GetAxis("Mouse X");
        //滑鼠垂直(Y軸)移動
        mouseY = Input.GetAxis("Mouse Y");

        Ray rayForward = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), transform.forward);
        Ray rayRight = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), transform.right);
        Ray rayLeft = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), -transform.right);
        Ray rayBack = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), -transform.forward);
        RaycastHit hit;
        isWall = false;

        bool climbLeft = false;
        bool climbRight = false;
        bool climbForward = false;
        bool climbBack = false;

        // 避免出錯 都先初始化
        gravity = 20;
        // 避免滑行問題
        moveDirection = new Vector3(0.0f, 0.0f, 0.0f);

        // 偵側牆壁 前方
        if (Physics.Raycast(rayForward, out hit, 1.0f))
        {
            if (hit.transform != transform)
            {
                isWall = true;
                climbForward = true;
            }
        }
        // 偵側牆壁 後方
        if (Physics.Raycast(rayBack, out hit, 1.0f))
        {
            if (hit.transform != transform)
            {
                isWall = true;
                climbBack = true;
            }
        }
        // 偵側牆壁 右方
        if (Physics.Raycast(rayRight, out hit, 1.0f))
        {
            if (hit.transform != transform)
            {
                isWall = true;
                climbRight = true;
            }
        }
        // 偵側牆壁 左方
        if (Physics.Raycast(rayLeft, out hit, 1.0f))
        {
            if (hit.transform != transform)
            {
                isWall = true;
                climbLeft = true;
            }
        }
        // 避免斜坡之後人整個飄在天上
        if ((isClimbing && !isWall) || !isInShadow) 
        {
            if (!isInShadow)
            {
                shadowOutCount += 10;
            }
            else
            {
                shadowOutCount++;
            }
            
            
        }
        else
        {
            shadowOutCount = 0;
        }

        // 移動
        if (inputHor != 0 || inputVer != 0)
        {
            //以camera LookAt pos與camera本身pos的向量 更改角色forward方向
            if (((mouseX != 0 || mouseY != 0) && inputHor != 0) || inputVer != 0)
            {
                Vector3 camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
                camFor.y = 0.0f;
                transform.forward = camFor;//Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // 移動方向
            moveDirection = transform.TransformDirection(new Vector3(inputHor, 0, inputVer)/*.normalized*/);

            // 爬牆中
            if (isWall)
            {
                gravity = 0;
                // 爬牆中三個方向的位移量
                float dirZ = inputVer;
                float dirX = inputHor;
                float dirY = 0;

                // 如果前方OR後方有牆
                if ((climbForward || climbBack) && inputVer != 0)
                {
                    dirY += inputVer;
                    if (climbBack)
                    {
                        dirY *= -1;

                    }
                }
                // 又如果左方OR右方有牆
                if ((climbLeft || climbRight) && inputHor != 0)
                {
                    dirY += inputHor;
                    if (climbLeft)
                    {
                        dirY *= -1;
                    }
                }
                if (dirY >= 1)
                {
                    dirY = 1;
                }
                // 確定有牆&&同時開始爬 (離地) 了
                if (!charController.isGrounded)
                {
                    isClimbing = true;
                    //(離地時 要將方向鎖定在上下而已)
                    if (climbForward || climbBack)
                    {
                        dirZ = 0;
                    }
                    //(離地時 要將方向鎖定在上下而已)
                    else if (climbLeft || climbRight)
                    {
                        dirX = 0;
                    }

                }
                // 新的移動向量
                moveDirection = transform.TransformDirection(new Vector3(dirX, dirY, dirZ)/*.normalized*/);

            }
            moveDirection *= charSpeed;            
        }
        

        moveDirection.y -= gravity * Time.deltaTime;



        if (!isInShadow && shadowMoveDir == Vector3.zero) 
        {
            transform.position = shadowPos + dir;
        }
        else
        {
            charController.Move(moveDirection * Time.deltaTime);            
        }

        //退出影子條件
        if (/*!isInShadow || */shadowOutCount >= 20 || (!isClimbing && (!charController.isGrounded && !isWall)) || Input.GetKeyDown(KeyCode.E))
        {
            shadowOutCount = 0;
            isShadowing = false;
            isClimbing = false;
            isWall = false;
            transformToShadow();
            gravity = 20;
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

        if (!isInShadow && isShadowing)
        {
            isShadowing = false;
            transformToShadow();
        }

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
                moveDirection.y = charJumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        //給予重力
        moveDirection.y -= gravity * Time.deltaTime;
        charController.Move(moveDirection * Time.deltaTime);
    }

    
    public void transformToShadow()
    {
        const float _newRigsHeight = 1.5f;
        const float _newRigsRadius = 6.0f;

        // 調整攝影機位置
        if (isShadowing)
        {
            animateController.jumpIntoShadow();
            ripple.GetComponent<ParticleSystem>().Play();
            freeLookCam.LookAt = transform;
            // freeLookCam.m_Orbits[0] = top
            // freeLookCam.m_Orbits[1] = mid
            // freeLookCam.m_Orbits[2] = bot
            freeLookCam.m_Orbits[1].m_Height = _newRigsHeight;
            freeLookCam.m_Orbits[1].m_Radius = _newRigsRadius;
            freeLookCam.m_Orbits[2].m_Height = _newRigsHeight;
            freeLookCam.m_Orbits[2].m_Radius = _newRigsRadius;
            setShadowsGameObject();
        }
        else
        {
            animateController.jumpOutOfShadow();
            ripple.GetComponent<ParticleSystem>().Stop();
            freeLookCam.LookAt = transform.GetChild(2);
            // freeLookCam.m_Orbits[0] = top
            // freeLookCam.m_Orbits[1] = mid
            // freeLookCam.m_Orbits[2] = bot
            freeLookCam.m_Orbits[1].m_Height = 2.5f;
            freeLookCam.m_Orbits[1].m_Radius = 3.0f;
            freeLookCam.m_Orbits[2].m_Height = 0.8f;
            freeLookCam.m_Orbits[2].m_Radius = 1.3f;
            shadowOwner = null;
        }
        // 把所有mesh物件關掉/打開
        for (int i = 0; i < meshs.Count; i++)
        {
            if (meshs[i].GetComponent<MeshRenderer>())
            {
                meshs[i].GetComponent<MeshRenderer>().enabled = !isShadowing;
            }
            else
            {
                meshs[i].GetComponent<SkinnedMeshRenderer>().enabled = !isShadowing;
            }
        }
    }



    //潛入影子動畫結束
    public void playerControllerJIS()
    {

        isShadowing = isInShadow;
        transformToShadow();



        //想做啥
        //transformToShadow();
    }
    private void findAllLightsInScene()
    {
        Light[] lightArr = FindObjectsOfType(typeof(Light)) as Light[];
        foreach (Light light in lightArr)
        {
            lights.Add(light.gameObject);
            lightsWithShadows.Add(light.gameObject, null);
        }
    }
    /// <summary>
    /// 印出你在哪個影子內
    /// </summary>
    private void printWhatShadowsIn()
    {
        foreach (KeyValuePair<GameObject, GameObject> i in lightsWithShadows)
        {
            if (i.Value != null)
            {
                Debug.Log("you are in " + i.Value.transform.name + " 's shadow");
            }
        }
    }
    /// <summary>
    /// 找出人物身上所有的meshs
    /// </summary>
    private void findAllMeshsInScene()
    {
        MeshFilter[] meshsFilter = GetComponentsInChildren<MeshFilter>();
        const string _kachujinMesh = "Kachujin";
        meshs.Add(transform.Find(_kachujinMesh).gameObject);
        for (int i = 0; i < meshsFilter.Length; i++)
        {
            meshs.Add(meshsFilter[i].gameObject);
        }
    }

    /// <summary>
    /// 影子移動    
    /// </summary>

   

    


}