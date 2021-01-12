using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSmoothMouseFollower : MonoBehaviour
{
    [SerializeField] private Vector2 minMaxRotX;
    [SerializeField] private Vector2 minMaxRotZ;
    [SerializeField] private float t = 1f;

    private Vector3 startEulerRot;

    private void Start()
    {
        startEulerRot = transform.localRotation.eulerAngles;
    }

    private void Update()
    {
        var xMousePercent = Input.mousePosition.x / Screen.width;
        var yMousePercent = Input.mousePosition.y / Screen.height;

        var yy = Mathf.Lerp(startEulerRot.y + minMaxRotX.x, startEulerRot.y + minMaxRotX.y, xMousePercent);
        var xx = Mathf.Lerp(startEulerRot.x + minMaxRotZ.x, startEulerRot.x + minMaxRotZ.y, 1 - yMousePercent);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(xx, yy, startEulerRot.z), Time.deltaTime * t);
    }
}
