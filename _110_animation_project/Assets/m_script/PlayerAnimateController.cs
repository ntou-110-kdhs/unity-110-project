using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimateController : MonoBehaviour
{
    Animator animator;              //設置空的ANIMATOR變數
    AnimatorStateInfo stateinfo;    //當前的ANIMATION
    private ThrowItemsModule throwModule;   //丟東西模組
    private M_TestPlayerController playerController;   //玩家控制
    private Damage_script PlayerDamage;     //玩家攻擊傷害(武器上)
    private Push_Module pushModule;
    private CharacterController charController;
    private ShadowModule shadowModule;
    public float forward = 0;
    float right = 0;
    float idletime = -10f;          //空閒一段時間  才會設置IDLE TRIGGER
    float TimeOffSet = -10;   //連擊判定
    float speed = 6;                     //移動攻擊時滑動距離
    [SerializeField]
    private Transform RightHandTarget;
    //部位IK
    private FastIKFabric[] IKs;
    //頭部IK
    private FastIKLook[] HeadIK;

    /***********推移動畫***********/
    bool isPushingObject = false;   //是否正在推動物體
    /***********推移動畫***********/

    private void Start()
    {
        //尋找有含IK的物件
        IKs = FindObjectsOfType(typeof(FastIKFabric)) as FastIKFabric[];
        HeadIK = FindObjectsOfType(typeof(FastIKLook)) as FastIKLook[];
        //EnabledIKInScene();
        DisabledIKInScene();

        animator = GetComponent<Animator>();      //指定此物件的ANIMATOR
        //丟東西模組
        throwModule = GetComponent<ThrowItemsModule>();

        playerController = GetComponent<M_TestPlayerController>();

        PlayerDamage = GetComponentInChildren<Damage_script>();

        pushModule = GetComponent<Push_Module>();
        charController = GetComponent<CharacterController>();
        shadowModule = GetComponent<ShadowModule>();
    }
    /***********IK動畫***********/

    /// <summary>
    /// 停止人物身上所有的IK
    /// </summary>
    private void DisabledIKInScene()
    {
        //四肢IK
        for (int i = 0; i < IKs.Length; i++)
        {
            IKs[i].GetComponent<FastIKFabric>().enabled = false;
        }
        //頭部IK
        for (int i = 0; i < HeadIK.Length; i++)
        {
            HeadIK[i].GetComponent<FastIKLook>().enabled = false;
        }
    }

    /// <summary>
    /// 啟用人物身上所有的IK
    /// </summary>
    private void EnabledIKInScene()
    {
        for (int i = 0; i < IKs.Length; i++)
        {
            IKs[i].GetComponent<FastIKFabric>().enabled = true;
        }
        //頭部IK
        for (int i = 0; i < HeadIK.Length; i++)
        {
            HeadIK[i].GetComponent<FastIKLook>().enabled = true;
        }
    }

    /***********IK動畫***********/

    /***********影子動畫***********/
    public void jumpIntoShadow()                        //潛入影子動畫
    {
        resetAllTrigger();
        animator.Play("jumping_into_shadow");

    }

    public void jumpOutOfShadowEnd()                //jumpOutOfShadow 動畫結束時呼叫
    {
        animator.ResetTrigger("out_of_shadow");
    }

    public void jumpOutOfShadow()                        //浮出影子動畫   
    {
        resetAllTrigger();
        animator.SetTrigger("out_of_shadow");           //目前是以jumpIntoShadow 轉換到 jumpOutOfShadow  所以使用TRIGGER
        //animator.Play("jumping_out_of_shadow");       //若之後要直接轉換  則使用play

    }

    /***********影子動畫***********/

    /***********推移動畫***********/
    public void pushingObject()                         //讓playercontroller傳遞  正在推動物體  並改變動畫控制器的isPushingObject
    {
        animator.SetBool("pushing", true);
        isPushingObject = true;
    }

    public void notPushingObject()                         //讓playercontroller傳遞  沒有推動物體  並改變動畫控制器的isPushingObject
    {
        animator.SetBool("pushing", false);
        isPushingObject = false;
    }

    public void addForward()                                //因為推移物品時 W S 不一定代表前進後退，因此需要這2個函式讓PLAYERCONTROLLER呼叫
    {
        if (forward < 1f) forward += 0.1f;         //限制forward在0~1之間
        animator.SetFloat("forward", forward);
    }

    public void minusForward()
    {
        if (forward > -1f) forward -= 0.1f;      //限制forward在0~1之間
        animator.SetFloat("forward", forward);
    }

    public void setToZero()                                     //用來規0
    {
        if (forward >= 0.1) forward -= 0.1f;
        else if (forward <= -0.1) forward += 0.1f;

        animator.SetFloat("forward", forward);      //將前進 後退 歸0
    }
    /***********推移動畫***********/

    /**********繩索射出*********/
    public void playShootCrossbowAnimation(float distanceBetweenTarget, float y)
    {
        float numBaseOnDis = 0;     //根據與目標距離  調整大小   越近越小
        if (distanceBetweenTarget <= 10) numBaseOnDis = 1;
        else if (distanceBetweenTarget <= 20) numBaseOnDis = 2;
        else if (distanceBetweenTarget <= 30) numBaseOnDis = 3;
        else if (distanceBetweenTarget <= 40) numBaseOnDis = 5;
        else numBaseOnDis = 10;
        if (y >= 0)                         //若目標物在上方
        {
            //Debug.Log("angle=" + angle);
            float angle = y / numBaseOnDis;
            if (angle > 1) angle = 1;
            animator.SetFloat("shooting_angle", Mathf.Lerp(0.5f, 1, angle));
            //Debug.Log(angle);
        }
        else                                //若目標物在下方
        {
            float angle = y * -1 / numBaseOnDis;
            if (angle > 1) angle = 1;
            animator.SetFloat("shooting_angle", Mathf.Lerp(0.5f, 0, angle));
        }
        animator.Play("Shoot_Crossbow");
    }
    /**********繩索射出*********/

    /***********投擲動畫***********/
    //投擲物品模組 throwingObjectTrigger() => throwingObject()

    public void throwingObject()                         //玩家點擊投擲時(瞄準後點擊左鍵)
    {
        resetAllTrigger();
        animator.SetBool("throw_item_aiming", false);
        animator.SetTrigger("throw_item");
    }


    public void throwingObjectTrigger()                         //玩家點擊瞄準時(點擊T)
    {
        resetAllTrigger();
        animator.SetTrigger("throw_item_start");
    }
    /***********投擲動畫***********/

    /***********攻擊動畫***********/
    public void attackInit()                           //攻擊動畫開始   使玩家停止移動
    {
        playerController.IsMovable = false;

    }
    public void attackStart()                           //攻擊開始   能開始傷害NPC
    {
        Debug.Log("on attack");
        PlayerDamage.Attacking();
    }

    public void attackMove(int forwardspeed)                           //攻擊時   玩家移動
    {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == ("kachujin_Move_Attack_F"))
        {
            Quaternion targetRotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(right, 0, forward)), Vector3.up);
            transform.rotation = targetRotation;
        }


        InvokeRepeating("attackForwardMove", 0, 0.02f);
        speed = forwardspeed;
    }

    private void attackForwardMove()        //攻擊時往前移動功能
    {

        Vector3 moveDirection = transform.forward;
        moveDirection.y -= 20 * Time.deltaTime;
        moveDirection *= speed;
        if (speed > 0.1) speed -= 0.1f;
        this.GetComponent<CharacterController>().Move(moveDirection * Time.deltaTime);
    }

    private void attackForwardMoveStop()        //停止攻擊時往前移動功能
    {
        CancelInvoke("attackForwardMove");
    }

    public void attackFinished()                           //攻擊結束   停止傷害NPC
    {
        //playerController.IsMovable = true;
        PlayerDamage.Stop_Attacking();
    }
    public void attackEnd()                           //攻擊結束   使玩家可以移動
    {
        playerController.IsMovable = true;

    }
    /***********攻擊動畫***********/
    public void resetAllTrigger()                       //重製所有TRIGGER
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("walking");
        animator.ResetTrigger("Attacking");
        animator.ResetTrigger("jump");
        animator.ResetTrigger("throw_item");
        //animator.ResetTrigger("throw_item_start");
    }



    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);

            animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
        }
    }


    void Update()
    {
        //當按下WASD時  觸發TRIGGER
        /*
        if (Input.GetKey(KeyCode.E))
        {
            jumpIntoShadow();
        }
        else if(Input.GetKeyUp(KeyCode.E))
        {
            jumpOutOfShadow();
        }
        
        */

        //每貞都會RESET投擲物品TRIGGER  避免BUG
        //animator.ResetTrigger("throw_item_start");


        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))       //移動ANIMATION
        {
            animator.ResetTrigger("Idle");

            //若isPushingObject為TURE 代表角色正在推動物體  則不觸發WALKING
            if (isPushingObject)
            {
                animator.ResetTrigger("walking");
            }
            else
            {
                animator.SetTrigger("walking");
            }


            /**********推移移動**********/
            if (isPushingObject)
            {
            }
            /**********推移移動**********/
            /**********一般移動**********/
            else
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (forward < 1f) forward += 0.1f;         //限制forward在0~1之間
                    animator.SetFloat("forward", forward);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    if (forward > -1f) forward -= 0.1f;      //限制forward在0~1之間
                    animator.SetFloat("forward", forward);
                }
                else                                        //沒有輸入W或S   forward將回歸至0
                {

                    if (forward >= 0.1) forward -= 0.1f;
                    else if (forward <= -0.1) forward += 0.1f;

                    animator.SetFloat("forward", forward);      //將前進 後退 歸0
                }

                if (Input.GetKey(KeyCode.D))
                {
                    if (right < 1f) right += 0.1f;          //限制right在0~1之間
                    animator.SetFloat("right", right);
                }
                else if (Input.GetKey(KeyCode.A))
                {
                    if (right > -1f) right -= 0.1f;         //限制right在0~1之間
                    animator.SetFloat("right", right);
                }
                else                                        //沒有輸入W或S   right將回歸至0
                {
                    if (right >= 0.1) right -= 0.1f;
                    else if (right <= -0.1) right += 0.1f;
                    animator.SetFloat("right", right);
                }
            }
            /**********一般移動**********/



        }
        else
        {
            //animator.SetTrigger("Idle");
            animator.ResetTrigger("walking");//重製TRIGGER

            /*****移動回歸*****/
            //沒有輸入W或S   forward將回歸至0
            if (forward >= 0.1) forward -= 0.1f;
            else if (forward <= -0.1) forward += 0.1f;
            animator.SetFloat("forward", forward);      //將前進 後退 歸0

            //沒有輸入A或D   right將回歸至0
            if (right >= 0.1) right -= 0.1f;
            else if (right <= -0.1) right += 0.1f;
            animator.SetFloat("right", right);
            /*****移動回歸*****/

        }



        if (Input.GetMouseButton(0))        //左鍵 TRIGGER
        {
            if (throwModule.IsTakingAim)
            {
                animator.SetTrigger("throw_item");
            }
            else if (!(pushModule.IsPushingObject || shadowModule.IsShadowing || !charController.isGrounded))
            {
                //防止單次點擊 造成2次攻擊TRIGGER
                if (Time.time - TimeOffSet >= 0.2)
                {
                    TimeOffSet = Time.time;
                    animator.SetTrigger("Attacking");
                }
            }

        }
        else
        {
            //animator.ResetTrigger("Attacking");
            animator.ResetTrigger("throw_item");
        }

        if (Input.GetMouseButton(1))        //右鍵 防禦
        {
            if (!(pushModule.IsPushingObject || shadowModule.IsShadowing || !charController.isGrounded || throwModule.IsTakingAim))
            {
                //防止單次點擊 造成2次防禦TRIGGER
                if (Time.time - TimeOffSet >= 0.2)
                {
                    TimeOffSet = Time.time;
                    animator.SetBool("Blocking", true);
                }
            }

        }
        else
        {
            //animator.ResetTrigger("Attacking");
            animator.SetBool("Blocking", false);
        }


        if (Input.GetKey(KeyCode.Space))    //空白建 跳躍
        {
            animator.SetTrigger("jump");
        }
        else
        {
            animator.ResetTrigger("jump");
        }

        if (!Input.anyKey)          //沒點擊 也沒按按鍵
        {
            if (Time.time - idletime >= 0.1) animator.SetTrigger("Idle");           //空閒一段時間  才會設置IDLE TRIGGER
        }
        else
        {
            idletime = Time.time;                                                     //紀錄IDLE 當下的時間
        }



    }
    void LateUpdate()
    {
        stateinfo = animator.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(stateinfo);
        //若非攻擊狀態 取消傷害
        if (!(stateinfo.IsName("kachujin_swing_sword") || stateinfo.IsName("kachujin_Move_Attack_F") || stateinfo.IsName("kachujin_Move_Attack_IK_F2") || stateinfo.IsName("kachujin_Move_Attack_IK_F3")))
        {
            PlayerDamage.Stop_Attacking();

        }
        //若非防禦狀態 取消防禦
        if (!(stateinfo.IsName("Block") || stateinfo.IsName("Block2")))
        {
            playerController.Isblocking = false;
            if (stateinfo.IsName("End_Block"))
            {
                attackEnd();
            }
        }
        else
        {
            EnabledIKInScene();
        }
        Debug.Log(playerController.Isblocking);
    }
}
