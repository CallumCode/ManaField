using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Linq;

public class ManaField : MonoBehaviour 
{

    Terrain terrain; 
    private TerrainData terrainMana;
    public TerrainData terrainLevel;

    public float regainRate = 1;

	// Use this for initialization
	void Start ()
    {      
        Init();
	}

    

	// Update is called once per frame
	void Update () 
    {
        Regen(terrain, regainRate * Time.deltaTime);
  	}

 

    void Regen(Terrain t, float rate)
    {
        float[,,] maps = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        float[, ,] mapsLevel = terrainLevel.GetAlphamaps(0, 0, terrainLevel.alphamapWidth, terrainLevel.alphamapHeight);


        for (var y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (var x = 0; x < t.terrainData.alphamapWidth; x++)
            {

                float mana = 1-  maps[x, y, 0];
                mana = mana + rate;
                mana = Mathf.Clamp01(mana);
                maps[x, y, 0] = (1 - mana);

                for (int tex = 1; tex < t.terrainData.alphamapLayers; tex++)
                {
                    maps[x, y, tex] = mana * mapsLevel[x, y, tex];
                }
                
             }
        }

        t.terrainData.SetAlphamaps(0, 0, maps);
    }

    void Init()
    {
        terrain = GetComponent<Terrain>();
        terrainMana = new TerrainData() ;

        CopyFrom(terrainMana, terrain.terrainData);  // copy the basic getable/setable properties
        Copy(terrain.terrainData, terrainMana);


        terrain.terrainData = terrainMana;
        
    }


    // so copys genric class... unsure on how it works
    public static void CopyFrom<T1, T2>(T1 obj, T2 otherObject)
        where T1 : class
        where T2 : class
    {
        PropertyInfo[] srcFields = otherObject.GetType().GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

        PropertyInfo[] destFields = obj.GetType().GetProperties(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

        foreach (var property in srcFields)
        {
            var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
            if (dest != null && dest.CanWrite)
                dest.SetValue(obj, property.GetValue(otherObject, null), null);
        }
    }



    void Copy(TerrainData level, TerrainData mana)
    {
        float[, ,] maps = level.GetAlphamaps(0, 0, level.alphamapWidth, level.alphamapHeight);
        mana.SetAlphamaps(0, 0, maps);

        mana.SetHeights(0, 0, level.GetHeights(0, 0, level.heightmapHeight, level.heightmapWidth));    
    }


    public float DrainMana(float drainStrength, Vector3 pos, int width, int height)
    {
        float mana = 0;

        float y = pos.z; // callng it y as the map is 2D
        float x = pos.x;

        y = y / (terrain.terrainData.size.z ) ;
        x = x / (terrain.terrainData.size.x );

        y = Mathf.Clamp01(y);
        x = Mathf.Clamp01(x);

        int indexY = Mathf.RoundToInt( y * (terrain.terrainData.alphamapHeight-1));
        int indexX = Mathf.RoundToInt(x * (terrain.terrainData.alphamapWidth-1));

        float[, ,] map = terrain.terrainData.GetAlphamaps(indexX, indexY , width, height);

        for (int w = 0; w < width; w++ )
        {
            for (int h = 0; h < height; h++)
            {
                map[w, h, 0] = 1;
                for (int a = 1; a < terrain.terrainData.alphamapLayers; a++)
                {
                    map[w, h, a] = 0;
                }
            }
        }
       


        terrain.terrainData.SetAlphamaps(indexX, indexY, map);


        return mana;
    }

}
