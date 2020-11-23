﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    // 取得血條&褪去條的物件
    private Image bar = null;
    private Image fadeBar = null;

    // 設定最大血量
    [SerializeField] private float maxHp = 0;
    // 血量
    private float hp = 0;
    // 褪去速度
    [SerializeField] private float fadeSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        bar = transform.Find("Bar").GetComponentInChildren<Image>();
        fadeBar = transform.Find("FadeBar").GetComponent<Image>();
        hp = maxHp;        
    }

    // Update is called once per frame
    void Update()
    {
        setImageHpFadeBar();

        // debug();
    }

    /// <summary>
    /// 補血
    /// </summary>
    /// <param name="healthValue"> 補血數值 </param>
    public void isHealthing(float healthValue)
    {
        setHp(hp + healthValue);
    }

    /// <summary>
    /// 被攻擊
    /// </summary>
    /// <param name="damagedValue">  攻擊數值 </param>
    public void isDamaged(float damagedValue)
    {
        float newHp = hp - damagedValue;
        setHp(newHp);
        setFadeSpeed();
    }

    /// <summary>
    /// 設定HP
    /// </summary>
    /// <param name="hpValue"> HP</param>
    private void setHp(float hpValue)
    {        
        hp = hpValue;

        hp = Mathf.Max(hp, 0);
        hp = Mathf.Min(maxHp, hp);
  
        setImageHpBar();
    }
    /// <summary>
    /// 設定目前血條
    /// </summary>
    private void setImageHpBar()
    {
        bar.fillAmount = hp / maxHp;
    }

    /// <summary>
    /// 褪去血條設定
    /// </summary>
    private void setImageHpFadeBar()
    {
        if(fadeBar.fillAmount >= bar.fillAmount)
        {
            fadeBar.fillAmount -= Time.deltaTime * fadeSpeed;
        }
        else
        {
            fadeBar.fillAmount = bar.fillAmount;
        }

    }
    /// <summary>
    /// 改變褪去血條的速度
    /// </summary>
    private void setFadeSpeed()
    {
        fadeSpeed = fadeBar.fillAmount - bar.fillAmount;
    }
    

    /// <summary>
    /// Debug 用
    /// </summary>
    private void debug()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isDamaged(10);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            isHealthing(10);
        }
    }
}
