using UnityEngine;

public class ContinueMenu : MonoBehaviour
{
    public bool IsActive { get; set; } = true;
    // Start is called before the first frame update
    void Start()
    {
        SwitchChildrenActive();    
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
    }
}
