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

    public bool isWhiteTurn = true;

    private void Start()
    {
        Instance = this;
        SpawnAllChessmen();
    }

    private void Update()
    {
        UpdateSelection();
        DrawChessboard();

        if (Input.GetMouseButtonDown (0)) {
            // Debug.Log(selectionX + " " + selectionY);
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
        if (Chessmans[x, y] == null) // check if a chessman exists on the clicked tile
            return;

        if (Chessmans[x, y].isWhite != isWhiteTurn) // can't move out of turn
            return;

        allowedMoves = Chessmans[x, y].PossibleMove();
        selectedChessman = Chessmans[x, y]; // if all tests pass, chessman at the location gets added to the selection
        BoardHighlights.Instance.HighlightAllowedMoves(allowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x,y])
        {
            // capture the enemy piece
            Chessman c = Chessmans[x, y];
            if (c != null && c.isWhite != isWhiteTurn)
            {
                // if it was the king
                if(c.GetType() == typeof(King))
                {
                    // end game
                    return;
                }

                activeChessman.Remove(c.gameObject);
                Destroy(c.gameObject);
            }

            // legal move
            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null;
            selectedChessman.transform.position = GetTileCenter(x, y);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
            isWhiteTurn = !isWhiteTurn;
        }

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
        else
        {
            selectionX = -1;
            selectionY = -1;
        }
    }

    private void DrawChessboard()
    {
        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for(int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);
            for (int j = 0; j <= 8; j++)
            {
                Vector3 height = Vector3.right * j;
                Debug.DrawLine(height, height + heightLine);
            }
        }

        // 'x' marks the grid where cursor is on
        if(selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1)
                );

            Debug.DrawLine(
                Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
                Vector3.forward * selectionY + Vector3.right * (selectionX + 1)
                );
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
        // Spawn the white team
        // King
        SpawnChessman(0, 3, 0);

        // Queen
        SpawnChessman(1, 4, 0);

        // Bishops
        SpawnChessman(2, 2, 0);
        SpawnChessman(2, 5, 0);

        // Knights
        SpawnChessman(3, 1, 0);
        SpawnChessman(3, 6, 0);

        // Rooks
        SpawnChessman(4, 0, 0);
        SpawnChessman(4, 7, 0);

        // Pawns
        for(int i = 0; i < 8; i++)
        {
            SpawnChessman(5, i, 1);
        }


        // Spawn the black team
        // King
        SpawnChessman(6, 4, 7);

        // Queen
        SpawnChessman(7, 3, 7);

        // Bishops
        SpawnChessman(8, 2, 7);
        SpawnChessman(8, 5, 7);

        // Knights
        SpawnChessman(9, 1, 7);
        SpawnChessman(9, 6, 7);

        // Rooks
        SpawnChessman(10, 0, 7);
        SpawnChessman(10, 7, 7);

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, i, 6);
        }


    }

    // returns the coordinate of the center of the tile given its x and y location on grid
    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;
        return origin;
    }
}
