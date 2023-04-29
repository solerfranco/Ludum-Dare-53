using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    private bool turnedOver;

    public override void Interact()
    {
        if (!turnedOver)
        {
            LeanTween.rotateLocal(gameObject, new Vector3(-89.5f, 0, 0), 1f).setEaseOutBounce();
            turnedOver = true;
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
            turnedOver = false;
        }
    }
}
