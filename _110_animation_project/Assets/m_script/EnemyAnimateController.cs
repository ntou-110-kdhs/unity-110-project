using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimateController : MonoBehaviour
{
    [SerializeField]
    private knight_damage_script KnightDamage;     //NPC攻擊傷害(武器上)

    private Animator knightAnimator;

    /************戰鬥***********/
    public bool Isblocking { get { return isblocking; } set { isblocking = value; } }    //角色是否正在防禦
    private bool isblocking = false;        //角色是否正在防禦

    /************戰鬥***********/


    void Start()
    {
        knightAnimator = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (knightAnimator.GetBool("Engaging"))
        {
            knightAnimator.SetLayerWeight(2, 0);
        }
        else
        {
            knightAnimator.SetLayerWeight(2, 1);
        }
    }


    /***********攻擊動畫***********/
   
    public void attackStart()                           //攻擊開始   能開始傷害
    {
        //Debug.Log("on attack");
        KnightDamage.Attacking();
    }

    public void attackFinished()                           //攻擊結束   停止傷害
    {
        KnightDamage.Stop_Attacking();
    }

    /***********攻擊動畫***********/

    /***********狀態設定***********/
    /// <summary>
    /// 使NPC進入追擊動畫
    /// </summary>
    public void knightEngageAnimation()
    {
        knightAnimator.SetBool("Engaging", true);
    }

    /// <summary>
    /// 使NPC停止進入追擊動畫
    /// </summary>
    public void knightNotEngage()
    {
        knightAnimator.SetBool("Engaging", false);
    }

    /// <summary>
    /// 使NPC進入Idle
    /// </summary>
    public void knightIdle()
    {
        knightAnimator.SetBool("Idle", true);
    }

    /// <summary>
    /// 使NPC取消IDLE
    /// </summary>
    public void knightNotIdle()
    {
        knightAnimator.SetBool("Idle", false);
    }

    /// <summary>
    /// 使NPC進入Running
    /// </summary>
    public void knightRunning()
    {
        knightAnimator.SetBool("Running", true);
    }

    /// <summary>
    /// 使NPC取消Running
    /// </summary>
    public void knightNotRunning()
    {
        knightAnimator.SetBool("Running", false);
    }

    /// <summary>
    /// NPC受到攻擊時
    /// </summary>
    public void knightIsDamage()
    {
        if (Isblocking)
        {
            knightAnimator.Play("paladin_Shield_Impact", 1);
        }
        else
        {
            knightAnimator.Play("paladin_Shield_Stand_Impact", 1);
        }

 
        knightAnimator.SetLayerWeight(1, 0.55f);
        Invoke("knightDamageReset", 0.45f);
    }

    /// <summary>
    /// 重置NPC受到攻擊
    /// </summary>
    private void knightDamageReset()
    {
        knightAnimator.SetLayerWeight(1, 0);
    }

    /***********狀態設定***********/


}
