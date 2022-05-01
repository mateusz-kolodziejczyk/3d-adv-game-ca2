using UnityEngine;

public class Collectable : MonoBehaviour
{
    private CollectableManager collectableManager;

    private void Start()
    {
        collectableManager = GameObject.FindWithTag("CollectableManager").GetComponent<CollectableManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collectableManager.Collect();
            gameObject.SetActive(false);
        }
    }
}
