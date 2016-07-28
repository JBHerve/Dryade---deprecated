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
        Vector3 position = new Vector3(point.x * coeffx, FromTerrainToWorld(0.2f, terrain), point.y * coeffz);
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

        Vector2[] directions = new Vector2[16];

        IncrementObject.Increment[] increments = new IncrementObject.Increment[directions.Length];


        increments[0] = IncrementObject.GenerateIncrement(0, 1);
        increments[1] = IncrementObject.GenerateIncrement(1, 1);
        increments[2] = IncrementObject.GenerateIncrement(1, 0);
        increments[3] = IncrementObject.GenerateIncrement(1, -1);
        increments[4] = IncrementObject.GenerateIncrement(0, -1);
        increments[5] = IncrementObject.GenerateIncrement(-1, -1);
        increments[6] = IncrementObject.GenerateIncrement(-1, 0);
        increments[7] = IncrementObject.GenerateIncrement(-1, 1);
        increments[8] = IncrementObject.GenerateIncrement(1, 0.5f);
        increments[9] = IncrementObject.GenerateIncrement(-1, 0.5f);
        increments[10] = IncrementObject.GenerateIncrement(0.5f, 1);
        increments[11] = IncrementObject.GenerateIncrement(0.5f, -1);
        increments[12] = IncrementObject.GenerateIncrement(-0.5f, -1);
        increments[13] = IncrementObject.GenerateIncrement(-0.5f, 1);
        increments[14] = IncrementObject.GenerateIncrement(1, -0.5f);
        increments[15] = IncrementObject.GenerateIncrement(-1, -0.5f);


        for (int i = 0; i < directions.Length; ++i)
        {
            directions[i] = Maximise(father, delta, increments[i], coast);
        }

        float xMax = Mathf.Min(directions[1].x, directions[2].x, directions[3].x, directions[8].x, directions[14].x, directions[10].x, directions[11].x);
        float xMin = Mathf.Max(directions[5].x, directions[6].x, directions[7].x, directions[9].x, directions[15].x, directions[12].x,directions[13].x);
        float yMax = Mathf.Min(directions[0].y, directions[1].y, directions[7].y, directions[10].y, directions[13].y, directions[8].y, directions[9].y);
        float yMin = Mathf.Max(directions[3].y, directions[4].y, directions[5].y, directions[11].y, directions[12].y, directions[14].y, directions[15].y);
        
        return new Vector4(xMax, yMax, xMin, yMin);
    }

    public Vector2 Maximise(Vector2 vect, float delta, IncrementObject.Increment incr, List<Vector2> nodes)
    {
        float y = GetHeight(vect);
        float zero = 0;
        float dst = 1 / zero;
        foreach (var item in nodes)
        {
            dst = Mathf.Min(dst, Mathf.Sqrt(Mathf.Pow(item.x - vect.x, 2) + Mathf.Pow(item.y - vect.y, 2)));
        }

        Vector2 aux = incr(vect);
        while (dst > delta && IsinTerrain(aux) && GetHeight(aux) - y >= 0)
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
        Vector4 range = FarFromCoast(coast, 20, new Vector2(origin.x, origin.z));
        //Vector2 tmp = FarFromRiver(root, father, new Vector2(range.x, range.y), IncrementObject.GenerateIncrement(-1, -1));
        //Vector2 tmp2 = FarFromRiver(root, father, new Vector2(range.z, range.w), IncrementObject.GenerateIncrement(1, 1));
        //range = new Vector4(tmp.x, tmp.y, tmp2.x, tmp2.y);
        float x = Random.Range(range.w, range.x);
        float z = Random.Range(range.z, range.y);
        float y = GetHeight(new Vector2(x, z));
        return new Vector3(x, y, z);
    }

    public float FromTerrainToWorld(float y, Terrain terrain)
    {
        return y * terrain.terrainData.size.y;
    }
}
