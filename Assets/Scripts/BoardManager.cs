using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = 0.5f;

    // coordinates of selected pieces (-1 indicates none selected)
    private int selectionX = -1;
    private int selectionY = -1;

    public List<GameObject> chessmanPrefabs;
    private List<GameObject> activeChessman;

    private void Start()
    {
        SpawnAllChessmen();
    }

    private void Update()
    {
        UpdateSelection();
        DrawChessboard();
    }

    private void UpdateSelection()
    {
        if (!Camera.main) return;

        // pick up where mouse is pointing and print coordinate
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            // remove decimal
            selectionX = (int)hit.point.x;
            selectionY = (int)hit.point.z;
            // Debug.DrawLine(Vector3.forward, hit.point);
        } else
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

    private void SpawnChessman(int index, Vector3 position)
    {
        // temporary chess piece
        GameObject go = Instantiate(chessmanPrefabs[index], position, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        activeChessman.Add(go);
    }

    private void SpawnAllChessmen()
    {
        activeChessman = new List<GameObject>();

        // Spawn the white team
        // King
        SpawnChessman(0, GetTileCenter(3, 0));

        // Queen
        SpawnChessman(1, GetTileCenter(4, 0));

        // Bishops
        SpawnChessman(2, GetTileCenter(2, 0));
        SpawnChessman(2, GetTileCenter(5, 0));

        // Knights
        SpawnChessman(3, GetTileCenter(1, 0));
        SpawnChessman(3, GetTileCenter(6, 0));

        // Rooks
        SpawnChessman(4, GetTileCenter(0, 0));
        SpawnChessman(4, GetTileCenter(7, 0));

        // Pawns
        for(int i = 0; i < 8; i++)
        {
            SpawnChessman(5, GetTileCenter(i, 1));
        }


        // Spawn the black team
        // King
        SpawnChessman(6, GetTileCenter(3, 7));

        // Queen
        SpawnChessman(7, GetTileCenter(4, 7));

        // Bishops
        SpawnChessman(8, GetTileCenter(2, 7));
        SpawnChessman(8, GetTileCenter(5, 7));

        // Knights
        SpawnChessman(9, GetTileCenter(1, 7));
        SpawnChessman(9, GetTileCenter(6, 7));

        // Rooks
        SpawnChessman(10, GetTileCenter(0, 7));
        SpawnChessman(10, GetTileCenter(7, 7));

        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawnChessman(11, GetTileCenter(i, 6));
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
