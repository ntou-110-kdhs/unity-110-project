using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using Cinemachine;

[RequireComponent(typeof(NavMeshAgent))]
public class FindRoadController : MonoBehaviour
{
    private Transform target;//target to follow
    private Transform checkTarget;//target to follow
    private EnemyAnimateController enemyAC; 
    [SerializeField] private NavMeshAgent agent;
    //private Ray rayToTarget;
    private RaycastHit hitInfo;

    [Header("路線物件")]
    [SerializeField] private GameObject agentRoadSet;
    [SerializeField] private Transform agentRoadSubSet;
    [SerializeField] private Transform[] setPoints;
    [SerializeField] private int pointSetSize = 0;
    [SerializeField] private int nextPoint = 0;
    [SerializeField] private bool isSetPoint = false;
    [SerializeField] private Vector3 tmpXYZ;
    private float tmpYoffset = 0.0f;


    //private Animator thisAnimator;          //自身动画组件
    private int Animate_timeLen = 3;            //模擬動畫播放時間
    [SerializeField] private Vector3 initialPosition;      //初始位置
    //private Vector3 moveDirection = Vector3.zero;
    //[SerializeField] private float gravity = 20.0f;
    
    [Header("狀態反映範圍")]
    [SerializeField] private float wanderRadius = 5f;          //游走半径，移动状态下，如果超出游走半径会返回出生位置
    [SerializeField] private float defendRadius = 6f;          //自卫半径，玩家进入后怪物会追击玩家，当距离<攻击距离则会发动攻击（或者触发战斗）
    [SerializeField] private float checkRadius = 7f;         //警戒半径，玩家进入后怪物会发出警告，并一直面朝玩家
    [SerializeField] private float alertRadius = 8f;         //警戒半径，玩家进入后怪物会发出警告，并一直面朝玩家
    [SerializeField] private float chaseRadius = 10f;            //追击半径，当怪物超出追击半径后会放弃追击，返回追击起始位置

    [SerializeField] private float attackRadius = 3f;            //攻击距离

    [Header("NPC反映參數")]
    [Range(0, 180)]
    [SerializeField] private float alertAngle;         //視角範圍
    [SerializeField] private float walkSpeed;          //移动速度
    [SerializeField] private float runSpeed;          //跑动速度
    [SerializeField] private float turnSpeed;         //转身速度，建议0.1

    [Header("警戒率")]
    //[Range(0, 100)]
    //[SerializeField] private float wonderRate = 0.0f;               //警戒第一階段比率
    [Range(0, 100)]
    [SerializeField] private float alertRate = 0.0f;                //警戒第二階段比率
    private float alertLstLostTime = 0.0f;                          //The last time lost the target.

    private enum MonsterState
    {
        STAND,      //原地呼吸
        CHECK,       //原地观察
        //WALK,       //移动
        WARN,       //盯着玩家
        CHASE,      //追击玩家
        RETURN,      //超出追击范围后返回
        ASSASSINATED
    }
    
    //public enum monsterState
    //{
    //    get { return MonsterState; }
    //}
    [Header("狀態")]
    [SerializeField] private MonsterState currentState = MonsterState.STAND;          //默认状态为原地呼吸
    [SerializeField] private EnemyIconManager iconManager = null;
    [SerializeField] private float actRestTme;            //更换待机指令的间隔时间
    private float lastActTime;          //最近一次指令时间

    [Header("狀態權重")]
    [SerializeField] private float[] actionWeight = { 1000, 1000 };         //设置待机时各种动作的权重，顺序依次为呼吸、观察/*、移动*/

    private float diatanceToPlayer;         //怪物与玩家的距离
    private float diatanceToInitial;         //怪物与初始位置的距离
    private Quaternion targetRotation;         //怪物的目标朝向

    private bool is_Checking = false;
    private bool is_Warned = false;    
    private bool is_Running = false;
    private bool is_Chased = false;


    // 巡邏到位之後是否停頓
    [SerializeField] private bool is_Stopping = false;



    //GetPointToDo test;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        enemyAC = GetComponent<EnemyAnimateController>();
        //检查并修正怪物设置
        //1. 自卫半径不大于警戒半径，否则就无法触发警戒状态，直接开始追击了
        defendRadius = Mathf.Min(alertRadius, defendRadius);
        //2. 攻击距离不大于自卫半径，否则就无法触发追击状态，直接开始战斗了
        attackRadius = Mathf.Min(defendRadius, attackRadius);
        //3. 游走半径不大于追击半径，否则怪物可能刚刚开始追击就返回出生点
        wanderRadius = Mathf.Min(chaseRadius, wanderRadius);

        //随机一个待机动作
        //RandomAction();

        if (agentRoadSet == null)
        {
            agentRoadSet = GameObject.Find("agentRoadPoint");
            agentRoadSubSet = agentRoadSet.transform.Find(gameObject.name);            
        }

        if (agentRoadSubSet != null)
        {
            pointSetSize = agentRoadSubSet.transform.childCount;
        }
            
        setPoints = new Transform[pointSetSize];
        for (int i = 0; i < pointSetSize; i++) 
        {
            if (setPoints[i] == null)
            {
                setPoints[i] = agentRoadSubSet.transform.GetChild(i);
            }
        }

        //保存初始位置信息
        //initialPosition = gameObject.GetComponent<Transform>().position;
        initialPosition = nextPoint > 0?setPoints[nextPoint].position: transform.position ;
        //test = setPoints[nextPoint].GetComponent<GetPointToDo>();
        tmpYoffset = agent.height / 2;
    }

    // Update is called once per frame
    void Update()
    {
        MonsterStateLoop(); 
        /*if (target != null && currentState != MonsterState.STAND)
        {
           
            //FaceTarget();
        }       
        EnemyDistanceCheck();*/
    }


    void MonsterStateLoop()
    {
        //FixedUpdate()
        Vector3 targetDirect = Vector3.zero;
        switch (currentState)
        {
            //待机状态，等待actRestTme后重新随机指令
            case MonsterState.STAND:
                if (Time.time - lastActTime > actRestTme)
                {
                    RandomAction();         //随机切换指令
                }

                if (!is_Stopping)
                {
                    enemyAC.knightNotEngage();
                    enemyAC.knightRunning();
                }


                StandPatrol();

                


                iconManager.setIcon(0);
                iconManager.setIconRate(0);
                //agent.stoppingDistance = 0f;
                agent.updateRotation = true;
                //该状态下的检测指令
                EnemyDistanceCheck();
                break;

            //待机状态，由于观察动画时间较长，并希望动画完整播放，故等待时间是根据一个完整动画的播放长度，而不是指令间隔时间
            case MonsterState.CHECK:
                enemyAC.knightNotEngage();
                enemyAC.knightRunning();
                //agent.stoppingDistance = 0f;
                agent.updateRotation = true;

                agent.stoppingDistance = 2f;

                // 角色行走速度
                agent.speed = walkSpeed;

                iconManager.setIcon(1);
                iconManager.setIconRate(0);

                // 遲疑一下、才決定去確認
                if (is_Checking)
                {
                    targetDirect = checkTarget.transform.position - transform.position;
                    targetDirect.y = 0.0f;
                    targetRotation = Quaternion.LookRotation(targetDirect, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                    agent.SetDestination(checkTarget.position);
                }


                //该状态下的检测指令
                EnemyDistanceCheck();
                CheckingCheck();
                break;

            //游走，根据状态随机时生成的目标位置修改朝向，并向前移动
            //case MonsterState.WALK:
            //    //transform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
            //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime*5f/*turnSpeed*/);
            //    agent.updateRotation = false;

            //    agent.speed = walkSpeed;
            //    Debug.Log("transform.TransformDirection(Vector3.forward)+ transform.position : " + (transform.TransformDirection(Vector3.forward) + transform.position));
            //    agent.SetDestination(transform.TransformDirection(Vector3.forward)+ transform.position);

            //    //moveDirection = transform.TransformDirection(Vector3.forward) * walkSpeed;

            //    if (Time.time - lastActTime > actRestTme)
            //    {
            //        RandomAction();         //随机切换指令
            //    }
            //    //该状态下的检测指令
            //    WanderRadiusCheck();
            //    break;

            //警戒状态，播放一次警告动画和声音，并持续朝向玩家位置
            case MonsterState.WARN:
                if (!is_Warned)
                {
                    is_Warned = true;
                    enemyAC.knightNotEngage();
                    enemyAC.knightRunning();
                }
                targetDirect = target.transform.position - transform.position;
                targetDirect.y = 0.0f;

                agent.stoppingDistance = 2f;

                //持续朝向玩家位置
                targetRotation = Quaternion.LookRotation(targetDirect, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //该状态下的检测指令


                // 角色行走速度、目標點
                agent.speed = walkSpeed;
                agent.SetDestination(target.transform.position);
                agent.updateRotation = true;

                iconManager.setIcon(2);
                iconManager.setIconRate(alertRate * 0.01f);

                WarningCheck();
                break;

            //追击状态，朝着玩家跑去
            case MonsterState.CHASE:
                if (!is_Running)
                {
                    is_Running = true;
                    enemyAC.knightEngageAnimation();
                    enemyAC.knightRunning();
                }
                targetDirect = target.transform.position - transform.position;
                targetDirect.y = 0.0f;

                agent.stoppingDistance = 2f;
                //朝向玩家位置
                targetRotation = Quaternion.LookRotation(targetDirect, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);

                //應根據距離來將runspeed降速
                agent.speed = runSpeed;
                agent.SetDestination(target.transform.position);

                iconManager.setIcon(3);
                iconManager.setIconRate(0);

                //该状态下的检测指令
                ChaseRadiusCheck();
                break;

            //返回状态，超出追击范围后返回出生位置
            case MonsterState.RETURN:
                enemyAC.knightNotEngage();
                enemyAC.knightRunning();
                //朝向初始位置移动
                //targetDirect = initialPosition - transform.position;
                //targetDirect.y = 0.0f;
                agent.stoppingDistance = 0f;
                agent.speed = walkSpeed;
                agent.SetDestination(initialPosition);

                iconManager.setIcon(0);
                iconManager.setIconRate(0);

                //targetRotation = Quaternion.LookRotation(targetDirect, Vector3.up);
                //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed);
                //该状态下的检测指令
                EnemyDistanceCheck();
                ReturnCheck();
                break;

            case MonsterState.ASSASSINATED:
                agent.SetDestination(transform.position);
                break;
        }
        alertRateCtl();
    }


    /// <summary>
    /// 巡邏
    /// </summary>
    void StandPatrol()
    {
        if (pointSetSize > 0)
        {

            if (!isSetPoint)
            {
                if (tmpXYZ == Vector3.zero)
                {
                    tmpXYZ = new Vector3(setPoints[nextPoint].position.x, setPoints[nextPoint].position.y, setPoints[nextPoint].position.z);
                }
                else
                {
                    tmpXYZ.x = setPoints[nextPoint].position.x;
                    tmpXYZ.y = setPoints[nextPoint].position.y;
                    tmpXYZ.z = setPoints[nextPoint].position.z;
                }
                isSetPoint = true;
            }
            if (!is_Stopping)
            {
                agent.SetDestination(setPoints[nextPoint].position);
            }
            
            if (transform.position.x == tmpXYZ.x
                && transform.position.z == tmpXYZ.z
                && Mathf.Abs(tmpXYZ.y - transform.position.y) < tmpYoffset)
            {
                //initialPosition = setPoints[nextPoint].position;
                //Debug.Log("NPC position : " + transform.position);
                isSetPoint = false;
                nextPoint++;
                nextPoint %= pointSetSize;
                initialPosition = setPoints[nextPoint].position;
                if (!is_Stopping)
                {
                    
                    Debug.Log("is Stopping");
                    enemyAC.knightIdle();
                    enemyAC.knightNotRunning();
                    Invoke("Idle", 3.0f);
                    is_Stopping = true;
                }                
            }
        }
    }

    /// <summary>
    /// 巡邏停頓點  
    /// </summary>
    void Idle()
    {
        Debug.Log("Idle");
        enemyAC.knightNotIdle();
        enemyAC.knightRunning();
        is_Stopping = false;


    }

    /// <summary>
    /// 根据权重随机待机指令
    /// </summary>
    void RandomAction()
    {
        //更新行动时间
        lastActTime = Time.time;
        //根据权重随机
        float number = Random.Range(0, actionWeight[0] + actionWeight[1] /*+ actionWeight[2]*/);
        if (number <= actionWeight[0])
        {
            currentState = MonsterState.STAND;
            //Debug.Log("MonsterState.STAND");//Stand state non-animated
            //thisAnimator.SetTrigger("Stand");
        }
        //else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])
        //{
        //    currentState = MonsterState.CHECK;
            // Debug.Log("MonsterState.CHECK");//Stand state animated
            //thisAnimator.SetTrigger("Check");
        //}
        //if (actionWeight[0] + actionWeight[1] < number && number <= actionWeight[0] + actionWeight[1] + actionWeight[2])
        //{
        //    currentState = MonsterState.WALK;
        //    //随机一个朝向
        //    targetRotation = Quaternion.Euler(0, Random.Range(1, 5) * 90, 0);
        //    //Debug.Log("MonsterState.WALK");//Random walk state
        //    //thisAnimator.SetTrigger("Walk");
        //}
    }

    /// <summary>
    /// 原地呼吸、观察状态的检测
    /// </summary>
    void EnemyDistanceCheck()
    {
        diatanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        if ((!alertAngleWithRaycast() && !checkTarget) && (currentState == MonsterState.STAND || currentState == MonsterState.RETURN))
        {
            //currentState = MonsterState.STAND;
            return;
        }

        if (diatanceToPlayer < attackRadius || alertRate == 100.0f)
        {
            Debug.Log("Attack EnemyDistanceCheck");
            //if currentState != MonsterState.CHASE
            currentState = MonsterState.CHASE;
        }
        else if (diatanceToPlayer < defendRadius || alertRate == 100.0f)
        {

            currentState = MonsterState.CHASE;
        }
        else if (diatanceToPlayer < alertRadius)
        {
            currentState = MonsterState.WARN;
        }
        else if (checkTarget != null && Vector3.Distance(checkTarget.position, transform.position) < checkRadius)
        {
            currentState = MonsterState.CHECK;
            Invoke("StartChecking", 2f);
        }
    }

    /// <summary>
    /// start checking
    /// </summary>
    void StartChecking()
    {
        is_Checking = true;
    }

    /// <summary>
    /// Check狀態下的檢測
    /// </summary>
    void CheckingCheck()
    {
        float distanceToCheckTarget = 0f;
        if (checkTarget != null)
        {
            distanceToCheckTarget = Vector3.Distance(checkTarget.position, transform.position);
        }
        

        if(distanceToCheckTarget < 2f)
        {
            // to do
            // animator idle
            Debug.Log("Is Checking...");
            Invoke("CheckFinish", 2);
        }

    }

    /// <summary>
    /// 確認完成
    /// </summary>
    void CheckFinish()
    {
        if(currentState == MonsterState.CHECK)
        {
            Debug.Log("Checking Finish!");
            is_Running = false;
            is_Checking = false;
            checkTarget = null;
            currentState = MonsterState.RETURN;
        }

        
    }
    /// <summary>
    /// 警告状态下的检测，用于启动追击及取消警戒状态
    /// </summary>
    void WarningCheck()
    {
        diatanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        if (!alertAngleWithRaycast())
        {
            //currentState = MonsterState.STAND;
            //alert rate active
            if (!(alertRate > 0.0f)) currentState = MonsterState.RETURN;
            return;
        }
        //if (alertAngleWithRaycast()) return;
        if (diatanceToPlayer < defendRadius || alertRate == 100.0f) 
        {
            is_Warned = false;
            currentState = MonsterState.CHASE;
        }

        if (diatanceToPlayer > alertRadius)
        {
            is_Warned = false;
            //RandomAction();
            currentState = MonsterState.CHECK;
        }
    }

    /// <summary>
    /// 游走状态检测，检测敌人距离及游走是否越界
    /// </summary>
    ///
    /*
    void WanderRadiusCheck()
    {
        diatanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        diatanceToInitial = Vector3.Distance(transform.position, initialPosition);

        if (diatanceToPlayer < attackRadius)
        {
            //Debug.Log("Attack WanderRadiusCheck");
        }
        else if (diatanceToPlayer < defendRadius)
        {
            currentState = MonsterState.CHASE;
        }
        else if (diatanceToPlayer < alertRadius)
        {
            currentState = MonsterState.WARN;
        }

        if (diatanceToInitial > wanderRadius)
        {
            //朝向调整为初始方向
            targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector3.up);
        }
    }
    */
    /// <summary>
    /// 追击状态检测，检测敌人是否进入攻击范围以及是否离开警戒范围
    /// </summary>
    void ChaseRadiusCheck()
    {
        diatanceToPlayer = Vector3.Distance(target.transform.position, transform.position);
        diatanceToInitial = Vector3.Distance(transform.position, initialPosition);

        //if (alertAngleWithRaycast()) return;

        if (diatanceToPlayer < attackRadius)
        {
            //Debug.Log("Attack ChaseRadiusCheck");
        }
        //如果超出追击范围或者敌人的距离超出警戒距离就返回
        if (/*diatanceToInitial > chaseRadius ||*/ diatanceToPlayer > alertRadius)
        {
            currentState = MonsterState.CHECK;
            is_Chased = true;
        }
    }

    /// <summary>
    /// 超出追击半径，返回状态的检测，不再检测敌人距离
    /// </summary>
    void ReturnCheck()
    {
        diatanceToInitial = Vector3.Distance(transform.position, initialPosition);
        //如果已经接近初始位置，则随机一个待机状态
        if (diatanceToInitial < 2f)
        {
            is_Running = false;
            currentState = MonsterState.STAND;
        }
        if(tmpXYZ != Vector3.zero)
        {
            isSetPoint = false;
            tmpXYZ = Vector3.zero;
        }
        //Debug.Log("Return state");
    }
    /// <summary>
    /// 角度 與 隔牆判定
    /// </summary>
    private bool alertAngleWithRaycast()
    {
        bool    ret = false,
                isShadowing = target.GetComponent<ShadowModule>().IsShadowing;
        Vector3 targetTransTmp  = target.transform.position,
                transTmp        = transform.position;
        Vector3 targetDirect = targetTransTmp - transTmp;
        float angle = Vector3.Angle(targetDirect, transform.forward);
        targetTransTmp.y += 1;
        transTmp.y += 1;

        //if (Physics.Raycast(transTmp, targetDirect, out hitInfo))
        //    Debug.Log("hitInfo : " + hitInfo.collider.gameObject.name);
        //else
        //    Debug.Log("Raycast not active.");
        //Debug.DrawLine(transTmp, transTmp + targetDirect.normalized * alertRadius, Color.red);
        bool    isAngleEqual    = angle <= alertAngle,
                isHitting       = Physics.Raycast(transTmp, targetDirect.normalized, out hitInfo, alertRadius);
        bool    isHittingEqual  = isHitting ? hitInfo.collider.gameObject.transform == target : false;
        if (isAngleEqual && isHitting && isHittingEqual && !isShadowing)
        {
            //Debug.Log("Alertistrue is True.");
            //if(angle <= alertAngle)
            //Debug.Log("angle <= alertAngle, angle : " + angle);
            //Debug.Log("hitInfo : " + hitInfo.collider.gameObject.name);
            ret = true;
        }
        else
        {
            //Debug.Log("Alertistrue is False.");
            //Debug.Log("hitInfo : " + hitInfo.collider.gameObject.name);
            ret = false;
        }
        //Debug.Log("angle <= alertAngle : " + isAngleEqual);
        //Debug.Log("Physics.Raycast : " + isHitting);
        //Debug.Log("hitInfo.collider.gameObject == target : " + isHittingEqual);
        //Debug.Log("target : " + isHittingEqual);

        return ret;
    }
    /// <summary>
    /// 警戒率控制
    /// </summary>
    private void alertRateCtl()
    {
        switch (currentState)
        {
            case MonsterState.WARN:
                if (!alertAngleWithRaycast())
                {
                    if (alertLstLostTime == 0.0f) alertLstLostTime = Time.time;
                    if (Time.time - alertLstLostTime >= 0.0f)
                    {
                        alertRate -= 0.8f;
                    }                        
                }
                else
                {
                    alertLstLostTime = 0.0f;
                    alertRate += 1.0f;
                }
                break;
            case MonsterState.CHASE:
                alertLstLostTime = 0.0f;
                alertRate = 100.0f;
                break;
            default:
                alertLstLostTime = 0.0f;
                if (alertRate > 0.0f) alertRate -= 0.8f;
                break;
        }

        if (alertRate < 0.0f)
        {
            alertRate = 0.0f;
        }
        else if (alertRate > 100.0f)
        {
            alertRate = 100.0f;
        }
    }
    private void OnDrawGizmosSelected()
    {
        Vector3 Position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Vector3 StartLine = Quaternion.Euler(0, -alertAngle, 0) * transform.forward;
        Color tmpBlue = Color.blue;
        Color tmpRed = Color.red;
        tmpBlue.a = 0.5f;
        tmpRed.a = 0.5f;

        Handles.color = tmpBlue;
        Handles.DrawSolidArc(Position, transform.up, StartLine, alertAngle, alertRadius);

        StartLine = Quaternion.Euler(0, alertAngle, 0) * transform.forward;
        Handles.color = tmpRed;
        Handles.DrawSolidArc(Position, transform.up, StartLine, -alertAngle, alertRadius);
    }
    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, attackRadius);
    //    Gizmos.color = Color.green;
    //    Gizmos.DrawWireSphere(transform.position, defendRadius);
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, alertRadius);
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, wanderRadius);
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position, chaseRadius);
    //}

    /// <summary>
    /// 設定 Check 目標物
    /// </summary>
    /// <param name="target"> 目標物 </param>
    public void SetCheckTarget(Transform target)
    {
        if(currentState == MonsterState.CHECK || currentState == MonsterState.STAND)
        {
            checkTarget = target;
        }        
    }


    /// <summary>
    /// 被暗殺
    /// </summary>
    public void Assassinated()
    {
        currentState = MonsterState.ASSASSINATED;
    }
}
