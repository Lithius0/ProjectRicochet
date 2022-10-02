using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChetSceneManager : MonoBehaviour
{
    [SerializeField] string nextScene;

    public void OnArrowHit()
    {
        Debug.Log("Win");
        //SceneManager.LoadScene(nextScene);
    }
}
