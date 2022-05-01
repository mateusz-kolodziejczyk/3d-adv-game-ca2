using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;

public class ExitController : MonoBehaviour
{
    public bool IsActive { get; set; } = true;

    private GameObject player;

    private FirstPersonController fpc;

    private PlayerShoot playerShoot;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        SwitchChildrenActive();    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwitchChildrenActive();
        }
    }

    public void SwitchChildrenActive()
    {

        IsActive = !IsActive;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(!child.gameObject.activeSelf);
        }
        // Pause game.
        Time.timeScale = IsActive ? 0 : 1;
        if (player != null)
        {
            if (IsActive)
            {
                ShutDownPlayer();
            }
            else
            {
                ReEnablePlayer();
            }
        }
    }

    private void ShutDownPlayer()
    {
        fpc ??= player.GetComponent<FirstPersonController>();
        playerShoot ??= player.GetComponent<PlayerShoot>();

        if (fpc != null)
        {
            fpc.enabled = false;
        }

        if (playerShoot != null)
        {
            playerShoot.enabled = false;
        }
    }

    private void ReEnablePlayer()
    {
        fpc ??= player.GetComponent<FirstPersonController>();
        playerShoot ??= player.GetComponent<PlayerShoot>();

        if (fpc != null)
        {
            fpc.enabled = true;
        }

        if (playerShoot != null)
        {
            playerShoot.enabled = true;
        }
    }
}
