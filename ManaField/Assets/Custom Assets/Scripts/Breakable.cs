using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour
{

    float damageState = 1;

    Material material;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void ChangeDamage( float amount)
    {
        damageState -= amount;
        damageState = Mathf.Clamp01(damageState);
 
    }
}
