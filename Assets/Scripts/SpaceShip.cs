using UnityEngine;

public class SpaceShip : MonoBehaviour
{
    [SerializeField] [Range(1, 1000)] private float horizontalRotationSpeed, verticalRotationSpeed, rollSpeed, flightSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PointSpaceShip((float vertical, float horizontal, float roll) inputAxes)
    {
        transform.Rotate(Vector3.up, verticalRotationSpeed * Time.deltaTime * inputAxes.horizontal, Space.Self);
        transform.Rotate(Vector3.left, horizontalRotationSpeed * Time.deltaTime * inputAxes.vertical, Space.Self);
        transform.Rotate(Vector3.back, rollSpeed * Time.deltaTime * inputAxes.roll, Space.Self);

    }

    public void MoveSpaceShip(float forwards)
    {
        transform.position += transform.forward * Time.deltaTime * flightSpeed * forwards;
    }
}
