using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputFieldKeyEventExecutor : MonoBehaviour
{
    public KeyCode keyCode;
    public UnityEvent onKeyDown;
    
    private void Update()
    {
        if(EventSystem.current.currentSelectedGameObject != this.gameObject)
            return;

        if(Input.GetKeyDown(keyCode))
            onKeyDown.Invoke();
    }
}
