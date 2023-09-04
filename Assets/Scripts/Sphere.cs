using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sphere : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<Collider> _onTriggerEnter;

    void OnTriggerEnter(Collider col)
    {
        if (_onTriggerEnter != null) _onTriggerEnter.Invoke(col);
    }
}
