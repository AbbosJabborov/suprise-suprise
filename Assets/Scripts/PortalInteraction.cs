using UnityEngine;

public class PortalInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private PortalTrigger portalTrigger;
    
    public void Interact()
    {
        sceneLoader.LoadScene(portalTrigger.GetSceneIndex());
    }
}