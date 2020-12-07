using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

public class ShadowModule : MonoBehaviour
{
    //人物的animateController
    private PlayerAnimateController animateController;
    //人物身上的freeLookCam攝影機
    [SerializeField] private CinemachineFreeLook freeLookCam;
    // minMapController
    [SerializeField]  private MiniMapController miniMap;
    // 敵人管理器
    [SerializeField] private EnemyManager enemyManager;

    //鏡頭濾鏡特效
    private PostProcessVolume ppv = null;

    

    /**********影子偵測*********/
    //你人是否站在影子上
    [SerializeField] private bool isInShadow = false;
    public bool IsInShadow { get { return isInShadow; } }
    //光源的陣列
    [SerializeField] private List<GameObject> lights = new List<GameObject>();
    //一個光源對一個物件所製造出的影子  陣列
    [SerializeField] private Dictionary<GameObject, GameObject> lightsWithShadows = new Dictionary<GameObject, GameObject>();
    /**********影子偵測*********/


    /**********潛入影子*********/
    //你人是否"進入"影子內
    [SerializeField] private bool isShadowing = false;
    public bool IsShadowing { get { return isShadowing; } set { isShadowing = value; } } 

    //人物身上的mesh物件陣列
    private List<GameObject> meshs = new List<GameObject>();
    //影子潛入物件
    [SerializeField] private GameObject ripple;
    //按著E 進入影子的延遲
    private float delayCount = 0;
    public float DelayCount { get { return delayCount; } set { delayCount = value; } }
    /**********潛入影子*********/


    /**********潛行移動*********/
    // 是否爬牆
    [SerializeField] private bool isClimbingVer = false;
    [SerializeField] private bool isClimbingHor = false;
    private Vector3 newForward = Vector3.zero;
    private Vector3 wallForward = Vector3.zero;
    // 出影子判定相關
    private int shadowOutCount = 0;
    private bool isInAir = false;
    private bool isWall = false;
    [SerializeField] private bool isLighted = false;
    public bool IsLighted { get { return isLighted; } set { isLighted = value; } }
    /**********影子邊界*********/
    private Vector3 dir = Vector3.zero;
    private Transform shadowOwner;
    private Transform shadowOwnerLight;
    private Vector3 shadowPos = Vector3.zero;
    private Vector3 shadowMoveDir = Vector3.zero;

    /**********影子邊界*********/

    /**********潛行移動*********/


    /**********物理性質*********/
    [SerializeField] private float charSpeed = 6.0f;
    [SerializeField] private float charJumpSpeed = 8.0f;
    [SerializeField] private float gravity = 20.0f;
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



    


    /********Debug用*******/
    private Vector3 testr;
    private Vector3 testg;
    private Vector3 testb;
    [SerializeField] private Transform testShowShadowPosCube;
    bool setForward = false;
    /********Debug用*******/

    // Start is called before the first frame update
    void Start()
    {

        //載入人物操控
        charController = GetComponent<CharacterController>();
        //取得animateController
        animateController = GetComponent<PlayerAnimateController>();
        //取得freelook攝影機，要用名字找太暴力，所以用掛的比較好
        if (freeLookCam == null)
        {
            freeLookCam = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();
        }

        // 取得 PostProcessVolume
        ppv = Camera.main.GetComponent<PostProcessVolume>();
        ppv.weight = 0;

        // 取得 miniMap Controller
        miniMap = FindObjectOfType<MiniMapController>();
        miniMap.setBlackPanel(false);
        
        // 取得 enemyManager
        enemyManager = FindObjectOfType<EnemyManager>();        
        enemyManager.setAllEnemiesOutline(0);

    }

    void FixedUpdate()
    {
        isLighted = false;
    }

    void LateUpdate()
    {
        if(isShadowing && (isLighted || isInShadow))
        {
            //shadowPos = transform.position;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
       /* if(isShadowing && ppv.weight < 1)
        {
            ppv.weight += Time.deltaTime;
            if (ppv.weight > 1)
            {
                ppv.weight = 1;
            }           
        }
        else if(!isShadowing && ppv.weight > 0)
        {
            ppv.weight -= Time.deltaTime;
            if (ppv.weight < 0)
            {
                ppv.weight = 0;
            }
        }*/
    }

    /// <summary>
    /// 潛入影子
    /// </summary>
    public void transformToShadow(bool isEnter)
    {
        const float _newRigsHeight = 1.5f;
        const float _newRigsRadius = 6.0f;



        // 調整攝影機位置
        if (isEnter == true) { 


            isShadowing = true;
            freeLookCam.LookAt = transform;

            // 顯示水波特效
            ripple.GetComponent<ParticleSystem>().Play();

            // 開啟小地圖黑背景
            miniMap.setBlackPanel(true);

            // 開啟敵人輪廓
            enemyManager.setAllEnemiesOutline(1);


            // freeLookCam.m_Orbits[0] = top
            // freeLookCam.m_Orbits[1] = mid
            // freeLookCam.m_Orbits[2] = bot
            freeLookCam.m_Orbits[1].m_Height = _newRigsHeight;
            freeLookCam.m_Orbits[1].m_Radius = _newRigsRadius;
            freeLookCam.m_Orbits[2].m_Height = _newRigsHeight;
            freeLookCam.m_Orbits[2].m_Radius = _newRigsRadius;


            charController.center = new Vector3(0.0f, 0.25f, 0.0f);
            charController.radius = 0.25f;
            charController.height = 0.0f;


            setShadowsGameObject();

            isClimbingVer = false;
            isClimbingHor = false;

        }
        else
        {
            isShadowing = false;
            animateController.jumpOutOfShadow();

            // 關閉水波特效
            ripple.GetComponent<ParticleSystem>().Stop();

            // 關閉小地圖黑背景
            miniMap.setBlackPanel(false);

            // 關閉敵人輪廓
            enemyManager.setAllEnemiesOutline(0);

            freeLookCam.LookAt = transform.GetChild(2);
            // freeLookCam.m_Orbits[0] = top
            // freeLookCam.m_Orbits[1] = mid
            // freeLookCam.m_Orbits[2] = bot
            freeLookCam.m_Orbits[1].m_Height = 2.5f;
            freeLookCam.m_Orbits[1].m_Radius = 3.0f;
            freeLookCam.m_Orbits[2].m_Height = 0.8f;
            freeLookCam.m_Orbits[2].m_Radius = 1.3f;


            charController.center = new Vector3(0.0f, 1.08f, 0.0f);
            charController.radius = 0.5f;
            charController.height = 2.0f;


            gravity = 20;
            shadowOwner = null;

            transform.rotation = Quaternion.Euler(0.0f, transform.rotation.eulerAngles.y, 0.0f);

            isClimbingVer = false;
            isClimbingHor = false;
        }
        // 把所有mesh物件關掉/打開
        for (int i = 0; i < meshs.Count; i++)
        {
            if((meshs[i] == ripple || meshs[i].tag == "MeshsDisabled") && !isShadowing)
            {
                continue;
            }

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

    /// <summary>
    /// 計時(當跑出shadow外的時候會現形)
    /// </summary>
    private void shadowOutCounter()
    {
        // 跑出影子外的計時  
        shadowOutCount += 10;
    }

    /// <summary>
    /// 重置 Counter
    /// </summary>
    private  void resetShadowOutCounter()
    {
        shadowOutCount = 0;
    }

    /// <summary>
    /// 影子移動    
    /// </summary>
    public void shadowMove()
    {
        //newForward = Vector3.zero;
        if (isInShadow)
        {
            setShadowsGameObject();
        }

        //shadowObjectLocalPos();
        //水平鍵(A,D)有按與否
        inputHor = Input.GetAxis("Horizontal");
        //垂直鍵(W,S)有按與否
        inputVer = Input.GetAxis("Vertical");
        //滑鼠水平(X軸)移動
        mouseX = Input.GetAxis("Mouse X");
        //滑鼠垂直(Y軸)移動
        mouseY = Input.GetAxis("Mouse Y");

        rayDetect();

        if (!isInShadow || isInAir)
        {
            shadowOutCounter();
        }
        else
        {
            resetShadowOutCounter();
        }
       


        // 移動
        if (inputHor != 0 || inputVer != 0)
        {

            //以camera LookAt pos與camera本身pos的向量 更改角色forward方向
            if (((mouseX != 0 || mouseY != 0) && inputHor != 0) || inputVer != 0)
            {

                Vector3 camFor = freeLookCam.LookAt.position - freeLookCam.transform.position;
                camFor.y = 0;
                if (!isClimbingVer && !isClimbingHor)
                {
                    transform.forward = camFor;
                }


                //Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            // 移動方向
            moveDirection = transform.TransformDirection(new Vector3(inputHor, 0, inputVer)/*.normalized*/);
            moveDirection *= charSpeed;
        }
        /*if (!isClimbingVer && !isClimbingHor)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }*/

        // 影子邊界 
        if ((!isInShadow || isLighted) && shadowMoveDir == Vector3.zero)
        {
            transform.position = shadowPos;

        }
        else
        {
            shadowPos = transform.position;
            charController.Move(moveDirection * Time.deltaTime);
        }

        RaycastHit hit;
        Ray groundRay = new Ray(transform.position, -transform.up);
        if (Physics.Raycast(groundRay, out hit, 1f))
        {
            isInAir = false;
            if (Vector3.Distance(hit.point, transform.position) >= 0.1)
            {
                transform.position = hit.point;
            }
        }

        //退出影子條件       
        if (isShadowing && /*!isInShadow || */shadowOutCount >= 50 /*|| (!isClimbing && (!charController.isGrounded && !isWall))*/ || Input.GetKeyDown(KeyCode.E))
        {

            shadowOutCount = 0;
            transformToShadow(false);
            gravity = 20;
            DelayCount = Time.time;
        }
        
    }

    /// <summary>
    /// 偵側牆壁的RayCast們    
    /// </summary>
    private void rayDetect()
    {
        isInAir = true;

        Ray rayForward = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), transform.forward);
        Ray rayBack = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), -transform.forward);
        Ray rayRight = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), transform.right);
        Ray rayLeft = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), -transform.right);
        
        RaycastHit hit;
        // 避免出錯 都先初始化
        gravity = 20;
        // 避免滑行問題
        moveDirection = new Vector3(0.0f, 0.0f, 0.0f);
        
        // RFTD = ray forward turning detect;        
        Vector3 startPos = transform.position - transform.forward / 10 + transform.up / 30;
        Vector3 endPos = transform.position - transform.forward / 3 - transform.up / 10;

       
        Ray RFTD = new Ray(startPos, endPos - startPos);
        // 偵側牆壁 前方
        if (Physics.Raycast(rayForward, out hit, 0.5f))
        {
            
            if (hit.transform != transform && inputVer > 0)
            {
                wallForward = -hit.normal;
                wallForward.y = 0;

                // 如果不是爬牆  下牆 or 爬完牆
                if (!isClimbingVer && !isClimbingHor)
                {
                    transform.forward = wallForward;
                }
                climbWalls(hit.normal, true);
                // 計算旋轉
                Quaternion rot = Quaternion.FromToRotation(transform.forward, newForward);

                transform.rotation = rot * transform.rotation;
            }            
        }
        // 偵測轉角 前方
        else if (Physics.Raycast(RFTD, out hit, 2.5f))
        {
            if (hit.transform != transform && inputVer > 0)
            {
                
                climbWalls(hit.normal, true);
                Quaternion rot = Quaternion.FromToRotation(transform.forward, newForward);
                if (rot != Quaternion.identity)
                {
                    // 
                    if (isFlatform(hit.normal))
                    {
                        wallForward = hit.normal;
                        wallForward.y = 0;
                        // 轉正
                        rot = Quaternion.FromToRotation(transform.forward, wallForward);
                        transform.rotation = rot * transform.rotation;
                        // 旋轉
                        rot = Quaternion.FromToRotation(transform.forward, newForward);

                    }
                    transform.rotation = rot * transform.rotation;
                }
            }
            isInAir = false;
        }

        startPos = transform.position + transform.forward / 10 + transform.up / 30;
        endPos = transform.position + transform.forward / 3 - transform.up / 10;
        Ray RBTD = new Ray(startPos, endPos - startPos);
        // 偵側牆壁 後方
        if (Physics.Raycast(rayBack, out hit, 0.5f))
        {
            if (hit.transform != transform && inputVer < 0)
            {
                wallForward = hit.normal;
                wallForward.y = 0;
                if (!isClimbingVer && !isClimbingHor)
                {
                    transform.forward = wallForward;
                }
                climbWalls(hit.normal, true);
                Quaternion rot = Quaternion.FromToRotation(transform.forward, newForward);
                transform.rotation = rot * transform.rotation;
            }
        }
        // 偵測轉角 後方        
        else if (Physics.Raycast(RBTD, out hit, 2.5f))
        {
            if (hit.transform != transform && inputVer < 0)
            {
                climbWalls(hit.normal, true);
                Quaternion rot = Quaternion.FromToRotation(transform.forward, newForward);
                if (rot != Quaternion.identity)
                {
                    if (isFlatform(hit.normal))
                    {
                        wallForward = -hit.normal;
                        wallForward.y = 0;
                        rot = Quaternion.FromToRotation(transform.forward, wallForward);
                        transform.rotation = rot * transform.rotation;
                        rot = Quaternion.FromToRotation(transform.forward, newForward);
                    }
                    transform.rotation = rot * transform.rotation;
                }
            }
            isInAir = false;
        }
        // 偵側牆壁 右方
        startPos = transform.position - transform.right / 10 + transform.up / 30;
        endPos = transform.position - transform.right / 3 - transform.up / 10;
        Ray RRTD = new Ray(startPos, endPos - startPos);
        if (Physics.Raycast(rayRight, out hit, 0.5f))
        {
            if (hit.transform != transform && inputHor > 0)
            {
                wallForward = Vector3.Cross(-hit.normal, transform.up);
                wallForward.y = 0;
                if (!isClimbingVer && !isClimbingHor)
                {
                    transform.forward = wallForward;
                }
                climbWalls(hit.normal, false);
                Quaternion rot = Quaternion.FromToRotation(transform.right, newForward);
                transform.rotation = rot * transform.rotation;
            }
        }
        // 偵測轉角 右方        
        else if (Physics.Raycast(RRTD, out hit, 2.5f))
        {
            if (hit.transform != transform && inputHor > 0)
            {
                climbWalls(hit.normal, false);
                Quaternion rot = Quaternion.FromToRotation(transform.right, newForward);
                if (rot != Quaternion.identity)
                {
                    if (isFlatform(hit.normal))
                    {
                        wallForward = Vector3.Cross(hit.normal, transform.up);
                        wallForward.y = 0;
                        rot = Quaternion.FromToRotation(transform.forward, wallForward);
                        transform.rotation = rot * transform.rotation;
                        rot = Quaternion.FromToRotation(transform.right, newForward);
                    }
                    transform.rotation = rot * transform.rotation;
                }
            }
            isInAir = false;
        }
        // 偵側牆壁 左方
        startPos = transform.position + transform.right / 10 + transform.up / 30;
        endPos = transform.position + transform.right / 3 - transform.up / 10;
        Ray RLTD = new Ray(startPos, endPos - startPos);
        if (Physics.Raycast(rayLeft, out hit, 0.5f))
        {
            if (hit.transform != transform && inputHor < 0)
            {
                wallForward = Vector3.Cross(hit.normal, transform.up);
                wallForward.y = 0;
                if (!isClimbingVer && !isClimbingHor)
                {
                    transform.forward = wallForward;
                }
                climbWalls(hit.normal, false);
                Quaternion rot = Quaternion.FromToRotation(transform.right, newForward);
                transform.rotation = rot * transform.rotation;
            }
        }
        // 偵測轉角 左方        
        else if (Physics.Raycast(RLTD, out hit, 2.5f))
        {
            if (hit.transform != transform && inputHor < 0)
            {
                climbWalls(hit.normal, false);
                Quaternion rot = Quaternion.FromToRotation(transform.right, newForward);
                if (rot != Quaternion.identity)
                {
                    if (isFlatform(hit.normal))
                    {
                        wallForward = Vector3.Cross(-hit.normal, transform.up);
                        wallForward.y = 0;
                        rot = Quaternion.FromToRotation(transform.forward, wallForward);
                        transform.rotation = rot * transform.rotation;
                        rot = Quaternion.FromToRotation(transform.right, newForward);
                    }                                        
                    transform.rotation = rot * transform.rotation;
                }
            }
            isInAir = false;
        }
        




        float ang = Vector3.Angle(transform.forward, Vector3.up);
        float ang2 = Vector3.Angle(transform.right, Vector3.up);
        //Debug.Log(ang2);
        if (System.Math.Abs(90 - ang) < 0.5f)
        {
            isClimbingVer = false;
        }
        if (System.Math.Abs(90 - ang2) < 0.5f)
        {
            isClimbingHor = false;
        }

        /*testr = newForward;
            testg = transform.right;
            Debug.DrawLine(transform.position + transform.up * 2, transform.position + transform.up * 2 + testr * 10, Color.red);
            Debug.DrawLine(transform.position + transform.up * 2, transform.position + transform.up * 2 + testg, Color.green);*/
    }


    /// <summary>
    /// 檢查是否為平台
    /// 三種方向向量去減掉
    /// 如果有兩種是接近 "0" 則代表他是平台
    /// </summary>        
    private bool isFlatform(Vector3 vec)
    {
       
        int count = 0;
        if (System.Math.Abs(1 - vec.x) > 0.995)
        {
            count++;
        }
        if (System.Math.Abs(1 - vec.y) > 0.995)
        {
            count++;
        }
        if (System.Math.Abs(1 - vec.z) > 0.995)
        {
            count++;
        }        
        if (count >= 2)
        {
            return true;
        }        
        return false;
    }

    
    /// <summary>
    /// 
    /// </summary>

    private void climbWalls(Vector3 normal, bool isVer)
    {
        if (isVer)
        {
            isClimbingVer = true;
            newForward = Vector3.Cross(normal, -transform.right);
        }
        else
        {
            isClimbingHor = true;
            newForward = Vector3.Cross(normal, transform.forward);
        }

    }

    /// <summary>
    /// 偵測人物在哪個影子內
    /// </summary>
    public void shadowDetect()
    {        
        isInShadow = false;
        Vector3 playerPos = transform.position;

        float offset = 1.0f;
        if (isShadowing)
        {
            offset = 0.2f;
        }
        playerPos += transform.up * offset;
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

                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * distance, Color.red);
                    // 判定有沒有在光線範圍內
                    // 判定光線有沒被物體檔到
                    // 光線擋到物體不可以是玩家
                    if (distance <= lightCompnent.range)
                    {
                        if (Physics.Raycast(ray, out hit, distance))
                        {
                            
                           
                            if (hit.transform == transform)
                            {

                                isLighted = true;
                            }
                            else if (lightsWithShadows[lights[i]] != hit.transform.gameObject)
                            {
                                lightsWithShadows[lights[i]] = hit.transform.gameObject;
                            }

                            if (!isLighted)
                            {
                                isInShadow = true;
                            }
                        }                                               
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
                    Vector3 dir = (playerPos - lights[i].transform.position) * 2f;
                    float angle = Vector3.Angle(dir, lights[i].transform.forward);
                    // 判定有沒有在光線範圍內
                    // 判定光線有沒有被物體檔到
                    // 光線擋到物體不可以是玩家
                    if (distance <= lightCompnent.range && angle <= lightCompnent.spotAngle / 2 && Physics.Raycast(ray, out hit, distance))
                    {
                        if (hit.transform == transform)
                        {
                            isLighted = true;
                            return;
                        }
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
    /// <summary>
    /// 找出所有在場景的光源
    /// </summary>
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
    /// 新增 光源進 lights 陣列裡面
    /// </summary>
    /// <param name="lightObject"> 要新增的光源物件 </param>
    public void addNewLightToLights(GameObject lightObject)
    {
        lights.Add(lightObject);
        lightsWithShadows.Add(lightObject, null);
    }


    /// <summary>
    /// 刪除 lights 的指定物件
    /// </summary>
    /// <param name="lightObject"> 要刪除的光源物件</param>
    public void deleteLightsObject(GameObject lightObject)
    {
        lights.Remove(lightObject);                
        lightsWithShadows.Remove(lightObject);
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
    /// 設立影子邊界座標
    /// </summary>
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
            dis = 1000;
        }
        else
        {
            dis = 1000;

        }
        Ray ray = new Ray(shadowOwner.position, shadowOwner.position - lightPos);
        //Debug.DrawRay(ray.origin, ray.direction, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dis))
        {
            if (hit.transform != transform)
            {
                shadowMoveDir = shadowPos - hit.point;
                shadowPos = hit.point;

            }

        }
        if (isInShadow || !isLighted)
        {
            Debug.Log(isLighted);
            dir = transform.position - shadowPos;            
        }

    }

    /// <summary>
    /// 設立 shadowOwner& shadowOwnerLight 物件為潛入影子的主人& 燈光
    /// </summary>
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




    public void init()
    {
        //載入所有光源
        findAllLightsInScene();
        //載入人物身上所有mesh物件
        findAllMeshsInScene();
    }
}
