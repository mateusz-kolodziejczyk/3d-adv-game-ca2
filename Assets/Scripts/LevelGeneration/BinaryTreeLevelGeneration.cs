using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTreeLevelGeneration : MonoBehaviour
{
    const int NORTH = 1, SOUTH = 2, EAST = 3, WEST = 4;
    int[,] grid;

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

        grid = new int[width, height];
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

    void DisplayGrid()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (grid[col,row] == NORTH) gridObjectHor[col, row + 1].active = false;
                if (grid[col, row] == EAST) gridObjectVer[col, row + 1].active = false;
            }
        }
    }

    void GenerateMazeBinary()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                int carvingDir;
                float randomNum = Random.Range(0, 100);
                if (randomNum > 30)
                    carvingDir = NORTH;
                else
                    carvingDir = EAST;

                if (col == width - 1)
                {
                    carvingDir = NORTH;

                    if (row < height -1)
                        carvingDir = NORTH;
                    else
                        carvingDir = WEST;
                }

                else if (row == height - 1)
                {
                    if (col < width - 1)
                        carvingDir = EAST;
                    else
                        carvingDir = -1;
                }
                grid[col, row] = carvingDir;
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
