using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerMotor))]
public class enemyComputerControlling : MonoBehaviour
{
    public Interactable focus;
    public LayerMask movementMask;
    //Camera cam;
    PlayerMotor motor;
    enemyDetect detect;
    GameObject focusObject;
    // Start is called before the first frame update
    void Start()
    {
        //cam = Camera.main;
        motor = GetComponent<PlayerMotor>();
        detect = GetComponent<enemyDetect>();
    }

    // Update is called once per frame
    void Update()
    {
        Interactable interactable = detect.Alert();
        if (interactable == null)//return null
        {
            RemoveFocus();
            /*???*/
            //if (/*???*/false)
            //{
            //    //取消移動行動
            //    RemoveFocus();
            //}
        }
        else//if exist
        {
            Debug.Log("interactable:" + interactable);
            SetFocus(interactable);
            /*???*/
            //if (/*???*/false)
            //{

            //    //check if we hit an interactable
            //    //Debug.Log("We hit " + hit.collider.name + " " + hit.point);

            //    //Interactable interactable = hit.collider.GetComponent<Interactable>();

            //    //if we did set it as our focus
            //    //Debug.Log("interactable is null: " + (interactable == null));
            //    //if (interactable != null)
            //    //{
            //    //    SetFocus(interactable);
            //    //}
            //}
        }
    }
    void SetFocus(Interactable newFoucs)
    {
        if (newFoucs != focus)
        {
            if (focus != null)
                focus.DeFocused();
            focus = newFoucs;
            Debug.Log("motor.FollowTarget:" + newFoucs.name);
            motor.FollowTarget(newFoucs);
        }
        Debug.Log("newFoucs.name:" + newFoucs.name);
        Debug.Log("foucs.name:" + focus.name);
        newFoucs.OnFocused(transform);
    }
    void RemoveFocus()
    {
        if (focus != null)
            focus.DeFocused();
        focus = null;
        Debug.Log("motor.StopFollowingTarget");
        motor.StopFollowingTarget();
    }
}
