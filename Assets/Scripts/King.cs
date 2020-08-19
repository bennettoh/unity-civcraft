using System.Collections;
using UnityEngine;

public class King : Chessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        Chessman c;
        int i, j;

        // top row
        if(CurrentY != 7)
        {
            i = CurrentX - 1;
            j = CurrentY + 1;

            for(int k = 0; k < 3; k++)
            {
                if(i >= 0 || i < 8)
                {
                    c = BoardManager.Instance.Chessmans[i, j];
                    if(c == null)
                    {
                        r[i, j] = true;
                    }
                    else if(c.isWhite != isWhite)
                    {
                        r[i, j] = true;
                    }
                    i++;
                }
            }
        }

        // bottom row
        if (CurrentY != 0)
        {
            i = CurrentX - 1;
            j = CurrentY - 1;

            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 || i < 8)
                {
                    c = BoardManager.Instance.Chessmans[i, j];
                    if (c == null)
                    {
                        r[i, j] = true;
                    }
                    else if (c.isWhite != isWhite)
                    {
                        r[i, j] = true;
                    }
                    i++;
                }
            }
        }

        // left
        if(CurrentX != 0)
        {
            c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY];
            if(c == null)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
            else if(c.isWhite != isWhite)
            {
                r[CurrentX - 1, CurrentY] = true;
            }
        }

        // right
        if (CurrentX != 7)
        {
            c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
            if (c == null)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
            else if (c.isWhite != isWhite)
            {
                r[CurrentX + 1, CurrentY] = true;
            }
        }

        return r;
    }
}
