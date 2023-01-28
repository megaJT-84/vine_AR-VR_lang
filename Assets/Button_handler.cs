using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    private Button _button;
    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Grip"))
        {
            _button.onClick.Invoke();
        }
    }
}
