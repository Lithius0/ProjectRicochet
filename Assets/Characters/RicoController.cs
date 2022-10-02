using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RicoController : MonoBehaviour
{
    [SerializeField] GameObject arrowPrefab;
    public GameObject activeArrow;

    [SerializeField] GameObject powerTextDisplay;

    FollowMouse followMouseComponent;
    private bool aiming = false;
    Vector3 targetPosition;

    [SerializeField] const float maxPower = 30f;
    [SerializeField] const float minPower = 10f;
    private float power = 0;

    [SerializeField] float debounceTime = 0.1f;
    private float lastFiredTime = -1000f;

    // Start is called before the first frame update
    void Start()
    {
        followMouseComponent = GetComponent<FollowMouse>();
    }

    void OnLaunchArrow(InputValue value)
    {
        if (value.isPressed && (Time.time - lastFiredTime) > debounceTime)
        {
            GameObject newObject = Instantiate(arrowPrefab, gameObject.transform.position, Quaternion.identity);
            Arrow newArrow = newObject.GetComponent<Arrow>();
            activeArrow = newObject;

            this.aiming = true;
            this.targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Need to get rid of Z
            this.targetPosition = new Vector3(this.targetPosition.x, this.targetPosition.y, 0); 
            this.lastFiredTime = Time.time;

            this.powerTextDisplay.GetComponent<TextMeshProUGUI>().enabled = true; ;
            newArrow.Point(targetPosition - gameObject.transform.position);
        }
        else if(aiming)
        {
            if (power >= minPower)
            {
                //Launch
                this.Launch(power, power);
            } 
            else
            {
                Destroy(activeArrow);
            }

            this.aiming = false;
            this.powerTextDisplay.GetComponent<TextMeshProUGUI>().enabled = false;
        }
    }

    void Launch(float speed, float penetration = 21)
    {
        //This is to prevent the user from clipping the arrow backwards through the wall.
        activeArrow.transform.position = gameObject.transform.position;

        Arrow arrowComponent = activeArrow.GetComponent<Arrow>();

        arrowComponent.penetration = penetration;

        arrowComponent.Launch((targetPosition - gameObject.transform.position).normalized * speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (this.aiming)
        {
            const float powerScale = 5f;

            float scaledMinPower = minPower / powerScale;
            float scaledMaxPower = maxPower / powerScale;

            Vector3 targetDirection = (targetPosition - gameObject.transform.position).normalized;
            Vector3 pullDirection = -(targetDirection).normalized;
            //Direction of the mouse from rico's reference frame
            Vector3 mouseDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position;

            Vector3 projectedDirection = Vector3.Project(mouseDirection, pullDirection);

            projectedDirection = Vector3.ClampMagnitude(projectedDirection, scaledMaxPower);

            //The projected direction should be opposite the targetDirection. That means their sum should be smaller than .
            bool wrongDirection = (projectedDirection + targetDirection).magnitude > projectedDirection.magnitude;
            if (projectedDirection.magnitude < scaledMinPower || wrongDirection)
            {
                projectedDirection = pullDirection * scaledMinPower;
            }

            Vector3 arrowPosition = gameObject.transform.position + projectedDirection;

            activeArrow.transform.position = arrowPosition;

            power = projectedDirection.magnitude * powerScale;
            TextMeshProUGUI textDisplay = powerTextDisplay.GetComponent<TextMeshProUGUI>();
            textDisplay.text = string.Format("{0:0.00}", power);
            RectTransform transform = textDisplay.GetComponent<RectTransform>();
            transform.position = arrowPosition;

            Debug.DrawRay(gameObject.transform.position, projectedDirection, Color.red);
        }

        followMouseComponent.enabled = !aiming && (Time.time - lastFiredTime) > debounceTime;

    }
}
