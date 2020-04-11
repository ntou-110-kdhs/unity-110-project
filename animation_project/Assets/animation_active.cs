using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation_active : MonoBehaviour
{
    void call_animation()                   //呼叫特效的component，用來啟用特效
    {
        gameObject.BroadcastMessage("active_animation");
    }

    void call_animation_to_stop()           //呼叫特效的component，用來關閉特效
    {
        gameObject.BroadcastMessage("unactive_animation");
    }
}
