using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Buttons")]
    public Button playButton;
    public Button quitButton;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Assign button click listeners
        if (playButton != null)
            playButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);

        StartGettingApiUpdates();
    }

    private void StartGettingApiUpdates()
    {
        ParkingApiRequestManager.Instance.StartGettingUpdates();
        Debug.Log("Started getting API updates");
    }

    public void StartGame()
    {
        Debug.Log("Started the game");
    }

    // Quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();

        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
