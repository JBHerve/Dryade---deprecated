using UnityEngine;

public class HeterogeneousTerrain : MonoBehaviour
{

    public float H_;
    public float lacunarity_; //The frequencies of the fractal. High lacunarity create compact relief
    public float octaves_; //Number of repetition of the fractal. Low number of octaves create soft relief
    public float divider_; //Divide all the point of the heightmap, lowering global height
    public float offset_; //Postition from sea level
    public float seed_;

    private bool first_;
    private float[] exponent_array_;
    private float[,] heightmap_;


    //Dimension of the heigthmap
    private int width;
    private int height;

    // Use this for initialization
    void Start ()
    {
        width = gameObject.GetComponent<Terrain>().terrainData.heightmapWidth;
        height = gameObject.GetComponent<Terrain>().terrainData.heightmapHeight;
        heightmap_ = new float[width, height];

        first_ = true;

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.C))
            SetHeightMap();
    }
    

    void SetHeightMap()
    {
        for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
            {
                heightmap_[i, j] = HybridMultifractal(new Vector2(i, j), H_, lacunarity_, octaves_, offset_);
            }


        first_ = true;
        gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, heightmap_);
    }

    float Hetero_Terrain(Vector2 point, float H, float lacunarity, float octaves, float offset)
    {
        int i = 0;


        float value = 0;
        float increment = 0;
        float frequency = 0;


        if (first_)
        {
            exponent_array_ = new float[(int)octaves + 1];
            frequency = 1.0f;
            for (i = 0; i <= octaves; ++i)
            {
                exponent_array_[i] = Mathf.Pow(frequency, -H);
                frequency *= lacunarity; 
            }
            first_ = false;
        }

        value = offset + (Mathf.PerlinNoise(point.x / width + seed_, point.y / height + seed_) * 2 - 1) / divider_;
        point *= lacunarity;

        for (i = 0; i <= octaves; ++i)
        {
            increment = offset + (Mathf.PerlinNoise(point.x / width + seed_, point.y / height + seed_) * 2 - 1) / divider_;
            increment *= exponent_array_[i];
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


        if (first_)
        {
            exponent_array_ = new float[(int)octaves + 1];
            frequency = 1.0f;
            for (i = 0; i <= octaves; ++i)
            {
                exponent_array_[i] = Mathf.Pow(frequency, -H);
                frequency *= lacunarity;
            }
            first_ = false;
        }

        result = offset + (Mathf.PerlinNoise(point.x / width + seed_, point.y / height + seed_) * 2 - 1) * exponent_array_[0] / divider_;
        weight = result;

        for (i = 0; i < octaves; ++i)
        {
            if (weight > 1.0f)
            {
                weight = 1.0f;
            }
            signal = offset + (Mathf.PerlinNoise(point.x / width + seed_, point.y / height + seed_) * 2 - 1) / divider_;
            signal *= exponent_array_[i];

            result += weight * signal;

            weight *= signal;

            point *= lacunarity;
        }

        return result;
    }
}
