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
        float coeffx = terrain.terrainData.size.x / terrain.terrainData.heightmapWidth;
        float coeffz = terrain.terrainData.size.z / terrain.terrainData.heightmapHeight;

        //Node Construction
        var rand = new System.Random();

        var point = coast[rand.Next(coast.Count)];
        Vector3 position = new Vector3(point.x * coeffx, 0.2f, point.y * coeffz);
        Node root = new Node(position, rand.Next(), rand.Next()); 

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Extend(Node root, List<Vector2> coast)
    {
        int Pa = 70;
        int Ps = Pa + 10;
        var rand = new System.Random();
        if (root.priority > 1)
        {
            int prob = rand.Next(0, 100);
            if (prob < Pa)
            {
                //Asymmmetrical extension (Pa)
                root.AddSon(new Node(new Vector3(), root.priority, root.flow));
                root.AddSon(new Node(new Vector3(), rand.Next(1, root.priority - 1), root.flow));
            }
            else if (prob < Ps)
            {
                //Symmetrical extension (Ps)
                root.AddSon(new Node(new Vector3(), root.priority, root.flow));
                root.AddSon(new Node(new Vector3(), root.priority, root.flow));
            }
            else
            {
                //Exension (Pc)
                root.AddSon(new Node(new Vector3(), root.priority, root.flow));
            }
        }
    }
}
