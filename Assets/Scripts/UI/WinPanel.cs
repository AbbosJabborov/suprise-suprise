using UnityEngine;

namespace UI
{
    public class WinPanel : MonoBehaviour
    {
        [SerializeField] private GameObject winPanel;

        void Start()
        {
            winPanel.SetActive(false);
        }
        public void OnWin()
        {
            Time.timeScale = 0;
            winPanel.SetActive(true);
            
        }
    }
}
