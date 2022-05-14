using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public const int WHITE_BASE_ID = 0;
    public const int BLACK_BASE_ID = 6;

    public const int WHITE_UNIT_ID = 5;
    public const int BLACK_UNIT_ID = 11;

    public const int WHITE_START_ROW = 1;
    public const int WHITE_START_COL = 1;
    public const int BLACK_START_ROW = 6;
    public const int BLACK_START_COL = 6;

    public const int BASE_COST = 3;
    public const int UNIT_COST = 1;

    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { get; set; }

    public Chessman[,] Chessmans { set; get; } = new Chessman[8, 8];
    private Chessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    // coordinates of selected pieces (-1 indicates none selected)
    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman = new List<GameObject>();

    private Material previousMat;
    public Material selectedMat;

    public bool isWhiteTurn;

    public bool whiteBaseExists;
    public bool blackBaseExists;

    private void Start()
    {
        Instance = this;
        InitializeBoard();
    }

    private void Update()
    {
        isWhiteTurn = GameManager.Instance.isWhiteTurn;

        // check if the game is over
        whiteBaseExists = false;
        blackBaseExists = false;
        foreach (GameObject go in activeChessman)
        {
            var unit = go.GetComponent<Chessman>();
            if (unit.GetType() == typeof(King))
            {
                //Debug.Log("Found a king.");
                if (unit.isWhite) whiteBaseExists = true;
                else blackBaseExists = true;
            }
        }
        if (!whiteBaseExists)
        {
            // no more white bases, black wins
            EndGame(false);
        }
        else if (!blackBaseExists)
        {
            // no more black bases, white wins
            EndGame(true);
        }

        // if the game did not end, carry on
        if (Input.GetMouseButtonDown(0))
        {
            UpdateSelection();
            // Debug.Log(GameManager.Instance.intent + " " + selectionX + " " + selectionY);
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    // if no chessman is currently selected, select the chessman on the current grid coordinate
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    // if a chessman is already selected, move the chessman to the given spot
                    MoveChessman(selectionX, selectionY);
                }
            }
        }
    }

    // A method to set up the beginning conditions of the board,
    // for either starting to re-starting a game.
    private void InitializeBoard() {

        GameManager.Instance.whiteResource = GameManager.WHITE_START_RESOURCE;
        GameManager.Instance.blackResource = GameManager.BLACK_START_RESOURCE;
        
        SpawnChessman(WHITE_BASE_ID, WHITE_START_ROW, WHITE_START_COL);
        SpawnChessman(BLACK_BASE_ID, BLACK_START_ROW, BLACK_START_COL);

        GameManager.Instance.isWhiteTurn = true;

    }

    private void SelectChessman(int x, int y)
    {

        Debug.Log(x + " " + y);
        if (Chessmans[x, y] == null) // check if a chessman exists on the tile
        {
            GameManager.Instance.intent = "tile";
            return;
        }
        else
        {
            GameManager.Instance.intent = "";
        }

        if (Chessmans[x, y].isWhite != isWhiteTurn) // out of turn
            return;

        bool hasAtLeastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (allowedMoves[i, j])
                {
                    hasAtLeastOneMove = true;
                }
            }
        }
        if (!hasAtLeastOneMove)
            return;

        selectedChessman = Chessmans[x, y]; // if all tests pass, chessman at the location gets added to the selection
        previousMat = selectedChessman.GetComponentInChildren<MeshRenderer>().material;
        selectedMat.mainTexture = previousMat.mainTexture;
        selectedChessman.GetComponentInChildren<MeshRenderer>().material = selectedMat;
        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {
            // capture the enemy piece
            Chessman c = Chessmans[x, y];
            if (c != null && c.isWhite != isWhiteTurn)
            {
                // remove enemy piece from the game
                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }

            // legal move
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.transform.position = GetTileCenter(x, y);
            selectedChessman.SetPosition(x, y);
            selectedChessman.useMove();
            Chessmans[x, y] = selectedChessman;
        }

        selectedChessman.GetComponentInChildren<MeshRenderer>().material = previousMat;
        BoardHighlights.Instance.HideHighlights();
        selectedChessman = null; // illegal move de-selects the piece
    }

    private void UpdateSelection()
    {
        if (!Camera.main) return;

        RaycastHit hit;

        // check if mouse cursor intersects with the plane and round up the decimal to grid number
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            // remove decimal
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
        }
    }

    private void SpawnChessman(int index, int x, int y)
    {
        // temporary chess piece
        GameObject go = Instantiate(chessmanPrefabs[index], GetTileCenter(x, y), Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        Chessmans[x, y] = go.GetComponent<Chessman>();
        Chessmans[x, y].SetPosition(x, y);
        activeChessman.Add(go);
    }

    // returns the coordinate of the center of the tile given its x and y location on grid
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void EndGame(bool whiteWon)
    {
        if (whiteWon)
        {
            Debug.Log("White team wins");
        }
        else
        {
            Debug.Log("Black team wins");
        }

        foreach (GameObject go in activeChessman)
            Destroy(go);

        activeChessman.Clear(); // safe to clear, after destroying contents

        BoardHighlights.Instance.HideHighlights();

        InitializeBoard(); // re-initialize the board state
    }

    public void Build()
    {
        GameManager.Instance.intent = "";
        if (isWhiteTurn && GameManager.Instance.whiteResource >= BASE_COST)
        {
            GameManager.Instance.whiteResource -= BASE_COST;
            SpawnChessman(WHITE_BASE_ID, selectionX, selectionY);
        }
        else if (!isWhiteTurn && GameManager.Instance.blackResource >= BASE_COST)
        {
            GameManager.Instance.blackResource -= BASE_COST;
            SpawnChessman(BLACK_BASE_ID, selectionX, selectionY);
        }
        else {
            Debug.Log("Not enough resources to build");
        }

        // not important to Deselect, since buildings can't move
    }

    public void Spawn()
    {
        GameManager.Instance.intent = "";
        if (isWhiteTurn && GameManager.Instance.whiteResource >= UNIT_COST)
        {
            GameManager.Instance.whiteResource -= UNIT_COST;
            SpawnChessman(WHITE_UNIT_ID, selectionX, selectionY);
        }
        else if (!isWhiteTurn && GameManager.Instance.blackResource >= UNIT_COST)
        {
            GameManager.Instance.blackResource -= UNIT_COST;
            SpawnChessman(BLACK_UNIT_ID, selectionX, selectionY);
        }
        else {
            Debug.Log("Not enough resources to spawn");
        }

        Deselect();
        // If not Deselected, the unit's position will be the last selection,
        // and ending the turn without moving will give the opponent the choice
        // to move the player's piece
    }

    public void EndTurn()
    {
        foreach (GameObject go in activeChessman)
        {
            var unit = go.GetComponent<Chessman>();
            if (unit.isWhite == isWhiteTurn)
            {
                unit.resetMoves();
                unit.produce();
            }
        }
        GameManager.Instance.endTurn();
        Deselect(); // make sure no square is selected at beginning of turn
    }

    private void Deselect()
    {
        Debug.Log("reset");
        this.selectionX = -1;
        this.selectionY = -1;
    }
}
