using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectReset : MonoBehaviour
{
    private bool isPlaced;
    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    // Start is called before the first frame update

    private void Start()
    {
        var trans = transform;
        originalParent = trans.parent;
        originalPosition = trans.localPosition;
        originalRotation = trans.localRotation;
    }
    public void Reset()
    {
        isPlaced = true;

        // Reset parent and placement of object
        var trans = transform;
        trans.SetParent(originalParent);
        trans.localPosition = originalPosition;
        trans.localRotation = originalRotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
