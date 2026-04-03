using UnityEngine;

public class SpeedUpOnEnabel : MonoBehaviour
{
    //===================== SpeedPowerup Invisible Bridge =================

    private PlayerControler playerController;

    public GameObject invisibelBridge;

    void Start()
    {
        playerController = Object.FindAnyObjectByType<PlayerControler>();

        if (playerController == null)
        {
            Debug.LogError("PlayerControler not found in scene!");
        }
    }

    void Update()
    {
        if (playerController == null) return;

        invisibelBridge.SetActive(playerController.isPowerUpOn);
    }
}
