using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Restart level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
