using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipFlag : MonoBehaviour
{
    private bool hasAttch = false;
    // Start is called before the first frame update
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasAttachment()
    {
        return hasAttch;
    }

    public void Attach(bool attachmentState)
    {
        hasAttch = attachmentState;
    }
}
