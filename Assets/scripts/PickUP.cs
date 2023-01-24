using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickUP : MonoBehaviour
{
    float throwForce = 600;
    Vector3 ObjPos;
    float distance;

    public bool canHold = true;
    public GameObject item;
    public GameObject tempParent;
    public bool isHolding = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(item.transform.position, tempParent.transform.position);
        if(distance > 1f)
        {
            isHolding= false;
        }
        if (isHolding == true)
        {
            item.GetComponent<Rigidbody>().velocity = Vector3.zero;
            item.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            item.transform.SetParent(tempParent.transform);

            if (Input.GetMouseButtonDown(1))
            {

            }
            else
            {
                ObjPos = item.transform.position;
                item.GetComponent<Rigidbody>().useGravity = true;
                item.transform.position = ObjPos;
            }
        }
    }

    private void OnMouseDown()
    {
        if (distance <= 1f)
        {
            isHolding = true;
            item.GetComponent<Rigidbody>().useGravity = false;
            item.GetComponent<Rigidbody>().detectCollisions = true;

            if (Input.GetMouseButtonDown(1))
            {
                item.GetOrAddComponent<Rigidbody>().AddForce(tempParent.transform.forward * throwForce);
                isHolding= false;
            }
        }

    }

    private void OnMouseUp()
    {
        isHolding= false;
    }
}
