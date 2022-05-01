using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [SerializeField] private int nCollectables = 4;
    [SerializeField] private int scorePerCollectable;
    private int nCollected = 0;

    private GameController gameController;

    public int NCollectables
    {
        get => nCollectables;
        set => nCollectables = value;
    }

    private void Start()
    {
        gameController = GameObject.FindWithTag("GameController").GetComponent<GameController>();
    }

    public void Collect()
    {
        Debug.Log("Collectable Collected");
        gameController.AddToScore(scorePerCollectable);
        nCollected += 1;
        if (nCollected < nCollectables) return;
        
        gameController.AddToScore();
        gameController.AdvanceLevel();
    }
}
