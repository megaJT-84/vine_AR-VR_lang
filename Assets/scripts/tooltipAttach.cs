using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public GameObject gameObject;
    GameObject ob;
    // Start is called before the first frame update
    void Start()
    {
        ob.transform.SetParent(gameObject.transform, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
