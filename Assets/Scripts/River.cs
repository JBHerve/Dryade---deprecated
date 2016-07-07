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
        Segment newSegment = new Segment(node.position, new Vector2(point.x, point.z));
        foreach (var son in node.son)
        {
            Segment existingSegment = new Segment(node.position, son.position);


            //On veut savoir si[AB] coupe[A'B']
            //ceci est vrai ssi : 
            //->produit vectoriel (AB A'B') != 0 cad les droites ne sont pas parallèles(et aussi A'!=B', A != B). (cf ligne 99)
            //->ET produit vectoriel (AB, AB').produit vectoriel (AB,AA')<= 0 cad le point d'intersection est entre B' et A' (donc sur le segment [A'B']) (cf ligne 109 -> 111)
            //->ET produit vectoriel (A'B', A'B).produit vectoriel (A'B',A'A)<= 0 cad le point d'intersection est entre B et A (donc sur le segment [AB]) (cf ligne 113 -> 115)
            if (newSegment * existingSegment  != 0)
            {
                //TODO: Add a minimal distance
                Segment aux1 = new Segment(newSegment.A, existingSegment.B);
                Segment aux2 = new Segment(newSegment.A, existingSegment.A);
                if ((newSegment * aux1) * (newSegment * aux2) <= 0)
                {
                    aux1 = new Segment(existingSegment.A, newSegment.B);
                    aux2 = new Segment(existingSegment.A, newSegment.A);
                    if ((existingSegment * aux1) * (existingSegment * aux2) <= 0)
                    {
                        return false;
                    }
                }
            }
            if (!IsFarFromRiver(son, point))
            {
                return false;
            }
        }
        return true;
    }

    // This work for only ONE river
    public Vector3 GeneratePoint(Node father, Node root, List<Vector2> coast, Terrain terrain)
    {
        while (true)
        {
            float x = Random.Range(father.position.x - 20, father.position.x + 20);
            float z = Random.Range(father.position.z - 20, father.position.z + 20);
            float y = FromTerrainToWorld(0.2f, terrain);
            if (gameObject.GetComponent<Terrain>().SampleHeight(new Vector3(x, y, z)) > y - 1)
            {
                bool aux = true;
                for (int i = 0; i < root.son.Count && aux; i++)
                {
                    aux = aux && IsFarFromRiver(root.son[i], new Vector3(x, y, z));
                }
                if (aux)
                {
                    var lol = IsFarFromCoast(coast, new Vector3(x, y, z), 20);
                    if (lol)
                    {
                        return new Vector3(x, y, z);
                    }
                }
            }
        }
    }

    public float FromTerrainToWorld(float y, Terrain terrain)
    {
        return y * terrain.terrainData.size.y;
    }
}
