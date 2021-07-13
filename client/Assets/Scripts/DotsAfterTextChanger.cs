using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DotsAfterTextChanger : MonoBehaviour
{
    private string _startTextValue;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _startTextValue = _text.text;
    }

    private void OnEnable()
    {
        StartCoroutine(ChangeText());
    }

    private IEnumerator ChangeText()
    {
        while (enabled)
        {
            _text.text = _startTextValue;
            for (int i = 0; i <= 3; i++)
            {
                yield return new WaitForSecondsRealtime(0.7f);
                _text.text += ".";
            }
        }
    }
}
