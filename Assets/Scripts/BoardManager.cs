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
    private List<GameObject> activeChessman = new List<GameObject>();

    private void Start()
    {
        SpawnChessman(0, Vector3.zero);
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
}
