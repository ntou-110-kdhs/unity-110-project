using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIconManager : MonoBehaviour
{

    [SerializeField] List<Sprite> iconLists = new List<Sprite>();
    private Transform target = null;

    private Image iconImg = null;
    private Transform rateImg = null;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        iconImg = GetComponent<Image>();
        rateImg = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        // 使 icon 面向攝影機
        Vector3 targetPostition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPostition);
    }

    /// <summary>
    /// 設定 state icon 圖樣
    /// </summary>
    /// <param name="index"> 目錄 </param>
    public void setIcon(int index)
    {
        iconImg.sprite = iconLists[index];
    }

    /// <summary>
    /// 設定頭上的Icon
    /// </summary>
    /// <param name="rate"> 百分比 </param>
    public void setIconRate(float rate)
    {
        Vector3 newPos = rateImg.localPosition;        
        newPos.y = -0.5f + (rate * 0.5f);
        rateImg.localPosition = newPos;
    }



}
