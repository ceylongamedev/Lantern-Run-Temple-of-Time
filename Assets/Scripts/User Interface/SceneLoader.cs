using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] CanvasGroup loadingScreen;
    [SerializeField] float fadeSpeed = 3f;

    Image progressBar;
    TMP_Text progressText;

    bool isLoading;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        HideLoadingScreenInstant();
    }

    public void LoadScene(int index)
    {
        if (isLoading) return;

        var ls = loadingScreen.GetComponent<LoadingScreen>();
        progressBar = ls.progressionBar;
        progressText = ls.progressionText;

        StartCoroutine(LoadSceneAsync(index));
    }

    IEnumerator LoadSceneAsync(int sceneIndex)
    {
        isLoading = true;
        yield return FadeInLoadingScreen();

        float minLoadTime = 5f;
        float elapsed = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;

        float visualProgress = 0f;

        while (!op.isDone)
        {
            elapsed += Time.deltaTime;

            float realProgress = Mathf.Clamp01(op.progress / 0.9f);
            float timeProgress = Mathf.Clamp01(elapsed / minLoadTime);
            float targetProgress = Mathf.Min(realProgress, timeProgress);

            visualProgress = Mathf.MoveTowards(visualProgress, targetProgress, Time.deltaTime);

            if (progressBar)
                progressBar.fillAmount = visualProgress;

            if (progressText)
                progressText.text = Mathf.RoundToInt(visualProgress * 100f) + "%";

            if (realProgress >= 1f && elapsed >= minLoadTime)
                op.allowSceneActivation = true;

            yield return null;
        }

        yield return FadeOutLoadingScreen();
        isLoading = false;
    }

    IEnumerator FadeInLoadingScreen()
    {
        loadingScreen.blocksRaycasts = true;
        while (loadingScreen.alpha < 1f)
        {
            loadingScreen.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutLoadingScreen()
    {
        while (loadingScreen.alpha > 0f)
        {
            loadingScreen.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        loadingScreen.blocksRaycasts = false;
        HideLoadingScreenInstant();
    }

    void HideLoadingScreenInstant()
    {
        loadingScreen.alpha = 0f;
        loadingScreen.blocksRaycasts = false;

        if (progressBar)
            progressBar.fillAmount = 0f;

        if (progressText)
            progressText.text = "";
    }
}
