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
        terraformer.SetHeightMap(terrain, 0.2f, 0.1f, 2, 6);
        System.Random rdm = new System.Random();
        Vector3 coord = terrain.transform.position;
        Vector3 max = terrain.terrainData.size;
        Vector2 position = new Vector2(rdm.Next((int)coord.x, (int)max.x), rdm.Next((int)coord.y, (int)max.y));
        while (!checkPosition(position, terrain))
        {
            position = new Vector2(rdm.Next((int)coord.x, (int)max.x), rdm.Next((int)coord.z, (int)max.z));
        }
        Node root = new Node(new Vector3(position.x, terrain.SampleHeight(position), position.y), rdm.Next(), rdm.Next());
        GameObject test = new GameObject();
        test.transform.position = root.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool checkPosition(Vector2 position, Terrain terrain)
    {
        if (terrain.SampleHeight(position) <= 0.19f * terrain.terrainData.size.y)
        {
            Vector2 test = new Vector2(position.x, position.y - 1);
            if (terrain.SampleHeight(test) < terrain.SampleHeight(position))
            {
                test.y = position.y + 1;
                if (terrain.SampleHeight(test) == terrain.SampleHeight(position))
                    return true;
                if (terrain.SampleHeight(test) < terrain.SampleHeight(position))
                {
                    test.y = position.y - 1;
                    if (terrain.SampleHeight(test) == terrain.SampleHeight(position))
                        return true;
                }
            }
            test.y = position.y;
            test.x = position.x - 1;
            if (terrain.SampleHeight(test) < terrain.SampleHeight(position))
            {
                test.x = position.x + 1;
                if (terrain.SampleHeight(test) == terrain.SampleHeight(position))
                    return true;
                if (terrain.SampleHeight(test) < terrain.SampleHeight(position))
                {
                    test.x = position.x - 1;
                    if (terrain.SampleHeight(test) == terrain.SampleHeight(position))
                        return true;
                }
            }
        }
        return false;
    }
}
