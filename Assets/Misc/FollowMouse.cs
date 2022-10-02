using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{ 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Point gameObject towards mouse.
        Vector3 mousePos = Input.mousePosition;
        Vector3 parentPos = gameObject.transform.position;
        parentPos = Camera.main.WorldToScreenPoint(parentPos);

        mousePos -= parentPos;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg + 180;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
