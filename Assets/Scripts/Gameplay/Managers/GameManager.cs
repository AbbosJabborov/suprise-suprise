using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Gameplay.Managers
{
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }
    
        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Playing;
    
        [Header("Player Stats")]
        [SerializeField] private int coins;
        [SerializeField] private int kills;
        [SerializeField] private int playerLevel = 1;
        [SerializeField] private int currentXP = 0;
        [SerializeField] private int xpToNextLevel = 100;
        [SerializeField] private float xpMultiplier = 1.5f; // XP required increases each level
    
        [Header("References")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject caravan;
        [SerializeField] private CoinCountUI coinCountUI;
    
        [Header("Events")]
        public UnityEvent<int> OnCoinsChanged;
        public UnityEvent<int> OnKillCountChanged;
        public UnityEvent<int> OnLevelUp; // Passes new level
        public UnityEvent<int, int> OnXPChanged; // Passes (current, required)
        public UnityEvent OnGameOver;
        public UnityEvent OnVictory;
    
        public enum GameState
        {
            Menu,
            Playing,
            Paused,
            GameOver,
            Victory
        }
    
        void Awake()
        {
        
            // Initialize events
            OnCoinsChanged ??= new UnityEvent<int>();
            OnKillCountChanged ??= new UnityEvent<int>();
            OnLevelUp ??= new UnityEvent<int>();
            OnXPChanged ??= new UnityEvent<int, int>();
            OnGameOver ??= new UnityEvent();
            OnVictory ??= new UnityEvent();
        }
    
        void Start()
        {
            // Auto-find player and caravan if not set
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
            if (caravan == null)
                caravan = GameObject.FindGameObjectWithTag("Caravan");
        
            if (player != null)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                    playerHealth.OnDeath.AddListener(OnPlayerDeath);
            }
        
            if (caravan != null)
            {
                Health caravanHealth = caravan.GetComponent<Health>();
                if (caravanHealth != null)
                    caravanHealth.OnDeath.AddListener(OnCaravanDeath);
            }
            
            OnCoinsChanged.Invoke(coins);
            OnKillCountChanged.Invoke(kills);
            OnXPChanged.Invoke(currentXP, xpToNextLevel);
        }
    
        void Update()
        {
            // Pause handling
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    
        // Coin management
        public void AddCoins(int amount)
        {
            coins += amount;
            OnCoinsChanged.Invoke(coins);
        }
    
        public bool SpendCoins(int amount)  
        {
            if (coins >= amount)
            {
                coins -= amount;
                OnCoinsChanged.Invoke(coins);
                return true;
            }
            return false;
        }

        public int GetCoins()
        {
            return coins;
        }
    
        // Kill tracking
        public void RegisterKill()
        {
            kills++;
            OnKillCountChanged.Invoke(kills);
        
            // Award XP for kill (you can customize XP per enemy type)
            AddXP(10);
        }
    
        public int GetKills() => kills;
    
        // XP and leveling
        public void AddXP(int amount)
        {
            currentXP += amount;
            OnXPChanged.Invoke(currentXP, xpToNextLevel);
        
            // Check for level up
            while (currentXP >= xpToNextLevel)
            {
                LevelUp();
            }
        }
    
        void LevelUp()
        {
            currentXP -= xpToNextLevel;
            playerLevel++;
            xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * xpMultiplier);
        
            OnLevelUp.Invoke(playerLevel);
            OnXPChanged.Invoke(currentXP, xpToNextLevel);
        
            Debug.Log($"Level Up! Now level {playerLevel}");
        
            // Pause game for upgrade selection (optional)
            // ShowUpgradeScreen();
        }
    
        public int GetPlayerLevel() => playerLevel;
        public int GetCurrentXP() => currentXP;
        public int GetXPToNextLevel() => xpToNextLevel;
        public float GetXPPercent() => (float)currentXP / xpToNextLevel;
    
        // Game state management
        public void SetGameState(GameState newState)
        {
            currentState = newState;
        
            switch (newState)
            {
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    OnGameOver.Invoke();
                    break;
                case GameState.Victory:
                    Time.timeScale = 0f;
                    OnVictory.Invoke();
                    break;
            }
        }
    
        public GameState GetGameState() => currentState;
    
        public void TogglePause()
        {
            if (currentState == GameState.Playing)
            {
                SetGameState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                SetGameState(GameState.Playing);
            }
        }
    
        // Death handlers
        void OnPlayerDeath()
        {
            Debug.Log("Player died!");
            SetGameState(GameState.GameOver);
            RestartGame();
        }
    
        void OnCaravanDeath()
        {
            Debug.Log("Caravan destroyed!");
            SetGameState(GameState.GameOver);
            RestartGame();
        }
    
        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );
        }
    
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}