using UnityEngine;

public class MahasonaDetact : MonoBehaviour
{
    [Header("Settings")]
    public float range = 5f;
    public string playerTag = "Player";

    private Transform player;
    private bool hasTriggered = false;
    private AudioSource audioSource;
    private AudioClip audioClip;

    [Header("=== Effects ===")]
    public GameObject completeEffect;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        audioSource = GetComponent<AudioSource>();

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found! Make sure tag is correct.");
    }

    void Update()
    {
        if (player == null || hasTriggered) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= range)
        {
            hasTriggered = true;
            OnPlayerEnterRange();
        }
    }

    public void CompleteOrder()
    {
        if (completeEffect != null)
        {
            GameObject fx = Instantiate(completeEffect, transform.position + Vector3.up, Quaternion.identity);

            // Auto destroy
            ParticleSystem ps = fx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                Destroy(fx, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                Destroy(fx, 3f);
            }
        }

    }

    void OnPlayerEnterRange()
    {
        Debug.Log("Player entered range!");
        if (audioSource != null && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip);
        }
        CompleteOrder();
        DestroyObject();
    }

    void DestroyObject()
    {
        Destroy(gameObject, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
