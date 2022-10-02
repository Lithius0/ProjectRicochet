using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{

    [SerializeField] public float penetration = 20;
    [SerializeField] public float expirationTime = 5;

    private Vector3 velocity;
    private bool inFlight = false;
    private float timeElapsed = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Launch(new Vector3(1f, 0, 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.inFlight)
        {
            Transform arrowTransform = gameObject.transform;

            Vector3 nextPosition = arrowTransform.position + (velocity * Time.fixedDeltaTime);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, velocity.normalized, out hit, velocity.magnitude * Time.fixedDeltaTime))
            {
                if (hit.collider.gameObject.GetComponent<ChetSceneManager>())
                {
                    hit.collider.gameObject.GetComponent<ChetSceneManager>().OnArrowHit();
                }

                //hit.collider should always return true since raycasts scans for colliders.
                Armor armorHit = hit.collider.gameObject.GetComponent<Armor>();

                if (armorHit != null)
                {
                    float effectiveThickness = armorHit.GetThickness(hit.point, velocity);
                    if (effectiveThickness > this.penetration)
                    {
                        //Reflection
                        nextPosition = hit.point;
                        //https://math.stackexchange.com/questions/13261/how-to-get-a-reflection-vector
                        this.velocity = this.velocity - (2 * Vector3.Dot(this.velocity, hit.normal) * hit.normal);
                    }

                }
                else
                {
                    //If the collider hit isn't an armor plate, just reflect
                    this.velocity = this.velocity - (2 * Vector3.Dot(this.velocity, hit.normal) * hit.normal);
                }

            }

            this.Point(velocity);

            arrowTransform.position = nextPosition;

            timeElapsed += Time.fixedDeltaTime;
            if (timeElapsed > expirationTime)
            {
                Destroy(gameObject);
            }
        }

    }

    public void Point(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    public void Launch(Vector3 velocity)
    {

        this.velocity = velocity;
        this.inFlight = true;
    }
}
