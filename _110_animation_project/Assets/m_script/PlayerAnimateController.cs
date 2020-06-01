using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimateController : MonoBehaviour
{
    Animator animator;              //設置空的ANIMATOR變數
    float forward=0;
    float right = 0;
    float idletime = -10f;          //空閒一段時間  才會設置IDLE TRIGGER
    [SerializeField]
    private Transform RightHandTarget;

    bool isPushingObject = false;   //是否正在推動物體

    private void Start()
    {
        animator=GetComponent<Animator>();      //指定此物件的ANIMATOR
    }

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
        animator.SetBool("pushing",true);
        isPushingObject = true;
    }

    public void notPushingObject()                         //讓playercontroller傳遞  沒有推動物體  並改變動畫控制器的isPushingObject
    {
        animator.SetBool("pushing", false);
        isPushingObject = false;
    }

    /***********推移動畫***********/


    public void attackStart()                           //攻擊動畫開始   能開始傷害NPC
    {
        Debug.Log("on attack");
        BroadcastMessage("Attacking");
    }

    public void attackFinished()                           //攻擊動畫結束   停止傷害NPC
    {
        BroadcastMessage("Stop_Attacking");
    }

    public void resetAllTrigger()                       //重製所有TRIGGER
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("walking");
        animator.ResetTrigger("Attacking");
        animator.ResetTrigger("jump");
        animator.ResetTrigger("pushing");
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

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))       //移動ANIMATION
        {
            animator.ResetTrigger("Idle");

            //若isPushingObject為TURE 代表角色正在推動物體  則不觸發WALKING
            if (isPushingObject)
            {
                animator.SetTrigger("pushing");
                animator.ResetTrigger("walking");
            }
            else 
            { 
                animator.SetTrigger("walking");
                animator.ResetTrigger("pushing");
            }


            /**********推移移動**********/
            if (isPushingObject)
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
            animator.ResetTrigger("pushing");

        }



        if (Input.GetMouseButton(0))        //左鍵 攻擊
        {
            animator.SetTrigger("Attacking");
        }

        else
        {
            animator.ResetTrigger("Attacking");
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
            if(Time.time- idletime>=0.1)animator.SetTrigger("Idle");           //空閒一段時間  才會設置IDLE TRIGGER
        }
        else
        {
            idletime = Time.time;                                                     //紀錄IDLE 當下的時間
        }


    }
}
