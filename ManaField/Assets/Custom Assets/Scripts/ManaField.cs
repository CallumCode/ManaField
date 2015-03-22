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
                maps[x, y, 1] = mana;
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
    }

}
