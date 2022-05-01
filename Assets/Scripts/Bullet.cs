using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float maxTimeInAir = 5;
    private float timeInAir = 0;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
            Destroy(gameObject);
        }

        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        timeInAir += Time.deltaTime;
        if (timeInAir >= maxTimeInAir)
        {
            Destroy(gameObject);
        }
    }
}
