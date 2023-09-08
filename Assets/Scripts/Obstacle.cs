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

    [SerializeField]
    private AudioSource _crashAS;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _cloudPoof.Play();
            _crashAS.pitch = Random.Range(0.7f, 2f);
            _crashAS.Play();
            ShootRocks(other);
            LeanTween.scale(gameObject, Vector3.zero, 1f).setEaseInCubic();
        }
    }

    private void ShootRocks(Collider other)
    {
        if (_rocks.Length <= 0) return;
        for (int i = 0; i < 5; i++)
        {
            GameObject rock = Instantiate(_rocks[Random.Range(0, _rocks.Length)], transform.position, Quaternion.identity);
            rock.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1f, 1f), 0.4f, -0.75f) * Random.Range(3, 15), ForceMode.Impulse);
        }
    }
}
