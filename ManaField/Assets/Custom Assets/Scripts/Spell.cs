using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{
    
    public float maxPushStrength = 1;
    public float maxStartStrength = 1;

    public Vector3 maxScale = new Vector3(5, 5, 0.2f);
    public Vector3 minScale = new Vector3(0.2f, 0.2f, 0.1f);

    public float maxEmission = 10;
    public float minEmission = 2;


    public float strength;

    Rigidbody rigidbody;

    bool init = false;

    public  float manaCostScaler = 1;
    // Use this for initializationd
	void Start () 
    {
        Init();
      
    }

    void CheckInit()
    {
        if (init == false)
        {
            Init();
        }
    }

    void Init()
    {
                    
        init = true;
        rigidbody = GetComponent<Rigidbody>();
    }
 
	// Update is called once per frame
	void Update ()
    {


    }

    void OnTriggerStay(Collider other)
    {
            if (other.attachedRigidbody)
        {
            other.attachedRigidbody.AddForce(transform.forward * strength* maxPushStrength * Time.deltaTime);
        }
    }

   public void FeedManaToSpell(float mana)
    {

        strength = 1 - Mathf.Exp(-mana * manaCostScaler);
        strength = Mathf.Clamp01(strength);

    }

    public void Fire()
   {
       CheckInit();

       rigidbody.AddForce(transform.forward * maxStartStrength * strength);
       transform.localScale = Vector3.Lerp(minScale, maxScale, strength);

       GetComponentInChildren<ParticleSystem>().emissionRate = Mathf.Lerp(minEmission, maxEmission, strength);
     }
}
