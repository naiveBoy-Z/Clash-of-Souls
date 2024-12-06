using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    public void LoadScenesByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScenesByIndex(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
