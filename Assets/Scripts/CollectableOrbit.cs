using UnityEngine;

public class CollectableOrbit : MonoBehaviour
{
        [SerializeField] private float baseOrbitalVelocity;
        [SerializeField] private float baseDistanceToPlanet;
        private float distanceToPlanet = 1;
        private float orbitalSpeed = 1;
        private float orbitalAngle = 0;
        public Color Color
        {
                get;
                set;
        } = Color.black;       
        public float DistanceToPlanet
        {
                get => distanceToPlanet; 
                set => distanceToPlanet = value * baseDistanceToPlanet;
        }

        public float Size { get; set; } = 1;
        
        public float OrbitalSpeed
        {
                get => orbitalSpeed;
                set => orbitalSpeed = value * baseOrbitalVelocity;
        }
        
        public Transform PlanetTransform { get; set; }

        public void ApplyAttributes(float planetSize)
        {
                distanceToPlanet += planetSize;

                var t = transform;
                t.position = new(distanceToPlanet, 0, distanceToPlanet);
                // apply scale
                t.localScale *= Size;
                // set color
                GetComponent<MeshRenderer>().material.color = Color;
        }
        private void Update()
        {
                if (PlanetTransform == null) return;
                
                orbitalAngle += Time.deltaTime * OrbitalSpeed;
                var position = PlanetTransform.position;
                var tempX = position.x + distanceToPlanet * Mathf.Cos(orbitalAngle);
                var tempZ = position.z + distanceToPlanet * Mathf.Sin(orbitalAngle);
                var tempY = position.y;
                transform.position = new(tempX, tempY, tempZ);
        }
}