using UnityEngine;

// Static cl
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float resource = 100;
    public string intent;

    private void Awake()
    {
        Instance = this;
    }
}