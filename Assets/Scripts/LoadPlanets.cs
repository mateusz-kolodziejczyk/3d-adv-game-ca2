using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Xml;
public class LoadPlanets : MonoBehaviour
{
    [SerializeField] private GameObject planetPrefab;
    [SerializeField] [Range(0, 1000)] private float timeModifier;
    [SerializeField] private GameObject collectablePrefab;

    private Dictionary<string, GameObject> planetsDictionary = new();

    private CollectableManager collectableManager;

    
    // Start is called before the first frame update
    void Start()
    {
        collectableManager = GameObject.FindWithTag("CollectableManager").GetComponent<CollectableManager>();
        LoadAllPlanets();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadAllPlanets(){
        var textAsset = (TextAsset) Resources.Load("planets");
        var doc = new XmlDocument();
        doc.LoadXml(textAsset.text);
        var planets = doc.SelectNodes("planets/planet");
        if (planets == null) return;
        // Select indexes for collectables.
        // Pick a random index out of the list of possible indexes.
        var indexes = new List<int>();
        indexes.AddRange(Enumerable.Range(0, planets.Count));
        // If there are more collectables than there are values in planets, make nCollectables = nplanets
        if (collectableManager.NCollectables > planets.Count)
        {
            collectableManager.NCollectables = planets.Count;
        }

        var indexHashSet = new HashSet<int>(indexes.OrderBy(x => Random.value).Take(collectableManager.NCollectables));

        for(var i = 0; i< planets.Count; i++)
        {
            var planet = planets[i];

            var planetAttributes = planet.Attributes;
            if (planetAttributes == null) continue;
            var planetName = planetAttributes.GetNamedItem("name").Value; 
            var diameter = float.Parse(planetAttributes.GetNamedItem("diameter").Value);
            var distanceToSun = float.Parse(planetAttributes.GetNamedItem("distancetoSun").Value);
            var rotationPeriod = float.Parse(planetAttributes.GetNamedItem("rotationPeriod").Value);
            var orbitalVelocity = float.Parse(planetAttributes.GetNamedItem("orbitalVelocity").Value);
        
            // Instantiate planet
            var planetObject = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity);
            planetsDictionary[planetName] = planetObject;
            
            // Get the planet component
            var planetScript = planetObject.GetComponent<Planet>();

            planetScript.PlanetName = planetName;
            planetScript.Diameter = diameter;
            planetScript.DistanceToSun = distanceToSun;
            // To get rotational speed from period, divide base speed by period.
            planetScript.RotationalSpeed /= rotationPeriod;
            planetScript.RotationalSpeed *= timeModifier;
            planetScript.OrbitalSpeed = orbitalVelocity * timeModifier;
            
            
            // Get colors
            var colour = planet.SelectSingleNode("colour");
            if (colour?.Attributes == null) continue;

            var colorAttributes = colour.Attributes;
            var r = float.Parse(colorAttributes.GetNamedItem("r").Value);
            var g = float.Parse(colorAttributes.GetNamedItem("g").Value);
            var b = float.Parse(colorAttributes.GetNamedItem("b").Value);

            planetScript.Color = new(r, g, b);
            Debug.Log(planetName);
            
            planetScript.ApplyAttributes();
            
            // Handle adding the collectable
            if (!indexHashSet.Contains(i)) continue;
            
            // Instantiate collectable
            var collectableObject = Instantiate(collectablePrefab, Vector3.zero, Quaternion.identity);
            var collectableOrbit = collectableObject.GetComponent<CollectableOrbit>();

            var collectable = planet.SelectSingleNode("collectable");

            var collectableColor = collectable?.SelectSingleNode("colour");
            if (collectableColor == null) return;

            var collectableAttributes = collectable.Attributes;
            var collectableColorAttributes = collectableColor.Attributes;

            if (collectableAttributes == null) return;
            if (collectableColorAttributes == null) return;
                
            // Attributes
            var collectableOrbitalVelocity = float.Parse(collectableAttributes.GetNamedItem("orbitalVelocity").Value);
            var collectableSize = float.Parse(collectableAttributes.GetNamedItem("size").Value);
            var collectableDistancePlanet = float.Parse(collectableAttributes.GetNamedItem("distancetoPlanet").Value);
                
            // Color
            var collectableR = float.Parse(collectableColorAttributes.GetNamedItem("r").Value);               
            var collectableG = float.Parse(collectableColorAttributes.GetNamedItem("g").Value);
            var collectableB = float.Parse(collectableColorAttributes.GetNamedItem("b").Value);


            collectableOrbit.Size = collectableSize;
            collectableOrbit.OrbitalSpeed = collectableOrbitalVelocity;
            collectableOrbit.DistanceToPlanet = collectableDistancePlanet;
            collectableOrbit.Color = new(collectableR, collectableG, collectableB);

            collectableOrbit.PlanetTransform = planetObject.transform;
            collectableOrbit.ApplyAttributes(planetScript.Diameter);
        }

    }
}
