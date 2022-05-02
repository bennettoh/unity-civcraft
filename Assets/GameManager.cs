using UnityEngine;

// Static cl
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public float whiteResource = 10;
    public float blackResource = 10;
    public string intent;
    public bool isWhiteTurn = true;

    private void Awake()
    {
        Instance = this;
    }

    public void endTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }
}