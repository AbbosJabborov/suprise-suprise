using Core.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Core
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private CanvasGroup menuCanvasGroup;

        private ISceneLoaderService sceneLoader;

        [Inject]
        public void Construct(ISceneLoaderService sceneLoaderService)
        {
            sceneLoader = sceneLoaderService;
        }

        void Start()
        {
            playButton.onClick.AddListener(OnPlayClicked);
            quitButton.onClick.AddListener(OnQuitClicked);
        }

        void OnPlayClicked()
        {
            sceneLoader.LoadSceneWithTransition("Delivery");
            sceneLoader.UnloadScene("MainMenu");
        }

        void OnQuitClicked()
        {
            Application.Quit();
        }

        void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlayClicked);
            quitButton.onClick.RemoveListener(OnQuitClicked);
        }
    }
}
