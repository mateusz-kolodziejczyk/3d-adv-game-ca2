using UnityEngine;

public class SpaceShipInput : MonoBehaviour
{
    private SpaceShip spaceShip;
    // Start is called before the first frame update
    void Start()
    {
        spaceShip = GetComponent<SpaceShip>();
    }

    // Update is called once per frame
    void Update()
    {
        (float vertical, float horizontal, float roll) inputAxes = (0, 0, 0);

        // Up
        if (Input.GetKey(KeyCode.W))
        {
            inputAxes.vertical -= 1;
        }
        // Left
        if (Input.GetKey(KeyCode.Q))
        {
            inputAxes.horizontal -= 1;
        }
        // Right
        if (Input.GetKey(KeyCode.E))
        {
            inputAxes.horizontal += 1;
        }
        // Down
        if (Input.GetKey(KeyCode.S))
        {
            inputAxes.vertical += 1;
        }
        
        // Roll
        if (Input.GetKey(KeyCode.A))
        {
            inputAxes.roll -= 1;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputAxes.roll += 1;
        }

        if (inputAxes.vertical != 0 || inputAxes.horizontal != 0 || inputAxes.roll != 0)
        {
            spaceShip.PointSpaceShip(inputAxes);
        }

        var forwards = 0f;
        // Forwads backwards
        if (Input.GetKey(KeyCode.Space))
        {
            forwards -= 1;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            forwards += 1;
        }

        if (forwards != 0)
        {
            spaceShip.MoveSpaceShip(forwards);
        }
    }
}
