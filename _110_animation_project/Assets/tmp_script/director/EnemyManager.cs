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


}
