using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

    public GameObject Terrain;
    private ManaField manaField;

	// Use this for initialization
	void Start ()
    {
        manaField = (ManaField)Terrain.GetComponent<ManaField>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            manaField.DrainMana(1, transform.position, 2, 2);

        }
	}
}
