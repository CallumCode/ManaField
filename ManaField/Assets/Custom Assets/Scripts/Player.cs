using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour 
{

    public GameObject Terrain;
    private ManaField manaField;

    public float drainRate = 1;
    public int drainSize = 5;

    public GameObject spell1Object;

    public GameObject spellLaunch;

    public float aimTurnRate = 10;

    float currentMana = 0;
 	// Use this for initialization
	void Start ()
    {
        manaField = (ManaField)Terrain.GetComponent<ManaField>();
	}
	
    bool fire = false;
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            spellLaunch.transform.forward = transform.forward;
            currentMana = 0;
        }


        if(Input.GetKey(KeyCode.Alpha1))
        {
            float mouseX = Input.GetAxis("Mouse X") * aimTurnRate;
            spellLaunch.transform.Rotate(Vector3.up, mouseX * Time.deltaTime);
            
            
           currentMana += manaField.DrainMana(drainRate * Time.deltaTime, transform.position, drainSize, drainSize);
            fire = true;
        }

          if(Input.GetKeyUp(KeyCode.Alpha1) &&  (fire == true))
       {
           Instantiate(spell1Object, spellLaunch.transform.position + spell1Object.transform.position, spellLaunch.transform.rotation);
           fire = false;
       }
	}
}
