using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

internal enum Direction{
    North,
    South,
    East,
    West,
    None,
}
public class MazeLevelGeneration : MonoBehaviour
{
    private readonly Dictionary<Direction,Vector2Int> directionAdditions = new(){
                    {Direction.North, new(0,-1)},
                    {Direction.East, new(1,0)},
                    {Direction.South, new(0,1)},
                    {Direction.West, new(-1,0)},
                    {Direction.None, new(0,0)}
                };

    private Direction[,] grid;
    

    // For Kruskal
    private Dictionary<Vector2Int, HashSet<Direction>> removedEdges = new();
    private Dictionary<(Vector2Int origin, Vector2Int destination), GameObject> walls = new();

    private Queue<(Vector2Int origin, Vector2Int destination)> edgesToCheck = new();

    private Dictionary<Vector2Int, Guid> cellGroups = new();

    private Dictionary<Guid, HashSet<Vector2Int>> sets = new();

    [SerializeField]
    [Range(2, 100)]
    private int width, height, wallSize;

    [SerializeField] private GameObject verticalWall, horizontalWall;

    private GameObject[,] gridObjectHor, gridObjectVer;
    private GameObject[] allObjectsInScene;

    [SerializeField] [Range(1, 50)] private int wallHeight; 

    private void Init()
    {
        grid = new Direction[width, height];
        gridObjectVer = new GameObject[width + 1, height + 1];
        gridObjectHor = new GameObject[width + 1, height + 1];
        DrawFullGrid();
        var ceiling = GameObject.FindWithTag("Ceiling");
        var floor = GameObject.FindWithTag("Floor");
        
        var xScale = (height + 1) * wallSize;
        var zScale = (width + 1) * wallSize;
        
        floor.transform.localScale = new(xScale, floor.transform.localScale.y, zScale);
        ceiling.transform.localScale = new(xScale, ceiling.transform.localScale.y, zScale);
        
        var xPosition = (height * wallSize)/2f + 2.5f;
        var zPosition =  (width * wallSize)/2f + 2.5f;
        
        ceiling.transform.position = new(xPosition, wallHeight, zPosition);
        ceiling.SetActive(false);

        floor.transform.position = new(xPosition, 0, zPosition);
    }

    private void DrawFullGrid() {

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
                foreach(var direction in (Direction[]) Enum.GetValues(typeof(Direction))){
                    
                    if(direction == Direction.None){
                        continue;
                    }

                    var toAdd = directionAdditions[direction];
                    var adjacentCell = new Vector2Int(toAdd.x + c, toAdd.y + r);
                    
                    // If has been added, continue;
                    // This makes sure that edges are not duplicated
                    if(cellGroups.TryGetValue(adjacentCell, out _)){
                        continue;
                    }

                    // Add the set of edges to "edges to check"
                    (Vector2Int origin, Vector2Int destination) edge = (cell, adjacentCell);
                    
                    // The edgesToCheck queue can contain edges on the maze wall as the algorithm will be trying to join two sets.
                    // As walls on the outside have only one adjacent cell, this is impossible.
                    
                    edgesToCheck.Enqueue(edge);
                    var pos = new Vector3(r * wallSize, (float)wallHeight / 2, c * wallSize);
                    var localScale = new Vector3(wallSize, wallHeight, wallSize);

                    // Instantiate new wall, making sure its oriented properly by a switch statement
                    switch(direction){
                        case Direction.West:
                            pos.x += (float)wallSize / 2;
                            localScale.z = .1f;
                            break;
                        case Direction.East:
                            pos.x += (float)wallSize / 2;
                            pos.z += wallSize;
                            localScale.z = .1f;
                            break;
                        case Direction.South:
                            pos.z += (float)wallSize / 2;
                            pos.x += wallSize;
                            localScale.x = .1f;
                            break;
                        case Direction.North:
                            pos.z += (float)wallSize / 2;
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

    private void DrawFullGridDict(){
        for(int r = 0; r < width; r++){
            for(int c = 0; c < height; c++){
                // Go through each direction separately once
                foreach(var direction in Enum.GetValues(typeof(Direction))){

                }
            }
        }
    }

    private void DisplayGrid()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (grid[col,row] == Direction.North) {
                    gridObjectVer[col, row].SetActive(false);
                }
                if (grid[col, row] == Direction.West) {
                    gridObjectHor[col, row].SetActive(false);
                }
            }
        }
    }

    private void GenerateMazeBinary()
    {

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var usableDirections = new List<Direction>{Direction.West, Direction.North};
            
                float randomNum = Random.Range(0, 100);
                var index = randomNum > 50 ? 0 : 1;
                var direction = usableDirections[index];

                var offset = new Vector2Int(0,0);
                switch(direction){
                    case Direction.North:
                        offset.y = 1;
                        break;
                    case Direction.West:
                        offset.x = 1;
                        break;
                    default:
                    break;
                }
                if(offset.x+col >= width || offset.y+row >= height ){
                    index = index == 0 ? 1 : 0;
                    direction = usableDirections[index];
                    offset = new(0,0);
                    switch(direction){
                        case Direction.North:
                            offset.y = 1;
                            break;
                        case Direction.West:
                            offset.x = 1;
                            break;
                        default:
                            break;
                    }
                    if(offset.x+col >= width || offset.y+row >= height){
                        direction = Direction.None;
                    }

                }
                Debug.Log(direction);
                grid[col, row] = direction;

            }
        }
    }

    private void GenerateMazeKruskal()
    {
        edgesToCheck = RandomiseQueue(edgesToCheck);
        while (edgesToCheck.Count > 0)
        {
            Debug.Log(edgesToCheck.Count);
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
        return new (queue.ToList().OrderBy(_ => Guid.NewGuid()));
    }

    private void Start()
    {
        Init();
        GenerateMazeKruskal();
        //GenerateMazeBinary();
        //DisplayGrid();
    }

    private void Update()
    {
        
    }
}
