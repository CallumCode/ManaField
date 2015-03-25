using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{
    
    public float pushStrength = 1;

    public float startStrength = 1;


    Rigidbody rigidbody;
    // Use this for initialization
	void Start () 
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.forward * startStrength);

    }
	
	// Update is called once per frame
	void Update ()
    {


    }

    void OnTriggerStay(Collider other)
    {
            if (other.attachedRigidbody)
        { 
            other.attachedRigidbody.AddForce(transform.forward * pushStrength * Time.deltaTime);
        }
    }
}
