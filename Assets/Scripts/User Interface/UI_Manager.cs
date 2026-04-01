using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance;
    [Header("Menu Objects")]
    [SerializeField] private GameObject InGameMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private bool AllowPause;

    [Header("In game Hud Elements")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("End Game Hud Elements")]
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI coinsAmountText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI scoreAmountText;
    [SerializeField] private GameObject powerupParent;
    public Image powerUpFillImage;

    [Header("Countdown UI")]
    [SerializeField] private TextMeshProUGUI countdownText;


    private int currentHearts;
    private int coinsCollected;
    private int distance;
    private int score;

    public bool isPlayerdead;


    bool paused = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        if (powerupParent != null)
        {
            powerupParent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape) && AllowPause && !paused))
        {
            PauseState(!paused);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            EndGame();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            StartPowerUp(5);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            UpdateCoins(1);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            distance = distance + Random.Range(0, 500);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            UpdateScore(Random.Range(0, 500));
        }
    }

    public void EndGame()
    {
        endScreen.SetActive(true);
        InGameMenu.SetActive(false);
        AllowPause = false;
         if (distanceText != null && coinsAmountText != null && scoreText != null)
         {
            distanceText.text = distance.ToString("0");
            coinsAmountText.text = coinsCollected.ToString("0");
            scoreAmountText.text = score.ToString("0");
        }
        else
        {
            Debug.LogError("Something havent been assigned properly in the UI Manager's end game ui elements");
        }
        
       
        Time.timeScale = 0f;
    }
    public void LoadScreenWithLoadingScreen(int index)
    {
        if (paused)
        {
            PauseState(false);
        }
        Time.timeScale = 1f;

        if (SceneLoader.Instance == null)
        {
            Instantiate(loadingScreen);
            SceneLoader.Instance.LoadScene(index);
        }
        else
        {
            SceneLoader.Instance.LoadScene(index);
        }

    }
  
    public void LoadScene(int index)
    {
        if (paused)
        {
            PauseState(false);
            //Time.timeScale = 1;
        }
        StartCoroutine(LoadSceneAsync(index));
        Time.timeScale = 1;

    }

    public void StartPowerUp(int time)
    {
        StartCoroutine(PowerUpRoutine(time));
    }
    private IEnumerator PowerUpRoutine(int time)
    {
        Debug.Log("Couroine Started");
        float timer = time;
        powerUpFillImage.fillAmount = 1f;
        powerupParent.gameObject.SetActive(true);

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            powerUpFillImage.fillAmount = timer / time;
            yield return null;
        }

        powerUpFillImage.fillAmount = 0f;
        powerupParent.gameObject.SetActive(false);

    }

    public void UpdateScore(int scoreAmount)
    {
        if(scoreText != null)
        {
            score = score + scoreAmount;
            scoreText.text = score.ToString();
        }
    }
    private System.Collections.IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = true;

        yield return null;
    }

    public void UpdateCoins(int coina)
    {
        if(coinText != null)
        {
            coinsCollected = coinsCollected + coina;
            coinText.text = coinsCollected.ToString();
        }
    }
    public void PauseState(bool state)
    {
        paused = !paused;

        if (paused)
        {
            Time.timeScale = 0f;
            if (InGameMenu != null)
            {
                InGameMenu.SetActive(false);
            }
        }
        else
        {
            if (InGameMenu != null)
            {
                InGameMenu.SetActive(true);
            }

            if (countdownText != null)
                StartCoroutine(CountdownBeforeResume());
            else
                Time.timeScale = 1f;
        }

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(paused);
        }
    }

    private IEnumerator CountdownBeforeResume()
    {
        Time.timeScale = 0f;
        int countdown = 3;

        countdownText.gameObject.SetActive(true);

        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSecondsRealtime(1f);
            countdown--;
        }

        countdownText.text = "Go";
        yield return new WaitForSecondsRealtime(0.5f);

        countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

}
