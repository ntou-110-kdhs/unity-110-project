using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//這裡會用動畫EVENT去判斷是否正在攻擊   玩家部分是直接用動畫(在腳色移動中)  預計之後修改
public class knight_damage_script : MonoBehaviour
{
    public bool onAttack = false;
    public float damage_status = 1;
    public float damage_dealt = 25;
    HealthSystem player_HS =null;
    M_TestPlayerController other_PC;        //相當於charecter_movment

    private void Start()
    {
        player_HS = GameObject.Find("PlayerUI").GetComponentInChildren<HealthSystem>();
    }
    public void Attacking()             //在KNIGHT MOTION被使用
    {
        onAttack = true;
    }
    public void Stop_Attacking()
    {
        onAttack = false;
    }

    private int getangle(Transform target)          //判斷是否在指定角度內 是則傳1  否則傳0   目前都是以武器往上3階層
    {
        Transform this_obj = transform.parent.parent.parent;
        //Debug.Log("ohter=" + target.position +"this="+ this_obj.position);
        Vector3 targetDir = this_obj.position-target.position ;
        targetDir.y = 0;
        float angle = Vector3.Angle(target.forward,targetDir);
       // Debug.Log("knight"+ angle);
        if (angle <= 80.0f)
        {
            return 1;
        }
        else return 0;
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("aaa");

        if (other.tag == "Player")
        {
            other_PC = other.GetComponent<M_TestPlayerController>();
        }
        /*
        if (!other_HC.ishitted && onAttack == true && other.tag.Equals("Player") && other_PC.Isparring == true && getangle(other.transform) == 1)       //阻擋後對方的ishitted轉為TRUE 且判定是否是因阻擋區域造成碰撞   
        {
            Debug.Log("parr");
            other_HC.ishitted = true;
            other.SendMessageUpwards("got_parr");                   // got_blocked  在HC裡

        }
        */
        if (!player_HS.Ishitted && onAttack == true && other.tag.Equals("Player") && other_PC.Isblocking == true && getangle(other.transform) == 1)       //阻擋後對方的ishitted轉為TRUE 且判定是否是因阻擋區域造成碰撞  以及當前是否為防禦中
        {
                Debug.Log("BLOCKED");
            //Debug.Log("before blocked"+other_HC.ishitted);
            player_HS.Ishitted = true;
            //Debug.Log("after blocked" + other_HC.ishitted);
            other.SendMessageUpwards("got_blocked");                   // got_blocked  在HC裡

        }
        else if (!player_HS.Ishitted && onAttack == true && other.tag.Equals("Player"))
        {
            //Debug.Log("before hit" + other_HC.ishitted);
            player_HS.Ishitted = true;
            Debug.Log("after hit" );
            player_HS.isDamaged(20);
        }
        

    }

}
