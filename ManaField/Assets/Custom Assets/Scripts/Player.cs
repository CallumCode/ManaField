using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

    public GameObject Terrain;
    private ManaField manaField;

    public float drainRate = 1;
    public int drainSize = 5;
	// Use this for initialization
	void Start ()
    {
        manaField = (ManaField)Terrain.GetComponent<ManaField>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	

        if(Input.GetKey(KeyCode.Alpha1))
        {
            manaField.DrainMana(drainRate * Time.deltaTime, transform.position, drainSize, drainSize);

        }
	}
}
