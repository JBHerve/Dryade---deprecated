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

        Vector2[] directions = new Vector2[8];

        IncrementObject.Increment[] increments = new IncrementObject.Increment[directions.Length];


        increments[0] = IncrementObject.GenerateIncrement(0, 1);
        increments[1] = IncrementObject.GenerateIncrement(1, 1);
        increments[2] = IncrementObject.GenerateIncrement(1, 0);
        increments[3] = IncrementObject.GenerateIncrement(1, -1);
        increments[4] = IncrementObject.GenerateIncrement(0, -1);
        increments[5] = IncrementObject.GenerateIncrement(-1, -1);
        increments[6] = IncrementObject.GenerateIncrement(-1, 0);
        increments[7] = IncrementObject.GenerateIncrement(-1, 1);

        for (int i = 0; i < directions.Length; ++i)
        {
            directions[i] = Maximise(father, delta, increments[i], coast);
        }

        float zero = 0;

        float xPos = 0;
        float xNeg = 1 / zero; ;
        float yPos = 0;
        float yNeg = 1 / zero; ;

        for (int i = 0; i < directions.Length; ++i)
        {
            xPos = Mathf.Max(xPos, directions[i].x);
            xNeg = Mathf.Min(xNeg, directions[i].x);
            yPos = Mathf.Max(yPos, directions[i].y);
            yNeg = Mathf.Min(yNeg, directions[i].y);
        }
        Debug.Log(new Vector4(xPos, yPos, xNeg, yNeg));
        return new Vector4(xPos, yPos, xNeg, yNeg);
    }

    public Vector2 Maximise(Vector2 vect, float delta, IncrementObject.Increment incr, List<Vector2> nodes)
    {
        float zero = 0;
        float dst = 1 / zero;
        foreach (var item in nodes)
        {
            dst = Mathf.Min(dst, Mathf.Sqrt(Mathf.Pow(item.x - vect.x, 2) + Mathf.Pow(item.y - vect.y, 2)));
        }

        Vector2 aux = vect;
        while (dst > delta && IsinTerrain(vect))
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

    //public Vector2 FarFromRiver(Node node, Vector2 point, IncrementObject.Increment incr)
    //{
    //    if (node.isALeaf())
    //    {
    //        return point;
    //    }
    //    Segment newSegment = new Segment(node.position, incr(point));
    //    List<Vector2> values = new List<Vector2>();
    //    foreach (var son in node.son)
    //    {
    //        Segment existingSegment = new Segment(node.position, son.position);

    //        //On veut savoir si[AB] coupe[A'B']
    //        //ceci est vrai ssi : 
    //        //->produit vectoriel (AB A'B') != 0 cad les droites ne sont pas parallèles(et aussi A'!=B', A != B). (cf ligne 99)
    //        //->ET produit vectoriel (AB, AB').produit vectoriel (AB,AA')<= 0 cad le point d'intersection est entre B' et A' (donc sur le segment [A'B']) (cf ligne 109 -> 111)
    //        //->ET produit vectoriel (A'B', A'B).produit vectoriel (A'B',A'A)<= 0 cad le point d'intersection est entre B et A (donc sur le segment [AB]) (cf ligne 113 -> 115)
    //        if (newSegment * existingSegment != 0)
    //        {
    //            //TODO: Add a minimal distance
    //            Segment aux1 = new Segment(newSegment.A, existingSegment.B);
    //            Segment aux2 = new Segment(newSegment.A, existingSegment.A);
    //            if ((newSegment * aux1) * (newSegment * aux2) <= 0)
    //            {
    //                aux1 = new Segment(existingSegment.A, newSegment.B);
    //                aux2 = new Segment(existingSegment.A, newSegment.A);
    //                if ((existingSegment * aux1) * (existingSegment * aux2) <= 0)
    //                {
    //                    values.Add(point);
    //                }
    //            }
    //            values.Add(FarFromRiver(son, newSegment.Coord, incr));
    //        }
    //    }
    //}

    // This work for only ONE river
    public Vector3 GeneratePoint(Node father, Node root, List<Vector2> coast, Terrain terrain)
    {
        Vector3 origin = father.position;
        Vector4 range = FarFromCoast(coast, 1, new Vector2(origin.x, origin.z));
        float x = Random.Range(range.z, range.x);
        float z = Random.Range(range.w, range.y);
        float y = FromTerrainToWorld(0.2f, terrain);
        return new Vector3(x, y, z);
    }

    public float FromTerrainToWorld(float y, Terrain terrain)
    {
        return y * terrain.terrainData.size.y;
    }
}
