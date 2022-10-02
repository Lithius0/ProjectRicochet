using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThicknessChecker : MonoBehaviour
{
    [SerializeField] private LineRenderer scanLine;
    [SerializeField] private LineRenderer reflectionLine;
    [SerializeField] private float penetration = 15;
    [SerializeField] private GameObject textDisplay;

    private float maxPen = 50;
    private float minPen = 5;

    private Vector3 startPosition;
    private bool active = false;
    // Start is called before the first frame update
    void Start()
    {
        textDisplay.GetComponent<TextMeshProUGUI>().enabled = false;
        scanLine.enabled = false;
        reflectionLine.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            penetration = Mathf.Clamp(penetration + Input.mouseScrollDelta.y, this.minPen, this.maxPen);

            // World position of the mouse
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition = new Vector3(mousePosition.x, mousePosition.y, 0);

            Vector3 direction = mousePosition - startPosition;
            Vector3 scanEndPosition = mousePosition;
            Vector3 reflectionEndPosition = mousePosition;

            RaycastHit hit;
            float effectiveThickness = 0;

            if (Physics.Raycast(startPosition, direction, out hit, direction.magnitude))
            {
                Vector3 hitPosition = hit.point;
                
                //hit.collider should always return true since raycasts scans for colliders.
                Armor armorHit = hit.collider.gameObject.GetComponent<Armor>();

                if (armorHit != null)
                {
                    effectiveThickness = armorHit.GetThickness(hitPosition, direction);
                    if (effectiveThickness > this.penetration)
                    {
                        //https://math.stackexchange.com/questions/13261/how-to-get-a-reflection-vector
                        Vector3 reflectionDirection = direction - (2 * Vector3.Dot(direction, hit.normal) * hit.normal);
                        this.reflectionLine.enabled = true;

                        float totalLength = (mousePosition - startPosition).magnitude;

                        reflectionEndPosition = hitPosition + (reflectionDirection.normalized * (totalLength - hit.distance));
                        scanEndPosition = hitPosition;
                    }

                }


            }

            SetLinePositions(startPosition, scanEndPosition, reflectionEndPosition);
            TextMeshProUGUI textComponent = textDisplay.GetComponent<TextMeshProUGUI>();
            RectTransform transform = textDisplay.GetComponent<RectTransform>();

            textComponent.text = string.Format("{0:0.00} : {1:0.00}", penetration, effectiveThickness);
            transform.position = scanEndPosition + new Vector3(0.2f, -0.2f, 0);
        }
    }

    void SetLinePositions(Vector3 start, Vector3 end)
    {
        SetLinePositions(start, end, end);
    }

    void SetLinePositions(Vector3 start, Vector3 middle, Vector3 end)
    {
        float startWidth = this.scanLine.startWidth;
        float scanLength = (middle - start).magnitude;
        float reflectionLength = (end - middle).magnitude;
        float totalLength = scanLength + reflectionLength;
        float breakWidth = (reflectionLength / totalLength) * startWidth;

        scanLine.SetPosition(0, start);
        scanLine.SetPosition(1, middle);
        reflectionLine.SetPosition(0, middle);
        reflectionLine.SetPosition(1, end);
        
        reflectionLine.startWidth = breakWidth;
        reflectionLine.endWidth = 0;
        scanLine.endWidth = breakWidth;
    }

    void OnActivateHitSimulator(InputValue value)
    {
        if (value.isPressed)
        {
            startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPosition = new Vector3(startPosition.x, startPosition.y, 0); //Eliminating the 3D component
            active = true;

            textDisplay.GetComponent<TextMeshProUGUI>().enabled = true;
            scanLine.enabled = true;
            reflectionLine.enabled = true;


            //The line renderer positions are relative to the game object it's tied to by default.
            scanLine.useWorldSpace = true;
            reflectionLine.useWorldSpace = true;

            SetLinePositions(startPosition, startPosition);
        } 
        else
        {
            active = false;

            textDisplay.GetComponent<TextMeshProUGUI>().enabled = false;
            scanLine.enabled = false;
            reflectionLine.enabled = false;
        }

    }
}
