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
public class BinaryTreeLevelGeneration : MonoBehaviour
{
    Direction[,] grid;

    Dictionary<Vector2Int, Dictionary<Direction, GameObject>> dictGrid;

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
    }

    async void drawFullGrid() {
        for (int i = 0; i <= height; i++)
        {
            for (int j = 0; j <= width; j++)
            {
                if (i < height)
                {
                    float vWallSize = wallSize;
                    float xOffset, zOffset;

                    xOffset = -(width * vWallSize) / 2;
                    zOffset = -(height * vWallSize) / 2;

                    gridObjectVer[j, i] = Instantiate(verticalWall, new Vector3(-vWallSize / 2 + j * vWallSize + xOffset, wallSize / 2, i * vWallSize + zOffset), Quaternion.identity);
                    gridObjectVer[j, i].transform.localScale = new Vector3(.1f, wallHeight, vWallSize);
                    
                    gridObjectVer[j, i].active = true;

                }

                if (j < width)
                {
                    float hWallSize = wallSize;
                    float xOffset, zOffset;
                    xOffset = -(width * hWallSize) / 2;
                    zOffset = -(height * hWallSize) / 2;

                    gridObjectHor[j, i] = Instantiate(horizontalWall, new Vector3(j * hWallSize + xOffset, wallSize / 2, -(hWallSize / 2) + i * hWallSize + zOffset), Quaternion.identity);
                    gridObjectHor[j, i].transform.localScale = new Vector3(hWallSize, wallHeight, .1f);

                    gridObjectHor[j, i].active = true;
                }
            }
        }
    }

    void DrawFullGridDict(){
        for(int r = 0; r < width; r++){
            for(int c = 0; c < height; c++){
                // Go through each direction separately once
                var directionAdditions = new Dictionary<Direction, Vector2Int> (){
                    {Direction.NORTH, new Vector2Int(1,0)},
                    {Direction.EAST, new Vector2Int(0,)},
                    {Direction.SOUTH, new Vector2Int(1,0)},
                    {Direction.WEST, new Vector2Int(1,0)},
                };
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

    void Start()
    {
        Init();
        GenerateMazeBinary();
        DisplayGrid();
    }

    void Update()
    {
        
    }
}
