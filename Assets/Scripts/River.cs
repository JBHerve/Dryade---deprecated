using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RiverObjects;

public class River : MonoBehaviour
{
    public int seed;
    private Node root;
    private Node father;
    private List<Vector2> coast;

    // Use this for initialization
    void Start()
    {
        Terraformer terraformer = new Terraformer(seed);
        Terrain terrain = gameObject.GetComponent<Terrain>();
        coast = terraformer.SetHeightMap(terrain, 0.2f, 0.1f, 6, 6);
        float coeffx = terrain.terrainData.size.x / terrain.terrainData.heightmapWidth;
        float coeffz = terrain.terrainData.size.z / terrain.terrainData.heightmapHeight;

        //Node Construction
        var rand = new System.Random();

        var point = coast[rand.Next(coast.Count)];
        Vector3 position = new Vector3(point.x * coeffx, GetHeight(point), point.y * coeffz);
        root = new Node(position, rand.Next(), rand.Next());
        GameObject test = new GameObject();
        test.transform.position = root.position;
        father = root;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Terrain terrain = gameObject.GetComponent<Terrain>();
            GameObject test2 = new GameObject();
            test2.transform.position = GeneratePoint(father, root, coast, terrain);
            var tmp = new Node(test2.transform.position, 0, 0);
            father.AddSon(tmp);
            father = tmp;
        }
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




    public Vector4 FarFromCoast(List<Vector2> coast, float delta, Vector2 father)
    { 
        var incrY = IncrementObject.GenerateIncrement(0, 10);
        var incrX = IncrementObject.GenerateIncrement(10, 0);
        var incrx = IncrementObject.GenerateIncrement(-10, 0);
        var incry = IncrementObject.GenerateIncrement(0, -10);


        float xMax = Maximise(father, delta, incrX, coast).x;
        float xMin = Maximise(father, delta, incrx, coast).x;
        float yMax = Maximise(new Vector2(xMin, father.y), delta, incrY, coast).y;
        float yMin = Maximise(new Vector2(xMin, father.y), delta, incry, coast).y;

        for (float i = xMin;  xMax - i > 0; ++i)
        {
            yMax = Mathf.Min(yMax, Maximise(new Vector2(i, father.y), delta, incrY, coast).y);
            yMin = Mathf.Max(yMin, Maximise(new Vector2(i, father.y), delta, incry, coast).y);
        }
        
        return new Vector4(xMax, yMax, xMin, yMin);
    }

    public Vector2 Maximise(Vector2 vect, float delta, IncrementObject.Increment incr, List<Vector2> nodes)
    {
        float y = GetHeight(vect);
        float zero = 0;
        float dst = 1 / zero;
        Vector2 aux = incr(vect);
        foreach (var item in nodes)
        {
            dst = Mathf.Min(dst, Mathf.Sqrt(Mathf.Pow(item.x - aux.x, 2) + Mathf.Pow(item.y - aux.y, 2)));
        }

        while (/*dst >= delta && */IsinTerrain(aux) && GetHeight(aux) - y >= 0)
        {
            vect = aux;
            aux = incr(vect);
            foreach (var item in coast)
            {
                dst = Mathf.Min(dst, Mathf.Sqrt(Mathf.Pow(item.x - aux.x, 2) + Mathf.Pow(item.y - aux.y, 2)));
            }
        }
        return vect;
    }

    public bool IsinTerrain(Vector2 point)
    {
        Terrain terrain = gameObject.GetComponent<Terrain>();
        Vector3 origin = terrain.GetPosition();
        Vector3 size = terrain.terrainData.size;

        return point.x > origin.x && point.x < origin.x + size.x && point.y > origin.z && point.y < origin.z + size.z;
    }

    public float GetHeight(Vector2 point)
    {
        TerrainData terrainData = gameObject.GetComponent<Terrain>().terrainData;
        float coeffx = terrainData.size.x / terrainData.heightmapWidth;
        float coeffz = terrainData.size.z / terrainData.heightmapHeight;

        int x = (int) (point.x * coeffx);
        int y = (int) (point.y * coeffz);
        
        return terrainData.GetHeight(x, y);
    }


    public Vector2 FarFromRiver(Node root,Node node, Vector2 point, IncrementObject.Increment incr)
    {
        if (root.isALeaf())
        {
            return point;
        }
        Segment newSegment = new Segment(node.position, point);
        List<Vector2> values = new List<Vector2>();
        foreach (var son in root.son)
        {
            Segment existingSegment = new Segment(node.position, son.position);

            while (Segment.Cross(newSegment, existingSegment))
            {
                incr(point);
            }
            values.Add(FarFromRiver(son, node, point, incr));
        }
        float zero = 0;
        float x = 1 / zero;
        float y = 1 / zero;

        foreach (var vect in values)
        {
            x = Mathf.Min(x, vect.x);
            y = Mathf.Min(y, vect.y);
        }
        return new Vector2(x, y);
    }


    // This work for only ONE river
    public Vector3 GeneratePoint(Node father, Node root, List<Vector2> coast, Terrain terrain)
    {
        Vector3 origin = father.position;
        Vector4 range = FarFromCoast(coast, 10, new Vector2(origin.x, origin.z));
        //Vector2 tmp = FarFromRiver(root, father, new Vector2(range.x, range.y), IncrementObject.GenerateIncrement(-1, -1));
        //Vector2 tmp2 = FarFromRiver(root, father, new Vector2(range.z, range.w), IncrementObject.GenerateIncrement(1, 1));
        // = new Vector4(tmp.x, tmp.y, tmp2.x, tmp2.y);
        float x = Random.Range(range.z, range.x);
        float z = Random.Range(range.w, range.y);
        float y = GetHeight(new Vector2(x, z));
        return new Vector3(x, y, z);
    }

    public float FromTerrainToWorld(float y, Terrain terrain)
    {
        return y * terrain.terrainData.size.y;
    }
}
