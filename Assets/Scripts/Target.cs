using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField]
    private Transform destination, player;

    [SerializeField]
    private float minDistance;

    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) < minDistance)
        {
            LeanTween.move(gameObject, destination.position, 0.5f);
            enabled = false;
        }
    }
}
