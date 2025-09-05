using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger: MonoBehaviour
{
    [SerializeField] private PortalInteraction portalInteraction;
    [SerializeField] private int sceneIndex;
    
    public int GetSceneIndex()
    {
        return sceneIndex;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        portalInteraction = other.GetComponent<PortalInteraction>();
    }
}