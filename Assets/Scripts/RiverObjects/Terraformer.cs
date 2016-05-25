using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.RiverObjects
{
    class Terraformer
    {
        private bool first;
        int width;
        int height;
        int seed;
        private float[] exponent_array_;

        public Terraformer(int seed)
        {
            first = true;
            this.seed = seed;
        }
        public List<Vector2> SetHeightMap(Terrain terrain, float offset, float H, float lacunarity, int octaves)
        {
            width = terrain.terrainData.heightmapWidth;
            height = terrain.terrainData.heightmapHeight;
            float[,] heightmap = new float[width, height];
            List<Vector2> list = new List<Vector2>();
            bool isTop = false;
            for (int i = 0; i < Terrain.activeTerrain.terrainData.heightmapWidth; i++)
                for (int j = 0; j < Terrain.activeTerrain.terrainData.heightmapHeight; j++)
                {
                    float value = HybridMultifractal(new Vector2(i, j), H, lacunarity, octaves, offset);
                    //The user give an offset, which work as a 'sea level'
                    //Max value is this offset
                    //When we detect a change of level, we stock this position in order to create a 'coast'
                    if (value >= offset)
                    {
                        heightmap[i, j] = offset;
                        if (!isTop)
                        {
                            list.Add(new Vector2(i, j));
                            isTop = true;
                        }
                    }
                    else
                    {
                        heightmap[i, j] = value;
                        if (isTop)
                        {
                            list.Add(new Vector2(i, j));
                            isTop = false;
                        }
                    }
                }
            first = true;
            terrain.terrainData.SetHeights(0, 0, heightmap);
            return list;
        }

        private float HybridMultifractal(Vector2 point, float H, float lacunarity, int octaves, float offset)
        {
            int i = 0;


            float result = 0;
            float signal = 0;
            float frequency = 0;
            float weight = 0;


            if (first)
            {
                exponent_array_ = new float[(int)octaves + 1];
                frequency = 1.0f;
                for (i = 0; i <= octaves; ++i)
                {
                    exponent_array_[i] = Mathf.Pow(frequency, -H);
                    frequency *= lacunarity;
                }
                first = false;
            }

            result = offset + (Mathf.PerlinNoise(point.x / width + seed, point.y / height + seed) * 2 - 1) * exponent_array_[0];
            weight = result;

            for (i = 0; i < octaves; ++i)
            {
                if (weight > 1.0f)
                {
                    weight = 1.0f;
                }
                signal = offset + (Mathf.PerlinNoise(point.x / width + seed, point.y / height + seed) * 2 - 1);
                signal *= exponent_array_[i];

                result += weight * signal;

                weight *= signal;

                point *= lacunarity;
            }

            return result;
        }
    }
}
