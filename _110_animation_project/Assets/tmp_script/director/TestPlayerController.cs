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
    //人物身上的freelook攝影機
    [SerializeField] private CinemachineFreeLook freelook;

    //你人是否站在影子上
    [SerializeField] private bool isInShadow = false;                
    //光源的陣列
    private List<GameObject> lights = new List<GameObject>();
    //一個光源對一個物件所製造出的影子  陣列
    private Dictionary<string, GameObject> lightsWithShadows = new Dictionary<string, GameObject>();
    
    //你人是否"進入"影子內
    [SerializeField] private bool isShadowing = false;
    //人物身上的mesh物件陣列
    private List<GameObject> meshs = new List<GameObject>();
    //水圈圈粒子物件
    [SerializeField] private GameObject ripple;

    [SerializeField] private bool isClimbing = false;
    [SerializeField] private bool isWall;
    [SerializeField] private int shadowFlyCount;

    /// /////////
    CharacterController charController;
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public CinemachineFreeLook Camera;

    private Vector3 moveDirection = Vector3.zero;
    private Collider collideR;
    private float distToGround;
    private Quaternion targetRotation;

    /// //////////////////////
    private float delayCount = 0;

    // Start is called before the first frame update
    void Start()
    {        
        //載入所有光源
        findAllLightsInScene();
        //載入人物身上所有mesh物件
        findAllMeshsInScene();
        //取得animateController
        animateController = GetComponent<PlayerAnimateController>();
        //取得freelook攝影機，要用名字找太暴力，所以用掛的比較好
        if(freelook == null)
        {
            freelook = GameObject.Find("CM FreeLook1").GetComponent<CinemachineFreeLook>();
        }
        //取得ripple物件，潛入影子後的特效
        if(ripple == null)
        {
            ripple = transform.Find("Ripple").gameObject;
        }




        ////
        charController = GetComponent<CharacterController>();
        collideR = GetComponent<Collider>();
        distToGround = collideR.bounds.extents.y;
        ////
    }

    // Update is called once per frame
    void Update()
    {
        // 偵測影子
        shadowDetect();
        // 印出你踩在哪個影子上
        //printWhatShadowsIn();

        if(charController.isGrounded)
        {
            if (Input.GetKey(KeyCode.E) && !isShadowing && isInShadow)
            {            
                if(Time.time - delayCount > 0.20f)
                {
                    isShadowing = true;
                    transformToShadow();
                }            
            }        
            else if (Input.GetKeyDown(KeyCode.E) && isInShadow && charController.isGrounded)
            {
                isShadowing = !isShadowing;
                transformToShadow();
                delayCount = Time.time;
            }
        }
        

        if (isShadowing)
        {
            shadowMove();
        }
        else
        {
            move();
        }
        


        
    }
    private void shadowMove()
    {
        float input_H = Input.GetAxis("Horizontal");
        float input_V = Input.GetAxis("Vertical");

        Ray rayForward = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f) , transform.forward);
        Ray rayRight = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f), transform.right);
        Ray rayLeft = new Ray(transform.position + new Vector3(0.0f, 0.005f, 0.0f) , -transform.right);
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
        if(Physics.Raycast(rayForward, out hit, 1.0f))
        {
            if(hit.transform != transform )
            {
                isWall = true;
                climbForward = true;
            }            
        }
        // 偵側牆壁 後方
        else if(Physics.Raycast(rayBack, out hit, 1.0f))
        {
            if (hit.transform != transform)
            {
                isWall = true;
                climbBack = true;
            }
        }
        // 偵側牆壁 右方
        else if(Physics.Raycast(rayRight, out hit, 1.0f))
        {
            if(hit.transform != transform)
            {
                isWall = true;
                climbRight = true;
            }
        }
        // 偵側牆壁 左方
        else if (Physics.Raycast(rayLeft, out hit, 1.0f))
        {
            if (hit.transform != transform)
            {
                isWall = true;
                climbLeft = true;
            }
        }
        // 避免斜坡之後人整個飄在天上
        if (isClimbing && !isWall)
        {
            shadowFlyCount++;
            if (shadowFlyCount >= 20)
            {
                shadowFlyCount = 0;
                isClimbing = false;
            }            
        }
        else
        {
            shadowFlyCount = 0;
        }

        // 移動
        if (input_H != 0 || input_V != 0)
        {
            //以camera LookAt pos與camera本身pos的向量 更改角色forward方向
            Vector3 camFor = freelook.LookAt.position - freelook.transform.position;
            camFor.y = 0.0f;
            targetRotation = Quaternion.LookRotation(camFor, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);  
            
            // 移動方向
            moveDirection = transform.TransformDirection(new Vector3(input_H, 0, input_V)/*.normalized*/);

            // 爬牆中
            if(isWall)
            {
                gravity = 0;
                // 爬牆中三個方向的位移量
                float dirZ = input_V;
                float dirX = input_H;
                float dirY = 0;

                // 如果前方OR後方有牆
                if (climbForward || climbBack)
                {
                    dirY = input_V;
                    if (climbBack)
                    {
                        dirY *= -1;
                    }
                }
                // 又如果左方OR右方有牆
                else if (climbLeft || climbRight)
                {
                    dirY = input_H;
                    if (climbLeft)
                    {
                        dirY *= -1;
                    }
                }                

                // 確定有牆&&同時開始爬 (離地) 了
                if (!charController.isGrounded )
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
            moveDirection *= speed;
        }
        // 爬牆失效時
        if (charController.isGrounded || !isInShadow || Input.GetKeyDown(KeyCode.E))
        {
            isClimbing = false;
            isWall = false;
        }    
        moveDirection.y -= gravity * Time.deltaTime;
        if ( !isClimbing && isShadowing && (!isInShadow || (!charController.isGrounded && !isWall)))
        {
            isShadowing = false;
            transformToShadow();
            gravity = 20;
        }
        charController.Move(moveDirection * Time.deltaTime);
    }
    

    /// <summary>
    /// 移動
    /// </summary>
    public void move()
    {
        float input_H = Input.GetAxis("Horizontal");
        float input_V = Input.GetAxis("Vertical");

        /////

        //角色在落地時啟動
        if (charController.isGrounded)
        {
            //方向鍵有按著的時候才會啟動
            if (input_H != 0 || input_V != 0)
            {
                //以camera LookAt pos與camera本身pos的向量 更改角色forward方向
                Vector3 camFor = freelook.LookAt.position - freelook.transform.position;
                camFor.y = 0.0f;
                targetRotation = Quaternion.LookRotation(camFor, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
 
            //前進方向local coord.轉world coord.
            moveDirection = transform.TransformDirection(new Vector3(input_H, 0, input_V)/*.normalized*/);

            moveDirection *= speed;

            //按空白鍵時啟動
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        //給予重力
        moveDirection.y -= gravity * Time.deltaTime;
        //Debug.Log(moveDirection);

        charController.Move(moveDirection * Time.deltaTime);



        /////
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

    /// <summary>
    /// 潛入影子
    /// parameter: 
    /// None
    /// member var need:
    /// None
    /// 
    /// </summary>
    public void transformToShadow()
    {
        const float _newRigsHeight = 1.5f;
        const float _newRigsRadius = 6.0f;

        // 調整攝影機位置
        if (isShadowing)
        {
            animateController.jumpIntoShadow();
            ripple.GetComponent<ParticleSystem>().Play();
            freelook.LookAt = transform;
            // freelook.m_Orbits[0] = top
            // freelook.m_Orbits[1] = mid
            // freelook.m_Orbits[2] = bot
            freelook.m_Orbits[1].m_Height = _newRigsHeight;
            freelook.m_Orbits[1].m_Radius = _newRigsRadius;
            freelook.m_Orbits[2].m_Height = _newRigsHeight;
            freelook.m_Orbits[2].m_Radius = _newRigsRadius;
        }
        else
        {
            animateController.jumpOutOfShadow();
            ripple.GetComponent<ParticleSystem>().Stop();
            freelook.LookAt = transform.GetChild(2);
            // freelook.m_Orbits[0] = top
            // freelook.m_Orbits[1] = mid
            // freelook.m_Orbits[2] = bot
            freelook.m_Orbits[1].m_Height = 2.5f;
            freelook.m_Orbits[1].m_Radius = 3.0f;
            freelook.m_Orbits[2].m_Height = 0.8f;
            freelook.m_Orbits[2].m_Radius = 1.3f;
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

    /// <summary>
    /// 刺殺
    /// </summary>
    public void assassinate()
    {
        //if (inShadow)
        //{
        //    if(Target.tag == "monster")
        //    {
        //    }
        //    else if (Target.tage == "boss")
        //    {
        //    }
        //}
        //// 一般暗殺
        //else
        //{
        //}
    }

    /// <summary>
    /// 丟東西
    /// </summary>
    public void throwItem()
    {

    }


    /// <summary>
    /// 十字弓射擊動作反映
    /// </summary>
    public void crossBowShoot()
    {
        //TODO
    }
    //做出射繩動作並綁好繩子(一剛開始要在能綁繩的位置才能觸發) 

    /// <summary>
    /// 偵測人物在哪個影子內
    /// </summary>
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
                    if(lightsWithShadows[lights[i].name] != hit.transform.gameObject)
                    {
                        lightsWithShadows[lights[i].name] = hit.transform.gameObject;
                    }
                    isInShadow = true;                    
                }
                else
                {
                    lightsWithShadows[lights[i].name] = null;
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
                        if (lightsWithShadows[lights[i].name] != hit.transform.gameObject)
                        {
                            lightsWithShadows[lights[i].name] = hit.transform.gameObject;
                        }
                        isInShadow = true;
                    }
                    else
                    {
                        lightsWithShadows[lights[i].name] = null;
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
                    if(distance <= lightCompnent.range && angle <= lightCompnent.spotAngle / 2 && Physics.Raycast(ray, out hit, distance) && hit.transform != transform)
                    {
                        //  Debug.Log(hit.transform.name);
                        //Debug.Log("Spot light make you in shadow");
                        if (lightsWithShadows[lights[i].name] != hit.transform.gameObject)
                        {
                            lightsWithShadows[lights[i].name] = hit.transform.gameObject;
                        }
                        isInShadow = true;
                    }
                    else
                    {
                        /*if(lightsWithShadows[lights[i].name] != null)
                        {
                            Debug.Log(lights[i].name);
                        }*/
                        lightsWithShadows[lights[i].name] = null;
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
            lightsWithShadows.Add(light.transform.name, null);
        }
    }
    /// <summary>
    /// 印出你在哪個影子內
    /// </summary>
    private void printWhatShadowsIn()
    {
        foreach (KeyValuePair<string, GameObject> i in lightsWithShadows)
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
        for(int i = 0; i < meshsFilter.Length; i++)
        {
            meshs.Add(meshsFilter[i].gameObject);
        }
    }

    



}