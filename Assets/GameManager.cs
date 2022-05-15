using UnityEngine;

// Static cl
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public const int WHITE_START_RESOURCE = 10;
    public const int BLACK_START_RESOURCE = 10;

    public int whiteResource;
    public int blackResource;
    public string intent;
    public bool isWhiteTurn = true;

    private void Awake()
    {
        Instance = this;
    }

    public void ToggleIntent()
    {
        intent = intent == "build" ? "move" : "build";
    }

    public void endTurn()
    {
        isWhiteTurn = !isWhiteTurn;
    }
}