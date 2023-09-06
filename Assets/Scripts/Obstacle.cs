using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private VisualEffect _cloudPoof;

    [SerializeField]
    private GameObject[] _rocks;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cloudPoof.Play();
            ShootRocks(other);
        }
    }

    private void ShootRocks(Collider other)
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject rock = Instantiate(_rocks[Random.Range(0, _rocks.Length)], transform.position, Quaternion.identity);
            rock.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 0.4f, -0.75f) * Random.Range(3, 15), ForceMode.Impulse);
        }
    }
}
