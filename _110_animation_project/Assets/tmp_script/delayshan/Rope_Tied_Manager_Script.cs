using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Rope_Tied_Manager_Script會抓取所有TAG為Rope_Tied_Object的物件  並且取用Rope_Tied_Object中的isPlayerInRange
 若有任何一個Rope_Tied_Object物件距離玩家夠近  則會改變PlayerController中的ableToShoot 且同時傳遞該物件的Rope_tied_objcet_start(綁繩子的點)
     
     */




public class Rope_Tied_Manager_Script : MonoBehaviour
{
    public Transform player;
    private GameObject[] playerarray;
    private PlayerController playerController;
    private GameObject[] allRTObjectArray;
    private bool isAbleToShoot = false;
    private int inRangeTarget=0;
    // Start is called before the first frame update
    void Start()
    {
        allRTObjectArray = GameObject.FindGameObjectsWithTag("Rope_Tied_Object");
        //獲取TAG為PLAYER的物件
        if (player == null) playerarray = GameObject.FindGameObjectsWithTag("Player");
        if (playerarray != null)
        {
            player = playerarray[0].transform;
            playerController = player.GetComponent<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (allRTObjectArray != null)
        {
            //在每貞開始時 將isAbleToShoot改為FALSE  之後開始判斷所有的Rope_Tied_Object的isPlayerInRange有沒有為true的  若有就將isAbleToShoot改為true
            isAbleToShoot = false;
            int len = allRTObjectArray.Length;
            for (int i = 0; i < len; i++)
            {
                if (allRTObjectArray[i].GetComponent<Rope_Tied_Object>().isPlayerInRange)
                {
                    inRangeTarget = i;
                    isAbleToShoot = true;
                }
            }
        }
    }

    //每貞結束時  判斷isAbleToShoot是否為true   若是則呼叫playerCanShoot   反之呼叫playerCanNotShoot
    private void LateUpdate()
    {
        if (isAbleToShoot) playerCanShoot();
        else playerCanNotShoot();
    }


    //更改玩家當前狀態   代表玩家可以使用射箭   並且傳遞在範圍內的物件
    public void playerCanShoot()
    {
        playerController.ableToShoot(allRTObjectArray[inRangeTarget].transform);
    }

    //更改玩家當前狀態   代表玩家不可以使用射箭
    public void playerCanNotShoot()
    {
        playerController.unAbleToShoot();
    }

}
