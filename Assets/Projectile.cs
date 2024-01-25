using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {
    public enum ProjectileType { Projectile, Beam, Cone, ConeBeam }
    public ProjectileType projectileType;
    public bool isExplosive;
    public float explosionForce;
    public float explosionRadius;

    public bool DestroyOnContact; //Does the Projectile get destroyed when it hits something?
    public float ProjectileLifeTime; // How long a projectile will exist before it is destroyed.



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.eulerAngles = transform.GetComponent<Rigidbody>().velocity;
        if (projectileType == ProjectileType.Beam)
        {
            //Debug.Log("Beam Destroyed");
            Destroy(this.gameObject);
        }
        if (projectileType == ProjectileType.Cone)
        {
            //Debug.Log("Cone Destroyed");
            Destroy(this.gameObject);
        }
        if (projectileType == ProjectileType.ConeBeam)
        {
            //Debug.Log("Cone Destroyed");
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter(Collider collider) //If power isTrigger
    {
        if (!collider.GetComponent<Projectile>())
        {
            if (isExplosive)
            {
                Debug.Log("Exploding");
                Explode();
            }
            if (DestroyOnContact)
            {
                Destroy(this.gameObject);
            }
            //TODO Apply Force at collision point, in direction of projectile movement.
        }
    }

    private void OnCollisionEnter(Collision collider) //If power !isTrigger
    {
        if (!collider.collider.GetComponent<Projectile>())
        {
            if (isExplosive)
            {
                //Debug.Log("Exploding");
                Explode();
            }
            if (DestroyOnContact)
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void Explode()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 3.0F);
        }
    }
}
