using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCollision : MonoBehaviour
{   


    private void OnTriggerEnter(Collider other)
    {
        {
            if (other.CompareTag("Enemy"))
            {
                GameObject.FindWithTag("GameController").GetComponent<GameController>().RestartLevel();
            }
        }
    }
}
