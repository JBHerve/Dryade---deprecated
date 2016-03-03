using UnityEngine;
using System.Collections;

public class ProceduralTerrain : MonoBehaviour
{

    public float _H;
    public float _lacunarity; //The frequencies of the fractal. High lacunarity create compact relief
    public float _octaves; //Number of repetition of the fractal. Low number of octaves create soft relief
    public float _divider; //Divide all the point of the heightmap, lowering global height
    public float _offset; //Postition from sea level
    public float _seed;

    private bool first;
    private float[] exponent_array;
    private float[,] heightmap;


    //Dimension of the heigthmap
    private int width;
    private int height;

    // Use this for initialization
    void Start ()
    {
        width = gameObject.GetComponent<Terrain>().terrainData.heightmapWidth;
        height = gameObject.GetComponent<Terrain>().terrainData.heightmapHeight;
        heightmap = new float[width, height];

        first = true;

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.C))
        {
            SetHeightMap();
        }

    }

    void SetHeightMap()
    {
        for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
                heightmap[i, j] = Hetero_Terrain(new Vector2(i, j), _H, _lacunarity, _octaves, _offset);

        gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, heightmap);
    }

    float Hetero_Terrain(Vector2 point, float H, float lacunarity, float octaves, float offset)
    {
        int i = 0;


        float value = 0;
        float increment = 0;
        float frequency = 0;
        float remainder = 0;


        if (first)
        {
            exponent_array = new float[(int)octaves + 1];
            frequency = 1.0f;
            for (i = 0; i < octaves; ++i)
            {
                exponent_array[i] = Mathf.Pow(frequency, -H);
                frequency *= lacunarity; 
            }
            first = false;
        }

        value = offset + (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1) * Mathf.Pow(lacunarity, -H * i) / _divider;
        point *= lacunarity;

        for (i = 0; i < octaves; ++i)
        {
            increment = offset + (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1) * Mathf.Pow(lacunarity, -H * i) / _divider;
            increment *= exponent_array[i];
            increment *= value;

            value += increment;

            point *= lacunarity;
        }

        return value;
    }
}
