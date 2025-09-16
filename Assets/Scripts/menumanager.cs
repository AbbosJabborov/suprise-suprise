using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject sceneLoader;
    
    public void OnPlay()
    {
        sceneLoader.GetComponent<SceneLoader>().LoadScene(1);
    }
}
