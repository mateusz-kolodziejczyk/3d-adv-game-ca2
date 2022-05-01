using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    float[,] map;

    [SerializeField]
    [Range(10, 100)]
    int mapHeight, mapWidth;

    [SerializeField]
    [Range(0, 100)]
    float blockSize, blockHeight, frequency, scale;

    public GameObject minecraftBlock;

    [SerializeField] private GameObject player;
    [SerializeField] private Vector2Int playerPosition;

    [SerializeField] private GameObject collectablePrefab;
    private CharacterController playerCharacterController;

    private CollectableManager collectableManager;

    private HashSet<Vector2Int> collectableLocations = new();
    void Start()
    {
        collectableManager = GameObject.FindWithTag("CollectableManager").GetComponent<CollectableManager>();
        playerCharacterController = player.GetComponent<CharacterController>();
        playerCharacterController.enabled = false;
        map = new float[mapWidth, mapHeight];
        minecraftBlock.transform.localScale = new Vector3(blockSize, blockHeight, blockSize);

        GenerateCollectableLocations();
        InitArray();
        DisplayArray();

        playerCharacterController.enabled = true;
    }

    private void GenerateCollectableLocations()
    {
        while (collectableLocations.Count < collectableManager.NCollectables)
        {
            var failuresInARow = 0;
            // Keep trying to add new collectable making sure its not in a position with a collectable and not on the player square.
            var pos = playerPosition;
            while (pos == playerPosition || collectableLocations.Contains(pos))
            {
                var x = Random.Range(0, mapHeight);
                var y = Random.Range(0, mapWidth);

                pos = new(x, y);
                failuresInARow += 1;
                if (failuresInARow > 100)
                {
                    return;
                }
            }
            collectableLocations.Add(pos);
        }
    }
    void InitArray()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                float nx = i / mapWidth;
                float ny = j / mapHeight;

                map[i, j] = Mathf.PerlinNoise(i * 1.0f / frequency + 0.1f, j * 1.0f / frequency + 0.1f);
            }
        }
    }

    void DisplayArray()
    {
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                GameObject t = Instantiate(minecraftBlock, new Vector3(i * blockSize, Mathf.Round(map[i, j] * blockHeight * scale), j * blockSize), Quaternion.identity);
                // Change players position if this is the player position
                Vector3 objectTransformPosition = new(i * blockSize, Mathf.Round(map[i, j] * blockHeight * scale) + 2,
                    j * blockSize);
                if (i == playerPosition.x && j == playerPosition.y)
                {
   
                    player.transform.position = objectTransformPosition;
                    Debug.Log(player.transform.position);
                }
                // Instantiate collectable if it exists at this location
                if (collectableLocations.Contains(new(i, j)))
                {
                    Instantiate(collectablePrefab, objectTransformPosition, Quaternion.identity);
                }
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
    }
}
