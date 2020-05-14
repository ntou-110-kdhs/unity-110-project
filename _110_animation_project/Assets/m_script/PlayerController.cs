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

    //你人是否站在影子上
    private bool isInShadow = false;
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
    /********鍵鼠操控變數*******/

    // Start is called before the first frame update
    void Start()
    {
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
        // 偵測影子
        shadowDetect();
        // 印出你踩在哪個影子上
        printWhatShadowsIn();


        if (Input.GetKey(KeyCode.E) && isInShadow)
        {
            if (!isShadowing)
            {
                isShadowing = true;
                transformToShadow();
            }
        }
        else if (Input.GetKeyUp(KeyCode.E) && isShadowing)
        {
            isShadowing = false;
            transformToShadow();
        }

        //移動模組
        move();
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

        if (!isInShadow && isShadowing)
        {
            isShadowing = false;
            transformToShadow();
        }

        //角色在落地時啟動
        if (charController.isGrounded)
        {
            //方向鍵有按著的時候才會啟動
            if (inputHor != 0 || inputVer != 0)
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
            freeLookCam.LookAt = transform;
            // freeLookCam.m_Orbits[0] = top
            // freeLookCam.m_Orbits[1] = mid
            // freeLookCam.m_Orbits[2] = bot
            freeLookCam.m_Orbits[1].m_Height = _newRigsHeight;
            freeLookCam.m_Orbits[1].m_Radius = _newRigsRadius;
            freeLookCam.m_Orbits[2].m_Height = _newRigsHeight;
            freeLookCam.m_Orbits[2].m_Radius = _newRigsRadius;
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

    //浮出影子動畫開始
    public void playerControllerJOFS()
    {
        //想做啥
    }

   //潛入影子動畫結束
    public void playerControllerJIS()
    {
        //想做啥
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
                    if (distance <= lightCompnent.range && angle <= lightCompnent.spotAngle / 2 && Physics.Raycast(ray, out hit, distance) && hit.transform != transform)
                    {
                        //Debug.Log("Spot light make you in shadow");
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
        for (int i = 0; i < meshsFilter.Length; i++)
        {
            meshs.Add(meshsFilter[i].gameObject);
        }
    }

    /// <summary>
    /// 影子移動    
    /// </summary>

    private void shadowMove()
    {

    }


}