using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.RiverObjects;
using Assets.Scripts.Tools;

public class River : MonoBehaviour
{
    public int seed;
    private Node root;

    private List<Vector2> coast;
    private TerrainData terrainData;
    Queue<Node> queue;
    // Use this for initialization
    void Start()
    {
        Terraformer terraformer = new Terraformer( seed );
        Terrain terrain = gameObject.GetComponent<Terrain>();
        coast = terraformer.SetHeightMap( terrain, 0.4f, 0.1f, 2.9f, 6 );
        terrainData = terrain.terrainData;
        float coeffx = terrainData.size.x / terrainData.heightmapWidth;
        float coeffz = terrainData.size.z / terrainData.heightmapHeight;

        //Node Construction
        var rand = new System.Random();

        var point = coast[rand.Next( coast.Count )];
        Vector3 position = new Vector3( point.x * coeffx, GetHeight(point), point.y * coeffz );
        root = new Node(position, rand.Next(), rand.Next());
        queue = new Queue<Node>();
        queue.Enqueue(root);
        GameObject tmp = new GameObject();
        tmp.transform.position = root.Position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            Queue<Node> aux = new Queue<Node>();
            while (queue.Count != 0)
            {
                Node tmpNode = queue.Dequeue();

                Extend(tmpNode, root);
                foreach (var son in tmpNode.Sons)
                {
                    Debug.DrawLine(tmpNode.Position, son.Position, Color.red, 3000);
                    GameObject tmp = new GameObject();
                    tmp.transform.position = son.Position;
                    aux.Enqueue(son);
                }
            }
            queue = aux;
        }
    }

    public void Extend(Node node, Node root)
    {
        int Pa = 70;
        int Ps = Pa + 10;
        var rand = new System.Random();
        Vector4 range = GenerateRange(node, root, coast);
        Debug.DrawLine(new Vector3(range.x, 400, range.y), new Vector3(range.x, 400, range.w), Color.blue, 3000);
        Debug.DrawLine(new Vector3(range.x, 400, range.w), new Vector3(range.z, 400, range.w), Color.green, 3000);
        Debug.DrawLine(new Vector3(range.z, 400, range.w), new Vector3(range.z, 400, range.y), Color.yellow, 3000);
        Debug.DrawLine(new Vector3(range.x, 400, range.y), new Vector3(range.z, 400, range.y), Color.magenta, 3000);
        if (root.Priority > 1)
        {
            int prob = rand.Next(0, 100);
            if (prob < Pa)
            {
                //Asymmmetrical extension (Pa)
                node.AddSon(new Node(GeneratePoint(range), node.Priority, node.Flow));
                node.AddSon(new Node(GeneratePoint(range), 1 /*Strahler ?*/, node.Flow));
            }
            else if (prob < Ps)
            {
                //Symmetrical extension (Ps)
                node.AddSon(new Node(GeneratePoint(range), node.Priority - 1, node.Flow));
                node.AddSon(new Node(GeneratePoint(range), node.Priority - 1, node.Flow));
            }
            else
            {
                //Exension (Pc)
                node.AddSon(new Node(GeneratePoint(range), node.Priority, node.Flow));
            }
        }
    }

    public Vector4 FarFromCoastRoot(Node root, float delta)
    {
        Vector2 fatherPos = new Vector2(root.Position.x, root.Position.z);
        
        float fatherHeight = GetHeight(fatherPos);
        float yMax = GetHeight(new Vector2(fatherPos.x, fatherPos.y + delta)) < fatherHeight ? fatherPos.y : fatherPos.y + delta;
        float xMax = GetHeight(new Vector2(fatherPos.x + delta, fatherPos.y)) < fatherHeight ? fatherPos.x : fatherPos.x + delta;
        float yMin = GetHeight(new Vector2(fatherPos.x, fatherPos.y - delta)) < fatherHeight ? fatherPos.y : fatherPos.y - delta;
        float xMin = GetHeight(new Vector2(fatherPos.x - delta, fatherPos.y)) < fatherHeight ? fatherPos.x : fatherPos.x - delta;


        if ( yMax != fatherPos.y )
        {
            var incrY = IncrementObject.GenerateIncrement(0, 1);
            yMax = Maximise(new Vector2(fatherPos.x, yMax), delta, incrY, coast).y;
        }
        if ( xMax != fatherPos.x )
        {
            var incrX = IncrementObject.GenerateIncrement(1, 0);
            xMax = Maximise(new Vector2(xMax, fatherPos.y), delta, incrX, coast).x;
        }
        if ( yMin != fatherPos.y )
        {
            var incry = IncrementObject.GenerateIncrement(0, -1);
            yMin = Maximise(new Vector2(fatherPos.x, yMin), delta, incry, coast).y;

        }
        if ( xMin != fatherPos.x )
        {
            var incrx = IncrementObject.GenerateIncrement(-1, 0);
            xMin = Maximise(new Vector2(xMin, fatherPos.y), delta, incrx, coast).x;
        }
                
        return new Vector4(xMax, yMax, xMin, yMin);

    }


    public Vector4 FarFromCoast(Vector2 father, float delta)
    {
        var incrY = IncrementObject.GenerateIncrement(0, 1);
        var incrX = IncrementObject.GenerateIncrement(1, 0);
        var incrx = IncrementObject.GenerateIncrement(-1, 0);
        var incry = IncrementObject.GenerateIncrement(0, -1);


        float xMax = Maximise(father, delta, incrX, coast).x;
        float xMin = Maximise(father, delta, incrx, coast).x;
        float yMax = Maximise(new Vector2(xMin, father.y), delta, incrY, coast).y;
        float yMin = Maximise(new Vector2(xMin, father.y), delta, incry, coast).y;

        for (float i = xMin + 1; xMax - 1 - i > 0; ++i)
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
            dst = Mathf.Min(VectorUtils.Distance(aux, item), dst);
        }

        while (dst >= delta && IsinTerrain(aux) && GetHeight(aux) - y >= 0)
        {
            vect = aux;
            aux = incr(vect);
            foreach (var item in coast)
            {
                dst = Mathf.Min(dst, VectorUtils.Distance(aux, item));
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
        return gameObject.GetComponent<Terrain>().SampleHeight(new Vector3(point.x, 0, point.y));
    }


    public Vector4 FarFromRiver(Node root, Node node, Vector4 range)
    {
        if (root.isLeaf())
        {
            return range;
        }
        foreach (var son in root.Sons)
        {
            if (root != node)
            {
                if (node.Position.x > son.Position.x)
                {
                    while (VectorUtils.Distance(root.Position, son.Position, new Vector2(range.x, node.Position.y)) < VectorUtils.Distance(node.Position.x, range.x) / Mathf.Sqrt(2))
                    {
                        --range.x;
                    }
                }
                else
                {
                    while (VectorUtils.Distance(root.Position, son.Position, new Vector2(range.z, node.Position.y)) < VectorUtils.Distance(node.Position.x, range.z) / Mathf.Sqrt(2))
                    {
                        ++range.z;
                    }
                }
                if (node.Position.y > son.Position.y)
                {
                    while (VectorUtils.Distance(root.Position, son.Position, new Vector2(node.Position.x, range.y)) < VectorUtils.Distance(node.Position.y, range.y) / Mathf.Sqrt(2))
                    {
                        --range.y;
                    }
                }
                else
                {
                    while (VectorUtils.Distance(root.Position, son.Position, new Vector2(node.Position.x, range.w)) < VectorUtils.Distance(node.Position.y, range.w) / Mathf.Sqrt(2))
                    {
                        ++range.w;
                    }
                }
            }

            range = FarFromRiver(son, node, range);

        }
        return range;
    }

    public Vector3 GeneratePoint(Vector4 range)
    {

        float x = Random.Range(range.z, range.x);
        float z = Random.Range(range.w, range.y);
        float y = GetHeight(new Vector2(x, z));
        return new Vector3(x, y, z);
    }

    public Vector4 GenerateRange(Node node, Node root, List<Vector2> coast)
    {
        Vector4 range;
        if (node.isRoot())
        {
            range = FarFromCoastRoot(node, 10);
        }
        else
        {
            range = FarFromCoast(node.Position, 10);
        }
        return range;
        //return FarFromRiver(root, node, range);
    }


    public float FromTerrainToWorld(float y, Terrain terrain)
    {
        return y * terrain.terrainData.size.y;
    }
}
