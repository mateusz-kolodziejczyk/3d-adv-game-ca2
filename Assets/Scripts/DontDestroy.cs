using UnityEngine;


// Class taken from https://docs.unity3d.com/ScriptReference/Object.DontDestroyOnLoad.html
public class DontDestroy : MonoBehaviour
{
    [SerializeField] private string tag;
    private void Awake()
    {
        var objs = GameObject.FindGameObjectsWithTag(tag);

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
}