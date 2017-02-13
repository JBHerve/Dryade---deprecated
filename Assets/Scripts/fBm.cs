using UnityEngine;

public class fBm : MonoBehaviour
{
    public float H_;
    public float lacunarity_; //The frequencies of the fractal. High lacunarity create compact relief
    public float octaves_; //Number of repetition of the fractal. Low number of octaves create soft relief
    public float[,] heightmap_;
    public float divider_; //Divide all the point of the heightmap, lowering global height
    public float baseGround_; //Set the height origin of the map. A high baseGround generate higher variation 

    public float seed_; //Genrate some randomness

    //Dimension of the heigthmap
    private int width_;
    private int height_;

    // Use this for initialization
    void Start()
    {
        width_ = gameObject.GetComponent<Terrain>().terrainData.heightmapWidth;
        height_ = gameObject.GetComponent<Terrain>().terrainData.heightmapHeight;
        heightmap_ = new float[width_, height_];
    }

    // Update is called once per frame
    void Update()
    {
        if ( Input.anyKeyDown )
        {
            SetHeightMap();
        }
    }

    void SetHeightMap()
    {
        for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
            for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
                heightmap_[i, j] = FBM(new Vector2(i, j), H_, lacunarity_, octaves_);

        gameObject.GetComponent<Terrain>().terrainData.SetHeights(0, 0, heightmap_);
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
        float weight = (Mathf.PerlinNoise(point.x / width_ + seed_, point.y / height_ + seed_) * 2 - 1 + baseGround_) * Mathf.Pow(lacunarity, -H * 0) / divider_;
        float signal = 0;
        int i = 0;

        for (; i < octaves; i++)
        {
            if (weight > 1.0f)
            {
                weight = 1.0f;
            }
            signal = (Mathf.PerlinNoise(point.x / width_ + seed_, point.y / height_ + seed_) * 2 - 1 + baseGround_) * Mathf.Pow(lacunarity, -H * i) / divider_;
            value += signal * weight;
            weight *= signal;
            point *= lacunarity;
        }

        remainder = octaves - (int)octaves;
        if (remainder != 0)
        {
            value += remainder * Mathf.PerlinNoise(point.x / width_, point.y / height_) * Mathf.Pow(lacunarity, -H * i);
        }
        return value;
    }
    
}
