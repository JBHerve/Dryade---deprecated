using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RiverObjects;

public class River : MonoBehaviour
{
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
        GameObject test = new GameObject();
        test.transform.position = root.position;
        GameObject test2 = new GameObject();
        test2.transform.position = GeneratePoint(root, root, coast);
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
                root.AddSon(new Node(new Vector3(), root.priority - 1, root.flow));
                root.AddSon(new Node(new Vector3(), root.priority - 1, root.flow));
            }
            else
            {
                //Exension (Pc)
                root.AddSon(new Node(new Vector3(), root.priority, root.flow));
            }
        }
    }

    public bool IsFarFromCoast(List<Vector2> coast, Vector3 point, float delta)
    {
        foreach (var item in coast)
        {
            // Compare the vector between father and new point, and not only the new point
            float dst = Mathf.Sqrt(Mathf.Pow(item.x - point.x, 2) + Mathf.Pow(item.y - point.y, 2));
            if (dst < delta)
            {
                return false;
            }
        }
        return true;
    }

    public bool IsFarFromRiver(Node node, Vector3 point)
    {
        if (node.isALeaf())
        {
            return true;
        }
        foreach (var son in node.son)
        {
            // TODO : Add a minimal distance
            if (node.position - son.position == (point - son.position) + (node.position - point))
            {
                return false;
            }
            if (!IsFarFromRiver(son, point))
            {
                return false;
            }
        }
        return true;
    }

    // This work for only ONE river
    public Vector3 GeneratePoint(Node father, Node root, List<Vector2> coast)
    {
        var rand = new System.Random();

        while (true)
        {
            float x = Random.Range(father.position.x - 10, father.position.x + 10);
            float z = Random.Range(father.position.z - 10, father.position.z + 10);
            if (gameObject.GetComponent<Terrain>().SampleHeight(new Vector3(x, 0.2f, z)) > 0.2f)
            {
                if (IsFarFromRiver(root, new Vector3(x, 0.2f, z)))
                {
                    if (IsFarFromCoast(coast, new Vector3(x, 0.2f, z), 1))
                    {
                        return new Vector3(x, 0.2f, z);
                    }
                }
            }
        }
    }
}
