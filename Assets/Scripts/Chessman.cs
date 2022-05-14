using System.Collections;
using UnityEngine;

public abstract class Chessman : MonoBehaviour
{
    public int CurrentX { set; get; }
    public int CurrentY { set; get; }
    public bool isWhite;
    private int moves = 1;
    public int production;
    public string moveType;

    public void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
    }

    public virtual bool[,] PossibleMove()
    {
        return new bool[8, 8];
    }

    public void useMove()
    {
        this.moves -= 1;
    }

    public int getMoves()
    {
        return this.moves;
    }

    public void resetMoves()
    {
        this.moves = 1;
    }

    public void produce()
    {
        if (this.isWhite)
        {
            GameManager.Instance.whiteResource += production;
        }
        else
        {
            GameManager.Instance.blackResource += production;
        }
    }
}
