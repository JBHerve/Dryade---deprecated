using UnityEngine;

public class HeterogeneousTerrain : MonoBehaviour
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
    }
    

    void SetHeightMap()
    {
        for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
            {
                heightmap[i, j] = HybridMultifractal(new Vector2(i, j), _H, _lacunarity, _octaves, _offset);
            }


        first = true;
        gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, heightmap);
    }

    float Hetero_Terrain(Vector2 point, float H, float lacunarity, float octaves, float offset)
    {
        int i = 0;


        float value = 0;
        float increment = 0;
        float frequency = 0;


        if (first)
        {
            exponent_array = new float[(int)octaves + 1];
            frequency = 1.0f;
            for (i = 0; i <= octaves; ++i)
            {
                exponent_array[i] = Mathf.Pow(frequency, -H);
                frequency *= lacunarity; 
            }
            first = false;
        }

        value = offset + (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1) / _divider;
        point *= lacunarity;

        for (i = 0; i <= octaves; ++i)
        {
            increment = offset + (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1) / _divider;
            increment *= exponent_array[i];
            increment *= value;

            value += increment;

            point *= lacunarity;
        }

        return value;
    }

    float HybridMultifractal(Vector2 point, float H, float lacunarity, float octaves, float offset)
    {
        int i = 0;


        float result = 0;
        float signal = 0;
        float frequency = 0;
        float weight = 0;


        if (first)
        {
            exponent_array = new float[(int)octaves + 1];
            frequency = 1.0f;
            for (i = 0; i <= octaves; ++i)
            {
                exponent_array[i] = Mathf.Pow(frequency, -H);
                frequency *= lacunarity;
            }
            first = false;
        }

        result = offset + (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1) * exponent_array[0] / _divider;
        weight = result;

        for (i = 0; i < octaves; ++i)
        {
            if (weight > 1.0f)
            {
                weight = 1.0f;
            }
            signal = offset + (Mathf.PerlinNoise(point.x / width + _seed, point.y / height + _seed) * 2 - 1)/ _divider;
            signal *= exponent_array[i];

            result += weight * signal;

            weight *= signal;

            point *= lacunarity;
        }

        return result;
    }


}
