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
    public float difuseRate = 1;
    public float manaBoundary = 1;

	// Use this for initialization
	void Start ()
    {      
        Init();
	}

    

	// Update is called once per frame
	void Update () 
    {

      
            UpdateFlow(terrain, regainRate * Time.deltaTime);

  	}

 
    float  Difuse(float[,,] mapOld , int x , int y  , float startMana)
    {

        float mana = Mathf.Clamp01(1 - startMana);

        float a = Time.deltaTime * difuseRate * terrainLevel.alphamapHeight * terrainLevel.alphamapWidth; // PASSS DOWN FOR OPI 
        mana = (mana + a * (GetNeig(mapOld, x - 1, y) + GetNeig(mapOld, x + 1, y) + GetNeig(mapOld, x, y - 1) + GetNeig(mapOld, x, y + 1))) / (1 + 4 * a);


        return Mathf.Clamp01(1 - mana);
    }

    float GetNeig(float[,,] mapOld , int x, int y)
    {
        if((x < 0 ) || (x >= terrainLevel.alphamapWidth) )
        {
            return manaBoundary;
        }

        if ((y < 0) || (y >= terrainLevel.alphamapHeight))
        {
            return manaBoundary;
        }


        return (1 - mapOld[x, y, 0]);
    }

    float Regen( float regenRate , float current )
    {
        float mana = 1 - current;
        mana = mana + regenRate;
        mana = Mathf.Clamp01(mana);
        return 1- mana;
    }
    

    void UpdateFlow(Terrain t, float regenRate)
    {
        float[,,] mapsNew = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);
        float[, ,] mapsOld = t.terrainData.GetAlphamaps(0, 0, t.terrainData.alphamapWidth, t.terrainData.alphamapHeight);

        float[, ,] mapsLevel = terrainLevel.GetAlphamaps(0, 0, terrainLevel.alphamapWidth, terrainLevel.alphamapHeight);


        for (int y = 0; y < t.terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < t.terrainData.alphamapWidth; x++)
            {


             //   mapsNew[x, y, 0] = Regen(regenRate, mapsOld[x, y, 0]);
                 
                    mapsNew[x, y, 0] = Difuse(mapsOld, x, y, mapsNew[x, y, 0]);
                 


                // this sorts out the rest of the textures based on the mana level
                for (int tex = 1; tex < t.terrainData.alphamapLayers; tex++)
                {
                    mapsNew[x, y, tex] = (  1- mapsNew[x, y, 0]    ) * mapsLevel[x, y, tex];
                }
                
             }
        }

        t.terrainData.SetAlphamaps(0, 0, mapsNew);
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
        float manaDrained = 0;

        float y = pos.z; // callng it y as the map is 2D
        float x = pos.x;

        y = y / (terrain.terrainData.size.z ) ;
        x = x / (terrain.terrainData.size.x );

        y = Mathf.Clamp01(y);
        x = Mathf.Clamp01(x);

        int indexY = Mathf.RoundToInt( y * (terrain.terrainData.alphamapHeight-1));
        int indexX = Mathf.RoundToInt(x * (terrain.terrainData.alphamapWidth-1));

        float[, ,] map = terrain.terrainData.GetAlphamaps(indexX, indexY     , width, height);
         float[, ,] mapLevel = terrainLevel.GetAlphamaps(indexX, indexY , width, height);

        for (int w = 0; w < width; w++ )
        {
            for (int h = 0; h < height; h++)
            {
                float manaAtPoint = 1 - map[w, h, 0];

                float newManaPoint = Mathf.Clamp01(manaAtPoint - drainStrength );

                map[w, h, 0] = Mathf.Clamp01( 1- newManaPoint );

                manaDrained += Mathf.Clamp01(manaAtPoint - newManaPoint);

                for (int a = 1; a < terrain.terrainData.alphamapLayers; a++)
                {
                    map[w, h, a] = newManaPoint  * mapLevel[w, h, a];
                }
            }
        }
       


        terrain.terrainData.SetAlphamaps(indexX, indexY, map);


        return manaDrained;
    }

}
