using UnityEngine;
using System.Collections;

public class CoinFlyEffect : MonoBehaviour
{
    public static CoinFlyEffect Instance;

    [Header("UI References")]
    public RectTransform targetIcon;
    public Canvas canvas;
    public GameObject flyingCoinPrefab;

    [Header("Settings")]
    public float flyDuration = 0.7f;
    public float curveHeight = 80f;
    public float spreadRadius = 60f;
    public float delayBetweenCoins = 0.05f;

    private Camera cam;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
    }

    public void PlayFlyEffect(Vector3 worldPosition, int amount = 1)
    {
        StartCoroutine(FlyMultiple(worldPosition, amount));
    }

    private IEnumerator FlyMultiple(Vector3 worldPos, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            StartCoroutine(FlySingle(worldPos));
            yield return new WaitForSeconds(delayBetweenCoins);
        }
    }

    private IEnumerator FlySingle(Vector3 worldPos)
    {
        if (flyingCoinPrefab == null || targetIcon == null || canvas == null)
            yield break;

        GameObject flyingCoin = Instantiate(flyingCoinPrefab, canvas.transform);
        RectTransform rect = flyingCoin.GetComponent<RectTransform>();

        Vector3 screenStart = cam.WorldToScreenPoint(worldPos);

        Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
        screenStart += new Vector3(randomCircle.x, randomCircle.y, 0);

        Vector3 screenEnd = targetIcon.position;

        //add randm offset
        screenEnd += new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);

        float randomCurve = Random.Range(0.7f, 1.3f) * curveHeight;

        float time = 0f;

        while (time < flyDuration)
        {
            time += Time.deltaTime;
            float t = time / flyDuration;

            float smoothT = Mathf.SmoothStep(0, 1, t);

            Vector3 curveOffset = Vector3.up * Mathf.Sin(smoothT * Mathf.PI) * randomCurve;

            rect.position = Vector3.Lerp(screenStart, screenEnd, smoothT) + curveOffset;

            yield return null;
        }


        Destroy(flyingCoin);
    }
}//Class