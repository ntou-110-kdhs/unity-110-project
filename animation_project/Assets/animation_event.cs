using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation_event : MonoBehaviour
{
    void active_animation()
    {
        gameObject.GetComponent<ParticleSystem>().Play();   //啟用特效
    }

    void unactive_animation()
    {
        gameObject.GetComponent<ParticleSystem>().Stop();   //啟用特效
    }
}
