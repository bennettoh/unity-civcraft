using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
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

    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        SpawnAllChessmen();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateSelection();
            Debug.Log(GameManager.Instance.intent + " " + selectionX + " " + selectionY);
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

    private void SelectChessman(int x, int y)
    {
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
                // if it was the king
                if (c.GetType() == typeof(King))
                {
                    EndGame();
                    return;
                }

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

    private void SpawnAllChessmen()
    {
        SpawnChessman(5, 1, 1);
        SpawnChessman(11, 6, 6);
    }

    // returns the coordinate of the center of the tile given its x and y location on grid
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }

    private void EndGame()
    {
        if (isWhiteTurn)
        {
            Debug.Log("White team wins");
        }
        else
        {
            Debug.Log("White team wins");
        }

        foreach (GameObject go in activeChessman)
            Destroy(go);

        isWhiteTurn = true;
        BoardHighlights.Instance.HideHighlights();
        SpawnAllChessmen();
    }

    public void Spawn()
    {
        GameManager.Instance.intent = "";
        GameManager.Instance.resource -= 5;
        if (isWhiteTurn)
        {
            SpawnChessman(5, selectionX, selectionY);
        }
        else
        {
            SpawnChessman(11, selectionX, selectionY);
        }
    }

    public void EndTurn()
    {
        foreach (GameObject go in activeChessman)
        {
            var unit = go.GetComponent<Chessman>();
            if (unit.isWhite == isWhiteTurn)
            {
                unit.resetMoves();
            }
        }
        isWhiteTurn = !isWhiteTurn;
    }
}
