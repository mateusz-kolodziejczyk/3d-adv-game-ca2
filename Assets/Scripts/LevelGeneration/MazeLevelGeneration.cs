using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using Random = UnityEngine.Random;

internal enum Direction
{
    North,
    South,
    East,
    West,
    None,
}

public class MazeLevelGeneration : MonoBehaviour
{
    private readonly Dictionary<Direction, Vector2Int> directionAdditions = new()
    {
        {Direction.North, new(0, -1)},
        {Direction.East, new(1, 0)},
        {Direction.South, new(0, 1)},
        {Direction.West, new(-1, 0)},
        {Direction.None, new(0, 0)}
    };

    // For Kruskal
    private Dictionary<(Vector2Int origin, Vector2Int destination), GameObject> walls = new();

    private Queue<(Vector2Int origin, Vector2Int destination)> edgesToCheck = new();

    private Dictionary<Vector2Int, Guid> cellGroups = new();

    private Dictionary<Guid, HashSet<Vector2Int>> sets = new();

    // Maze generation settings
    [SerializeField] [Range(2, 100)] private int width, height, wallSize;

    // Prefabs
    [SerializeField] private GameObject verticalWall;

    [SerializeField] [Range(1, 50)] private int wallHeight;
    private GameObject floor;

    [SerializeField] private Vector2Int playerLocation;
    [SerializeField] private GameObject player;

    [SerializeField] [Range(0, 20)] int numberOfEnemies;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject collectablePrefab;

    private void Init()
    {
        DrawFullGrid();
        floor = GameObject.FindWithTag("Floor");

        var xScale = (height + 1) * wallSize;
        var zScale = (width + 1) * wallSize;

        floor.transform.localScale = new(xScale, floor.transform.localScale.y, zScale);

        var xPosition = (height * wallSize) / 2f + 2.5f;
        var zPosition = (width * wallSize) / 2f + 2.5f;

        floor.transform.position = new(xPosition, 0, zPosition);
    }

    private void DrawFullGrid()
    {
        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                // Creation algorithm steps
                // Start at first cell
                var cell = new Vector2Int(c, r);
                // Add it to the various collections
                // Add to set with uuid
                var guid = Guid.NewGuid();
                cellGroups[cell] = guid;
                sets[guid] = new() {cell};
                // Go through each adjacent cell
                foreach (var direction in (Direction[]) Enum.GetValues(typeof(Direction)))
                {
                    if (direction == Direction.None)
                    {
                        continue;
                    }

                    var toAdd = directionAdditions[direction];
                    var adjacentCell = new Vector2Int(toAdd.x + c, toAdd.y + r);

                    // If has been added, continue;
                    // This makes sure that edges are not duplicated
                    if (cellGroups.TryGetValue(adjacentCell, out _))
                    {
                        continue;
                    }

                    // Add the set of edges to "edges to check"
                    (Vector2Int origin, Vector2Int destination) edge = (cell, adjacentCell);

                    // The edgesToCheck queue can contain edges on the maze wall as the algorithm will be trying to join two sets.
                    // As walls on the outside have only one adjacent cell, this is impossible.

                    edgesToCheck.Enqueue(edge);
                    var pos = new Vector3(r * wallSize, (float) wallHeight / 2, c * wallSize);
                    var localScale = new Vector3(wallSize, wallHeight, wallSize);

                    // Instantiate new wall, making sure its oriented properly by a switch statement
                    switch (direction)
                    {
                        case Direction.West:
                            pos.x += (float) wallSize / 2;
                            localScale.z = .1f;
                            break;
                        case Direction.East:
                            pos.x += (float) wallSize / 2;
                            pos.z += wallSize;
                            localScale.z = .1f;
                            break;
                        case Direction.South:
                            pos.z += (float) wallSize / 2;
                            pos.x += wallSize;
                            localScale.x = .1f;
                            break;
                        case Direction.North:
                            pos.z += (float) wallSize / 2;
                            localScale.x = .1f;
                            break;
                        default:
                            break;
                    }

                    var w = Instantiate(verticalWall, pos, Quaternion.identity);
                    w.transform.localScale = localScale;
                    w.SetActive(true);
                    walls[edge] = w;
                }
            }
        }
    }


    private void GenerateMazeKruskal()
    {
        edgesToCheck = RandomiseQueue(edgesToCheck);
        while (edgesToCheck.Count > 0)
        {
            // Get next edge
            var (origin, destination) = edgesToCheck.Dequeue();

            // Check if the two cellgroup entries actually exist;
            if (!cellGroups.TryGetValue(origin, out var firstGroup)) continue;
            if (!cellGroups.TryGetValue(destination, out var secondGroup)) continue;

            // Check if they are in the same set
            if (firstGroup == secondGroup) continue;

            // If the edges pass all the checks, remove the wall and combine the sets.
            RemoveWall((origin, destination));
            sets[firstGroup].UnionWith(sets[secondGroup]);

            // Set the cellgroups on all the secondgroup elements to the first group    
            var secondSet = sets[secondGroup];

            foreach (var cell in secondSet)
            {
                if (cellGroups.TryGetValue(cell, out var _))
                {
                    cellGroups[cell] = firstGroup;
                }
            }

            // Remove the second set
            sets.Remove(secondGroup);
        }

        Debug.Log("Loop has been run");
        Debug.Log($"No. Sets: {sets.Values.Count}");
    }

    private void RemoveWall((Vector2Int origin, Vector2Int destination) edge)
    {
        if (walls.TryGetValue(edge, out var wall))
        {
            wall.SetActive(false);
        }
    }

    private Queue<T> RandomiseQueue<T>(Queue<T> queue)
    {
        return new(queue.ToList().OrderBy(_ => Guid.NewGuid()));
    }

    private void Start()
    {
        Init();
        GenerateMazeKruskal();
        BakeFloorNavmesh();
        PopulateWithEnemies();
        PopulateWithCollectables();
        SetPlayerLocation();
    }

    private void BakeFloorNavmesh()
    {
        floor.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    private void PopulateWithEnemies()
    {
        var failuresInARow = 0;
        var spawnedPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < numberOfEnemies; i++)
        {
            if (failuresInARow > 50)
            {
                return;
            }
            // Pick a  random location, if it has an enemy on it try again
            var pos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            if (spawnedPositions.Contains(pos) || playerLocation == pos)
            {
                failuresInARow += 1;
                i--;
                continue;
            }
            
            failuresInARow = 0;
            // Spawn the enemy.
            var enemyObject = Instantiate(enemyPrefab, CellLocationToWorldPosition(pos, wallHeight), Quaternion.identity);
            enemyObject.GetComponent<AICharacterControl>().target = player.transform;

        }
    }

    private void PopulateWithCollectables()
    {
        var collectableManager = GameObject.FindWithTag("CollectableManager").GetComponent<CollectableManager>();

        var failuresInARow = 0;
        var spawnedPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < collectableManager.NCollectables; i++)
        {
            if (failuresInARow > 50)
            {
                return;
            }

            // Pick a  random location, if it has an enemy on it try again
            var pos = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            if (spawnedPositions.Contains(pos) || playerLocation == pos)
            {
                failuresInARow += 1;
                i--;
                continue;
            }

            failuresInARow = 0;
            // Spawn the collectable.
            Instantiate(collectablePrefab, CellLocationToWorldPosition(pos, wallHeight/2), Quaternion.identity);
        }
    }

    private void SetPlayerLocation()
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = CellLocationToWorldPosition(playerLocation, wallHeight);
        player.GetComponent<CharacterController>().enabled = true;
    }

    private Vector3 CellLocationToWorldPosition(Vector2Int pos, float y)
    {
        return new(pos.x * wallSize + wallSize/2, y, pos.y * wallSize + wallSize/2);
    }
}