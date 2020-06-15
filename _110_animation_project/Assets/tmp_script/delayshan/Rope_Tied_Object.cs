using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 使用前  確保玩家的TAG為PLAYER  且此物件的tag為Rope_Tied_Object  且有將material附上物件的linerenderer

 將此函式放置在綁繩索的物體上  此函式將判斷玩家與此物體的距離 
 若小於rangeToShoot的距離  isPlayerInRange為TRUE
 若不夠近  isPlayerInRange為false

 isPlayerInRange會被Rope_Tied_Manager_Script使用  Rope_Tied_Manager_Script會抓取所有TAG為Rope_Tied_Object的物件  並且取用此函式

     */



public class Rope_Tied_Object : MonoBehaviour
{
    public Transform Rope_tied_objcet;
    public Transform player;
    public bool isPlayerInRange = false;
    [SerializeField]
    private float rangeToShoot = 2.5f;
    private GameObject [] playerarray;
    private LineRenderer tiedObejectLineRenderer=null;
    private Vector3[] tiedPoints=null;
    // Start is called before the first frame update
    void Start()
    {
        //獲取TAG為PLAYER的物件
        if (player == null) playerarray = GameObject.FindGameObjectsWithTag("Player");
        if (playerarray != null) { 
            player = playerarray[0].transform;
        }
        //獲取Rope_tied_objcet (綁繩子的點)
        if (null == this.transform.GetChild(0)) Debug.Log("Tied Object does not have a Tied point");
        else Rope_tied_objcet = this.transform.GetChild(0).transform;

        if (tiedObejectLineRenderer == null)
        {
            tiedObejectLineRenderer = this.GetComponentInChildren<LineRenderer>();
            tiedObejectLineRenderer.positionCount = 2;
            tiedObejectLineRenderer.startWidth = 0.05f;
            tiedObejectLineRenderer.SetPosition(0, Rope_tied_objcet.transform.position);
            tiedObejectLineRenderer.SetPosition(1, Rope_tied_objcet.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) Debug.Log("player not found");
        else
        {
            //計算物體與玩家間的距離  若小於rangeToShoot  則呼叫playerCanShoot() 更改狀態
            float distance = Vector3.Distance(player.transform.position, this.transform.position);
            //Debug.Log("distance with player==" + distance);
            if (distance < rangeToShoot) isPlayerInRange=true;
            else isPlayerInRange=false;
        }
    }


    //獲得玩家的LINERENDERER的所有結點  並且將第0個設為自己的位置  在玩家控制器的tiedRopeAnimationEnd取用
    public void getLineRendererPoints(Vector3[] array)
    {
        if (tiedObejectLineRenderer != null)
        {
            tiedObejectLineRenderer.SetPositions(array);
        }
        tiedObejectLineRenderer.SetPosition(0, Rope_tied_objcet.position);
    }

}
