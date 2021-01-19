using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        StartCoroutine(LoadSceneAfter3Seconds(sceneName));
    }

    public void LoadSceneImmediately(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    
    private IEnumerator LoadSceneAfter3Seconds(string sceneName)
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
