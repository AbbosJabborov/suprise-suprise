using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalInteraction : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private PortalTrigger portalTrigger;
    public void EnterRoom()
    {
        sceneLoader.LoadScene(portalTrigger.GetSceneIndex());
    }
}
