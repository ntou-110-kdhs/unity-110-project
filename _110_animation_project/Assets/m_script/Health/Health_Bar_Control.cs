using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Bar_Control : MonoBehaviour
{

    float currentHP = 0;
    Transform grandfather;
    // Start is called before the first frame update
    void Start()
    {
        grandfather = gameObject.transform.parent.parent;
        currentHP = grandfather.GetComponent<HeathControl>().health;
    }

    // Update is called once per frame
    void updateHP()
    {
        currentHP = grandfather.GetComponent<HeathControl>().health;
        gameObject.transform.GetChild(0).GetComponent<SimpleHealthBar>().UpdateBar(currentHP / 100f, 1.0f);
    }

    void death()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {

        transform.LookAt(Camera.main.transform);
    }
}

/*
 public class Health_Bar_Control : MonoBehaviour
{

    float currentHP=0;
    Transform grandfather;
    // Start is called before the first frame update
    void Start()
    {
        grandfather = gameObject.transform.parent.parent;
        currentHP = grandfather.GetComponent<HeathControl>().health;
    }

    // Update is called once per frame
    void updateHP()
    {
        currentHP = grandfather.GetComponent<HeathControl>().health;
        gameObject.transform.GetChild(0).GetComponent<SimpleHealthBar>().UpdateBar(currentHP / 100f, 1.0f);
    }

    void death()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }

    void Update()
    {
        
        transform.LookAt(Camera.main.transform);
    }
}
     
     */
