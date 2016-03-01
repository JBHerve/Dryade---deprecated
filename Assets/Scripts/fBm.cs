using UnityEngine;
using System.Collections;

public class fBm : MonoBehaviour
{
    public float _H;
    public float _lacunarity; //The frequencies of the fractal. High lacunarity create compact relief
    public float _octaves; //Number of repetition of the fractal. Low number of octaves create soft relief
    public float[,] heightmap;
    public float _divider; //Divide all the point of the heightmap, lowering global height
    public float _baseGround; //Set the height origin of the map. A high baseGround generate higher variation 

    public float _seed; //Genrate some randomness

    //Dimension of the heigthmap
    private int width;
    private int height;

    // Use this for initialization
    void Start()
    {
        width = gameObject.GetComponent<Terrain>().terrainData.heightmapWidth;
        height = gameObject.GetComponent<Terrain>().terrainData.heightmapHeight;
        heightmap = new float[width, height];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetHeightMap();
        }
    }

    void SetHeightMap()
    {
        for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
                heightmap[i, j] = FBM(new Vector2(i, j), _H, _lacunarity, _octaves);

        gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, heightmap);
    }

    /// <summary>
    /// Generate a number using fractal generation
    /// </summary>
    /// <param name="point"></param>
    /// <param name="H">1 - fractal increment</param>
    /// <param name="lacunarity">Gap between successive frequencies</param>
    /// <param name="octaves">Number of repetition of the fractal</param>
    /// <returns></returns>
    float FBM(Vector2 point, float H, float lacunarity, float octaves)
    {
        float value = 0;
        float remainder = 0;
        float weight = (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1 + _baseGround) * Mathf.Pow(lacunarity, -H * 0) / _divider;
        float signal = 0;
        int i = 0;

        for (; i < octaves; i++)
        {
            if (weight > 1.0f)
            {
                weight = 1.0f;
            }
            signal = (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1 + _baseGround) * Mathf.Pow(lacunarity, -H * i) / _divider;
            value += signal * weight;
            weight *= signal;
            point *= lacunarity;
        }

        remainder = octaves - (int)octaves;
        if (remainder != 0)
        {
            value += remainder * Mathf.PerlinNoise(point.x / width, point.y / height) * Mathf.Pow(lacunarity, -H * i);
        }
        return value;
    }
    
}
