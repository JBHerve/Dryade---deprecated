using UnityEngine;
using System.Collections;

public class fBm : MonoBehaviour
{
    public float _H;
    public float _lacunarity;
    public float _octaves;
    public float[,] heightmap;
    public float _offset;

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
        int i = 0;

        for (; i < octaves; i++)
        {

            value += (Mathf.PerlinNoise(point.x / width, point.y / height) * 2 - 1) * Mathf.Pow(lacunarity, -H * i);
            point *= lacunarity;
        }

        remainder = octaves - (int)octaves;
        if (remainder != 0)
        {
            value += remainder * Mathf.PerlinNoise(point.x / Terrain.activeTerrain.terrainData.heightmapWidth, point.y / Terrain.activeTerrain.terrainData.heightmapHeight) * Mathf.Pow(lacunarity, -H * i);
        }
        return value;
    }

    /// <summary>
    /// Generate a number using multifractals.
    /// </summary>
    /// <param name="point"></param>
    /// <param name="H">1 - fractal increment</param>
    /// <param name="lacunarity">Gap between successive frequencies</param>
    /// <param name="octaves">Number of repetition of the fractal</param>
    /// <param name="offset">Control the multifractaly. The closet it's from 0, the more heterogenous the fractal is</param>
    /// <returns></returns>
    float multifractal(Vector2 point, float H, float lacunarity, float octaves, float offset)
    {
        float value;

        value = 1;

        for (int i = 0; i < octaves; i++)
        {
            value *= ((Mathf.PerlinNoise(point.x / Terrain.activeTerrain.terrainData.heightmapWidth, point.y / Terrain.activeTerrain.terrainData.heightmapHeight) * 2 - 1) + offset) * Mathf.Pow(lacunarity , -H * i);
            point.x *= lacunarity;
            point.y *= lacunarity;
        }
        return value;
    }
}
