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
    [SerializeField]private GameObject pauseMenu;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private bool AllowPause;

    [Header("In game Hud Elements")]
    [SerializeField] private TextMeshProUGUI giftText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private TextMeshProUGUI giftAmountText;
    [SerializeField] private GameObject powerupParent;
    public Image powerUpFillImage;

    [Header("Countdown UI")]
    [SerializeField] private TextMeshProUGUI countdownText;


    private int currentHearts;
    private int giftsCollected;

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

        if(countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
        if(powerupParent != null)
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
            EndGame(true);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartPowerUp(5);
        }

        if (Input.GetKeyDown(KeyCode.B)) 
        {
            updateGifts(1);
        }
    }

    public void EndGame(bool didWin)
    {
        endScreen.SetActive(true);
        InGameMenu.SetActive(false);
        AllowPause = false;
        if(didWin)
        {
           if(gameOverText != null && giftAmountText !=null)
            {
                gameOverText.text = ("Level Complete!");
                giftAmountText.text = giftsCollected.ToString("0");
            }
        }
        else
        {
            if (gameOverText != null && giftAmountText != null)
            {
                gameOverText.text = ("Game Over");
                giftAmountText.text = giftsCollected.ToString("0");
            }
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

    public void updateGifts(int amount)
    {
        if(giftText == null)
        {
            Debug.LogError("No Gift Text Assigned");
            return;
        }
        giftsCollected = giftsCollected + amount;
        giftText.text = giftsCollected.ToString();
    }

  
    public void LoadScene(int index)
    {
        if(paused)
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
    private System.Collections.IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = true;

        yield return null;
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
