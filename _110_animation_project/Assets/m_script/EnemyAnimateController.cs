using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimateController : MonoBehaviour
{
    [SerializeField]
    private knight_damage_script KnightDamage;     //NPC攻擊傷害(武器上)


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
        Debug.Log("on attack");
        KnightDamage.Attacking();
    }

    public void attackFinished()                           //攻擊結束   停止傷害
    {
        KnightDamage.Stop_Attacking();
    }

    /***********攻擊動畫***********/



}
