using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChetSceneManager : MonoBehaviour
{
    [SerializeField] string nextScene;

    public void OnArrowHit()
    {
        //Debug.Log("Win");

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if ((sceneIndex + 1) < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex + 1);
        }
        else
        { 
            SceneManager.LoadScene(0);
            Debug.Log(sceneIndex);
            Debug.Log("Win");
        }
    }
}
