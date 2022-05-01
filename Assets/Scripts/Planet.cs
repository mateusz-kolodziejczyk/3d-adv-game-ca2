using UnityEngine;
using TMPro;

public class Planet : MonoBehaviour
{
    [SerializeField] private TextMeshPro planetText;
    [SerializeField] private MeshRenderer planetRenderer;
    [SerializeField] private float baseOrbitalVelocity = 10f;
    [SerializeField] private float baseDistanceToSun = 150f;
    [SerializeField] private float orbitalAngle = 0.2f;
    [SerializeField] private float angle = 0f;
    [SerializeField] private float rotationalSpeed = 10f;
    [SerializeField] private float baseDiameter = 10;
    private float orbitalSpeed;
    private float distanceToSun;
    private float diameter;
    private GameObject sun;

    private const int LengthOfLineRenderer = 100;

    // Start is called before the first frame update
    public float RotationalSpeed { get => rotationalSpeed; set => rotationalSpeed = value; }

    public float OrbitalSpeed
    {
        get => orbitalSpeed;
        set => orbitalSpeed = value * baseOrbitalVelocity;
    }

    public float OrbitalAngle
    {
        get => orbitalAngle;
        set => orbitalAngle = value;
    }

    public float Angle
    {
        get => angle;
        set => angle = value;
    }

    public Color Color
    {
        get;
        set;
    } = Color.black;
    public float DistanceToSun { 
        get => distanceToSun;
        set => distanceToSun = baseDistanceToSun * value;
    }

    public string PlanetName { get; set; } = "";
    public float Diameter { 
        get => diameter;
        set => diameter = baseDiameter * value;
    }

    private Transform cameraTransform;
    private async void DrawOrbit(){
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        lineRenderer.SetColors(Color, Color);
        lineRenderer.SetWidth(1.0f, 1.0f);
        lineRenderer.SetVertexCount(LengthOfLineRenderer + 1);

        for (int i = 0; i <= LengthOfLineRenderer; i++){
            float unitAngle = (2f*Mathf.PI)/LengthOfLineRenderer;
            float currentAngle = unitAngle * i;
            Vector3 pos = new Vector3(DistanceToSun * Mathf.Cos(currentAngle), 0, DistanceToSun * Mathf.Sin(currentAngle));
            lineRenderer.SetPosition(i, pos);
        }
    }

    public void ApplyAttributes()
    {
        sun = GameObject.FindWithTag("Sun");
        distanceToSun += sun.transform.localScale.x;
        var t = transform;
        // Add sun's radius to distance so planets don't get stuck in the sun.
        t.position = new(DistanceToSun, 0, DistanceToSun);
        // apply scale
        t.localScale *= Diameter;
        planetText.text = PlanetName;
        // set color
        planetRenderer.material.color = Color;
        DrawOrbit();
    }
        void Start()
    {
        sun = GameObject.FindWithTag("Sun");
        cameraTransform = GameObject.FindWithTag("MainCamera").transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, RotationalSpeed * Time.deltaTime, Space.World);
        orbitalAngle += Time.deltaTime * OrbitalSpeed;
        var tempX = sun.transform.position.x + distanceToSun * Mathf.Cos(orbitalAngle);
        var tempZ = sun.transform.position.z + distanceToSun * Mathf.Sin(orbitalAngle);
        var tempY = sun.transform.position.y;
        transform.position = new(tempX, tempY, tempZ);
        // make the text face the player.
        planetText.transform.LookAt(cameraTransform);
    }


}
