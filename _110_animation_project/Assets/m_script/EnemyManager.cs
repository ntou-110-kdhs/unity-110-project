using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        findAllEnemiesInScene();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 找出所有在場景的敵人
    /// </summary>
    private void findAllEnemiesInScene()
    {
        GameObject[] enemyArr = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyArr)
        {
            enemies.Add(enemy);
        }
    }

    /// <summary>
    /// 替所有人設定
    /// </summary>
    /// <param name="target"></param>
    public void setAllEnemiesCheckTarget(Transform target)
    {
        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<FindRoadController>().SetCheckTarget(target);
        }
    }

    /// <summary>
    /// 設定場景中所有敵人的Outline Width
    /// 0 = 不顯示 Outline, 1 = 顯示
    /// </summary>
    /// <param name="outlineWidth"></param>
    public void setAllEnemiesOutline(int outlineWidth)
    {
        if(outlineWidth > 1)
        {
            outlineWidth = 1;
        }
        else if(outlineWidth < 0)
        {
            outlineWidth = 0;
        }



        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].GetComponent<Outline>().OutlineWidth = outlineWidth;
        }
    }


}
