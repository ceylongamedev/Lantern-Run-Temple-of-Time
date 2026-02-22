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
    public float flyDuration = 0.6f;
    public float curveHeight = 50f; 

    private void Awake()
    {
        Instance = this;
    }

    public void PlayFlyEffect(Vector3 worldPosition)
    {
        StartCoroutine(Fly(worldPosition));
    }

    private IEnumerator Fly(Vector3 worldPos)
    {
        if (flyingCoinPrefab == null || targetIcon == null || canvas == null) yield break;

        GameObject flyingCoin = Instantiate(flyingCoinPrefab, canvas.transform);
        RectTransform rect = flyingCoin.GetComponent<RectTransform>();

        Vector3 startPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector3 endPos = targetIcon.position;

        float time = 0f;

        while (time < flyDuration)
        {
            time += Time.deltaTime;
            float t = time / flyDuration;
            float smoothT = t * t * (3f - 2f * t);
            Vector3 curveOffset = Vector3.up * Mathf.Sin(smoothT * Mathf.PI) * curveHeight;
            rect.position = Vector3.Lerp(startPos, endPos, smoothT) + curveOffset;

            yield return null;
        }

        rect.position = endPos;
        Destroy(flyingCoin);
    }
}//Class