using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SliceMap {

    private const int SideLength = 32;


    public static void SliceTerrain(GameObject terrain)
    {
        int childrenCount = terrain.transform.childCount;
        for(int i = childrenCount - 1 ; i >= 0; --i)
        {
            GameObject.DestroyImmediate(terrain.transform.GetChild(i).gameObject);
        }

        EditorUtility.DisplayProgressBar("Slicing Terrain", "", 0);

        Terrain terrainCmp = terrain.GetComponent<Terrain>();
        TerrainData terrainData = terrainCmp.terrainData;

        float width = terrainData.size.x;
        float height = terrainData.size.z;
        float tall = terrainData.size.y;

        SplatPrototype[] splats = terrainData.splatPrototypes;
        for (int x = 0, xi = 0; x < width; x += SideLength, ++xi)
        {
            for (int y = 0, yi = 0; y < height; y += SideLength, ++yi)
            {
                EditorUtility.DisplayProgressBar("Slicing Terrain", xi + "_" + yi, x / width + 1 / width * y / height);

                TerrainData td = new TerrainData();
                td.heightmapResolution = 32;
                td.alphamapResolution = 32;
                td.baseMapResolution = 32;
                td.size = new Vector3(SideLength, tall, SideLength);

                SplatPrototype[] newSplats = new SplatPrototype[splats.Length];
                for(int i = 0; i < splats.Length; ++i)
                {
                    newSplats[i] = new SplatPrototype();
                    newSplats[i].texture = splats[i].texture;
                    newSplats[i].normalMap = splats[i].normalMap;
                    newSplats[i].tileSize = splats[i].tileSize;

                    float offsetX = x % splats[i].tileSize.x + splats[i].tileOffset.x;
                    float offsetY = y % splats[i].tileSize.y + splats[i].tileOffset.y;

                    newSplats[i].tileOffset = new Vector2(offsetX, offsetY);
                }
                td.splatPrototypes = newSplats;

                float[,,] alphaMap = new float[SideLength, SideLength, SideLength];
                alphaMap = terrainData.GetAlphamaps(x, y, SideLength, SideLength);
                td.SetAlphamaps(0, 0, alphaMap);

                float[,] heightMap = terrainData.GetHeights(x, y, SideLength + 1, SideLength + 1);
                td.SetHeights(0, 0, heightMap);

                GameObject newTerrain = new GameObject(xi + "_" + yi, typeof(Terrain), typeof(TerrainCollider));
                newTerrain.GetComponent<Terrain>().terrainData = td;
                newTerrain.GetComponent<TerrainCollider>().terrainData = td;
                newTerrain.transform.parent = terrain.transform;
                newTerrain.transform.position = new Vector3(x, 0, y);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Terrain/Slice")]
    public static void Slice()
    {
        GameObject t = GameObject.Find("Terrain");
        SliceTerrain(t);
    }
}
