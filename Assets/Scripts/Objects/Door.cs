using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    private bool open;

    public override void Interact()
    {
        if (!LeanTween.isTweening(gameObject))
        {
            LeanTween.rotateLocal(gameObject, new Vector3(0, open ? 0 : -90, 0), 0.3f);
            open = !open;
        }
    }
}
