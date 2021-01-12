using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class IntroLogic : MonoBehaviour
{
    [SerializeField] private RenderTexture textureToRenderLoginCamera;
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private Camera introCamera;
    [SerializeField] private GameObject[] objectToRemoveAfterAnimationEnd;

    private Camera _loginSceneCamera;

    private async void Start()
    {
        await SceneManager.LoadSceneAsync("Login", LoadSceneMode.Additive);
        _loginSceneCamera = GameObject.FindObjectsOfType<Camera>().First(camera => camera != introCamera);

        _loginSceneCamera.targetTexture = textureToRenderLoginCamera;
        StartCoroutine(DetectAnimationEnd());
    }

    private IEnumerator DetectAnimationEnd()
    {
        yield return new WaitUntil( () => canvasAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        OnAnimationEnd();
    }

    private void OnAnimationEnd()
    {
        _loginSceneCamera.targetTexture = null;

        foreach (var objectToDestroy in objectToRemoveAfterAnimationEnd)
            Destroy(objectToDestroy);
    }

    private void LateUpdate()
    {
        if (_loginSceneCamera == null) 
            return;
        
        var loginSceneCameraTransform = _loginSceneCamera.transform;
        introCamera.transform.SetPositionAndRotation(loginSceneCameraTransform.position, loginSceneCameraTransform.rotation);
    }
}
