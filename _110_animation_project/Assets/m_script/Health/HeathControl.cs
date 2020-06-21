using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HeathControl : MonoBehaviour
{
    public float health = 100;
    public bool ishitted = false;
    // Start is called before the first frame update
    Animator animator;

    //目前這部分還不會用到

    /*
    public static Transform FindChild(Transform parent, string name)        //尋找子物件的函示  用於got_hit
    {
        Transform child = null;
        child = parent.Find(name);
        if (child != null)
            return child;
        Transform grandchild = null;
        for (int i = 0; i < parent.childCount; i++)
        {
            grandchild = FindChild(parent.GetChild(i), name);
            if (grandchild != null)
                return grandchild;
        }
        return null;
    }
    void got_blocked()
    {
        //Debug.Log("IN");
        //ishitted = true;
        gameObject.BroadcastMessage("spark_effect_control_active");                                    //在handsword中
        if(this.tag == ("enimy")) animator.Play("block_shaking_left");                                  //成功防禦動畫
        if(this.tag==("Player")) animator.Play("blocking_shake_right");
        Invoke("reset_hitted", 0.5f);
    }

    void got_parr()
    {
        //ishitted = true;
        gameObject.BroadcastMessage("spark_effect_control_active2");                                    //在handsword中
        if (this.tag == ("enimy")) animator.Play("block_shaking_left");                                  //成功防禦動畫
        if (this.tag == ("Player")) animator.Play("blocking_shake_right");
        Invoke("reset_hitted", 0.5f);
    }*/





    public void got_hit()
    {
        ishitted = true;


        /*
        if (this.tag == ("Player")) animator.Play("got_hit_1");                                         //受傷動畫
        else if (this.tag == ("enimy"))
        {
            animator.Play("got_hit_1");
        }*/


        health -= 25;
        gameObject.BroadcastMessage("updateHP");                //受傷後  呼叫血調控制的函式
        Invoke("reset_hitted", 0.5f);                           //受傷害後  一段時間將不會更動血條
    }

    void reset_hitted()
    {
        ishitted = false;
    }


    void Start()
    {
       animator = GetComponent<Animator>();
       // else if (this.tag == ("player")) animator = GetComponent<Transform>().GetChild(0).transform.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(gameObject + "  " + ishitted);
        if (health <= 0)
        {/*
            animator.SetTrigger("death");
            gameObject.BroadcastMessage("death");         */      //受傷後  呼叫血條控制的DISABLE
            gameObject.GetComponent<HeathControl>().enabled = false;
            gameObject.GetComponentInParent<NavMeshAgent>().enabled = false;
        }
    }


}
