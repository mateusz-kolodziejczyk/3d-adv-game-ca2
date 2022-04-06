using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Direction{
    NORTH,
    SOUTH,
    EAST,
    WEST,
    NONE,
}
public class MazeLevelGeneration : MonoBehaviour
{
    Dictionary<Direction,Vector2Int> directionAdditions = new Dictionary<Direction, Vector2Int> (){
                    {Direction.NORTH, new Vector2Int(0,-1)},
                    {Direction.EAST, new Vector2Int(1,0)},
                    {Direction.SOUTH, new Vector2Int(0,1)},
                    {Direction.WEST, new Vector2Int(-1,0)},
                    {Direction.NONE, new Vector2Int(0,0)}
                };
    Direction[,] grid;
    

    // For Kruskal
    Dictionary<Vector2Int, HashSet<Direction>> removedEdges = new Dictionary<Vector2Int, HashSet<Direction>>();
    Dictionary<(Vector2Int, Vector2Int), GameObject> walls = new Dictionary<(Vector2Int, Vector2Int), GameObject>();

    Queue<(Vector2Int, Vector2Int)> edgesToCheck = new Queue<(Vector2Int, Vector2Int)>();

    Dictionary<Vector2Int, System.Guid> cellGroups = new Dictionary<Vector2Int, System.Guid>();
    

    [SerializeField]
    [Range(5, 100)]
    int width, height, wallSize;

    [SerializeField]
    GameObject verticalWall, horizontalWall;

    GameObject[,] gridObjectHor, gridObjectVer;
    GameObject[] allObjectsInScene;

    float wallHeight; 

    private void Init()
    {
        height = width;
        wallHeight = 4;

        grid = new Direction[width, height];
        gridObjectVer = new GameObject[width + 1, height + 1];
        gridObjectHor = new GameObject[width + 1, height + 1];
        drawFullGrid();
        var ceiling = GameObject.FindWithTag("Ceiling");
        var floor = GameObject.FindWithTag("Floor");

        floor.transform.localScale = new Vector3((width + 1) * wallSize, floor.transform.localScale.y, (height + 1) * wallSize);
        ceiling.transform.localScale = new Vector3((width + 1) * wallSize, ceiling.transform.localScale.y, (height + 1) * wallSize);

        ceiling.transform.position = new Vector3(ceiling.transform.position.x, wallSize - 1, ceiling.transform.position.z);
        ceiling.active = false;
    }

    async void drawFullGrid() {

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                // if (i < height)
                // {
                //     float vWallSize = wallSize;
                //     float xOffset, zOffset;

                //     xOffset = -(width * vWallSize) / 2;
                //     zOffset = -(height * vWallSize) / 2;

                //     gridObjectVer[j, i] = Instantiate(verticalWall, new Vector3(-vWallSize / 2 + j * vWallSize + xOffset, wallSize / 2, i * vWallSize + zOffset), Quaternion.identity);
                //     gridObjectVer[j, i].transform.localScale = new Vector3(.1f, wallHeight, vWallSize);
                    
                //     gridObjectVer[j, i].active = true;

                // }

                // if (j < width)
                // {
                //     float hWallSize = wallSize;
                //     float xOffset, zOffset;
                //     xOffset = -(width * hWallSize) / 2;
                //     zOffset = -(height * hWallSize) / 2;

                //     gridObjectHor[j, i] = Instantiate(horizontalWall, new Vector3(j * hWallSize + xOffset, wallSize / 2, -(hWallSize / 2) + i * hWallSize + zOffset), Quaternion.identity);
                //     gridObjectHor[j, i].transform.localScale = new Vector3(hWallSize, wallHeight, .1f);

                //     gridObjectHor[j, i].active = true;
                // }

                // Creation algorithm steps
                // Start at first cell
                var cell = new Vector2Int(c, r);
                // Add it to the various collections
                // Add to set with uuid
                cellGroups[cell] = System.Guid.NewGuid();
                // Go through each adjacent cell
                foreach(var direction in (Direction[]) System.Enum.GetValues(typeof(Direction))){
                    if(direction == Direction.NONE){
                        continue;
                    }

                    var toAdd = directionAdditions[direction];
                    var adjacentCell = new Vector2Int(toAdd.x + c, toAdd.y + r);
                    // If has been added, continue;
                    if(cellGroups.TryGetValue(adjacentCell, out System.Guid value)){
                        continue;
                    }

                    var pos = new Vector3(r * wallSize, (float)wallSize / 2, c * wallSize);
                    var localScale = new Vector3(wallSize, wallHeight, wallSize);

                    // Instantiate new wall, making sure its oriented properly by a switch statement
                    switch(direction){
                        case Direction.WEST:
                            pos.x = pos.x + (float)wallSize / 2;
                            localScale.z = .1f;
                            break;
                        case Direction.EAST:
                            pos.x = pos.x + (float)wallSize / 2;
                            pos.z = pos.z + wallSize;
                            localScale.z = .1f;
                            break;
                        case Direction.SOUTH:
                            pos.z = pos.z + (float)wallSize / 2;
                            pos.x = pos.x + wallSize;
                            localScale.x = .1f;
                            break;
                        case Direction.NORTH:
                            pos.z = pos.z + (float)wallSize / 2;
                            localScale.x = .1f;
                            break;
                        default:
                            break;
                    }
                    var w = Instantiate(verticalWall, pos, Quaternion.identity);
                    w.transform.localScale = localScale;
                    w.active = true;
                }
                // Calculate offset for walls

                // Do not all adjacent cells if they have already been added(as the edge has already been added)
            }
        }
    }

    void DrawFullGridDict(){
        for(int r = 0; r < width; r++){
            for(int c = 0; c < height; c++){
                // Go through each direction separately once
                foreach(var direction in System.Enum.GetValues(typeof(Direction))){

                }
            }
        }
    }

    void DisplayGrid()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (grid[col,row] == Direction.NORTH) {
                    gridObjectVer[col, row].active = false;
                }
                if (grid[col, row] == Direction.WEST) {
                    gridObjectHor[col, row].active = false;
                }
            }
        }
    }

    void GenerateMazeBinary()
    {

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                var usableDirections = new List<Direction>{Direction.WEST, Direction.NORTH};
            
                float randomNum = Random.Range(0, 100);
                var index = randomNum > 50 ? 0 : 1;
                var direction = usableDirections[index];

                var offset = new Vector2Int(0,0);
                switch(direction){
                    case Direction.NORTH:
                        offset.y = 1;
                        break;
                    case Direction.WEST:
                        offset.x = 1;
                        break;
                    default:
                    break;
                }
                if(offset.x+col >= width || offset.y+row >= height ){
                    index = index == 0 ? 1 : 0;
                    direction = usableDirections[index];
                    offset = new Vector2Int(0,0);
                    switch(direction){
                        case Direction.NORTH:
                            offset.y = 1;
                            break;
                        case Direction.WEST:
                            offset.x = 1;
                            break;
                        default:
                            break;
                    }
                    if(offset.x+col >= width || offset.y+row >= height){
                        direction = Direction.NONE;
                    }

                }
                Debug.Log(direction);
                grid[col, row] = direction;

            }
        }
    }

    private void GenerateMazeKruskal(){

    }
    void Start()
    {
        Init();
        //GenerateMazeBinary();
        //DisplayGrid();
    }

    void Update()
    {
        
    }
}
