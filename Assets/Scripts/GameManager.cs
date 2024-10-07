using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int currentLevel;
    public int currentRequired;
    public int currentCollected;
    public BoidProvider provider;
    public EffectorProvider effectors;
    public TextMeshProUGUI collectedCount, requiredCount, levelCount;

    public Button nextLevel;
    public static GameManager Instance;

    public GameObject startUi, gameUi, gameoverUi;
    public AudioSource startAudio, gameAudio;

    private bool _gameover;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        nextLevel.onClick.AddListener(NextLevel);
    }

    public void StartGame()
    {
        _gameover = false;
        gameoverUi.SetActive(false);
        startUi.SetActive(false);
        gameUi.SetActive(true);
        startAudio.enabled = false;
        gameAudio.enabled = true;
        currentLevel = 1;
        currentRequired = currentLevel * 15;
        requiredCount.text = currentRequired.ToString();
        levelCount.text = currentLevel.ToString();
        collectedCount.text = currentCollected.ToString();
        effectors.Init(currentLevel + 5);
        foreach (var spawner in provider.spawners)
        {
            spawner.amount = currentLevel * 12;
        }
        provider.CreateBoids();
    }

    private void NextLevel()
    {
        provider.RemoveBoid();
        nextLevel.interactable = false;
        currentLevel++;
        currentCollected = 0;
        currentRequired = currentLevel * 15;
        collectedCount.text = currentCollected.ToString();
        requiredCount.text = currentRequired.ToString();
        levelCount.text = currentLevel.ToString();
        foreach (var spawner in provider.spawners)
        {
            spawner.amount = currentLevel * 12;
        }
        EffectorProvider.RemoveFood();
        effectors.Init(currentLevel + 5);
        provider.CreateBoids();
    }

    public void QuitGame()
    {
        startAudio.enabled = true;
        _gameover = false;
        gameAudio.enabled = false;
        gameoverUi.SetActive(false);
        startUi.SetActive(true);
        gameUi.SetActive(false);
        provider.RemoveBoid();
        currentCollected = 0;
        EffectorProvider.RemoveFood();
        if (Camera.main != null)
        {
            Camera.main.transform.position = new Vector3(0, 0, Camera.main.transform.position.z);
            Camera.main.orthographicSize = 50;
        }
        startUi.SetActive(true);
        requiredCount.text = 0.ToString();
        levelCount.text = 0.ToString();
        collectedCount.text = 0.ToString();
        nextLevel.interactable = false;
        effectors.Init(0);
    }

    private void GameOver()
    {
        gameoverUi.SetActive(true);
        gameUi.SetActive(false);
    }

    public void Restart()
    {
        QuitGame();
        StartGame();
    }
    
    public void Collect()
    {
        currentCollected++;
        collectedCount.text = currentCollected.ToString();
        if (currentCollected >= currentRequired)
        {
            nextLevel.interactable = true;
        }
    }

    public void FinishedLevel()
    {
        if (currentCollected < currentRequired && !_gameover)
        {
            _gameover = true;
            GameOver();
        }
    }
}
