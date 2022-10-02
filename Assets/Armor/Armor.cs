using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{
    //The length at which nominalThickness is defined.
    //The physical thickness of the plate in unity.
    [SerializeField] public float nominalSize = 2; 
   
    [SerializeField] public float nominalThickness = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
     * Dynamic effective thickness system. 
     * Didn't bother to do the slightly more performant static thickness system.
     * This system is more flexible anyway.
     * 
     * Casts a raycast from the opposite side of the plate and measures the distance 
     * between the hitPosition and the raycast hit.
     * Should work with any convex hull. May have issues with non-convex hulls but whatevs
     */
    public float GetThickness(Vector3 hitPosition, Vector3 projectileVelocity)
    {
        float thickness = 0;
        
        Vector3 size = gameObject.transform.localScale;
        float largestDimension = Mathf.Max(size.x, size.y, size.z);

        // The diagonal of a cube is the side * sqrt(2). That's the length of the longest line in a given square
        // Change to be sqrt(3) to generalize to 3d
        float maxSize = largestDimension * 1.42f; //Sqrt 2 is 1.4142...

        Vector3 farSide = hitPosition + projectileVelocity.normalized * maxSize;

        Collider partCollider = gameObject.GetComponent<Collider>();
        RaycastHit rayHit;
        //if (Physics.Raycast(oppositePosition, -projectileVelocity.normalized, out rayHit, maxSize))
        if (partCollider != null && partCollider.Raycast(new Ray(farSide, hitPosition - farSide), out rayHit, maxSize))
        {
            thickness = ((rayHit.point - hitPosition).magnitude / nominalSize) * nominalThickness;

            Debug.DrawLine(farSide, rayHit.point, Color.red);
        }

        return thickness;
    }
}
