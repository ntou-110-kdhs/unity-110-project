﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Animator animator;              //設置空的ANIMATOR變數
    float forward=0;
    float right = 0;

    private void Start()
    {
        animator=GetComponent<Animator>();      //指定此物件的ANIMATOR
    }


    void Update()
    {
        //當按下WASD時  觸發TRIGGER

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            animator.ResetTrigger("Idle");
            animator.SetTrigger("walking"); //觸發TRIGGER
            Debug.Log("RR");
            //根據不同的輸入 給予浮點數不同的值
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
            //根據不同的輸入 給予浮點數不同的值

        }
        else
        {
            animator.SetTrigger("Idle");
            animator.ResetTrigger("walking");//重製TRIGGER


            if (forward >= 0.1) forward -= 0.1f;
            else if (forward <= -0.1) forward += 0.1f;

            animator.SetFloat("forward", forward);      //將前進 後退 歸0
        }


        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) { }
        else
        {

            if (right >= 0.1) right -= 0.1f;
            else if (right <= -0.1) right += 0.1f;
            animator.SetFloat("right", right);
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
            animator.SetTrigger("Idle");
        }


    }
}
