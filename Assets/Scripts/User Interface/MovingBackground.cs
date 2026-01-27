using UnityEngine;

public class MovingBackground : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveDistance = 200f;   
    public float moveSpeed = 100f;    

    private RectTransform rect;
    private float startX;
    private float targetX;
    private int direction = 1;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startX = rect.anchoredPosition.x;
        targetX = startX + moveDistance;
    }

    void Update()
    {
        Vector2 pos = rect.anchoredPosition;

        pos.x += direction * moveSpeed * Time.unscaledDeltaTime;
        rect.anchoredPosition = pos;

        if (direction == 1 && pos.x >= targetX)
        {
            direction = -1;
            targetX = startX - moveDistance;
        }
        else if (direction == -1 && pos.x <= targetX)
        {
            direction = 1;
            targetX = startX + moveDistance;
        }
    }
}
