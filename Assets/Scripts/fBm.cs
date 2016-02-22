using UnityEngine;
using System.Collections;

public class fBm : MonoBehaviour
{
    public float _H;
    public float _lacunarity;
    public float _octaves;
    public float[,] height;
    public float _offset;


    public Perlin noise;

    // Use this for initialization
    void Start()
    {
        noise = new Perlin();
        height = new float[Terrain.activeTerrain.terrainData.heightmapWidth, Terrain.activeTerrain.terrainData.heightmapHeight];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            noise = new Perlin();
            height = new float[Terrain.activeTerrain.terrainData.heightmapWidth, Terrain.activeTerrain.terrainData.heightmapHeight];
            for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
                for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
                    height[i, j] = FBM(new Vector2(i, j), _H, _lacunarity, _octaves);

            gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, height);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
                for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
                    height[i, j] = multifractal(new Vector2(i, j), _H, _lacunarity, _octaves, _offset);

            gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, height);
        }
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

            value += noise.getPoint(point) * Mathf.Pow(lacunarity, -H * i);
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
    /// Generate a number using multifractals
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
