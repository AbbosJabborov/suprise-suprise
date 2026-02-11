using Core;
using Core.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace UI.Menu
{
    public class MenuManager : MonoBehaviour
    {
        [Inject] private ISceneLoaderService sceneLoader;
        [SerializeField] private Button playButton;

        private void Start()
        {
            playButton.onClick.AddListener(LoadTown);
        }
        
        async void LoadTown()
        {
            await sceneLoader.LoadSceneWithTransition("Town", transitionDuration: 0.5f);
        }

    }
}
