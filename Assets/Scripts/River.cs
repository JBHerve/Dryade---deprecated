using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RiverObjects;

public class River : MonoBehaviour
{

    List<Node> graph;
    public int seed;


    // Use this for initialization
    void Start()
    {
        Terraformer terraformer = new Terraformer(seed);
        Terrain terrain = gameObject.GetComponent<Terrain>();
        List<Vector2> coast = terraformer.SetHeightMap(terrain, 0.2f, 0.1f, 6, 6);
        foreach (var point in coast)
        {
            GameObject test = new GameObject();
            float coeffx = terrain.terrainData.size.x / terrain.terrainData.heightmapWidth;
            float coeffz = terrain.terrainData.size.z / terrain.terrainData.heightmapHeight;
            Vector3 root = new Vector3(point.x * coeffx, 0.2f, point.y * coeffz);
            test.transform.position = root;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
