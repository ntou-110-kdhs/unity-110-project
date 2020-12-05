using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimateController : MonoBehaviour
{
    [SerializeField]
    private knight_damage_script KnightDamage;     //NPC攻擊傷害(武器上)

    /************戰鬥***********/
    public bool Isblocking { get { return isblocking; } set { isblocking = value; } }    //角色是否正在防禦
    private bool isblocking = false;        //角色是否正在防禦
    /************戰鬥***********/


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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



}
