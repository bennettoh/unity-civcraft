using System.Collections;
using UnityEngine;

public class King : Chessman
{
    public King()
    {
        this.moveType = "spawn";
    }

    public override bool[,] PossibleMove()
    {
        bool[,] moveArr = new bool[8, 8];
        (int, int)[] directions = new[] { (0, 1), (1, 0), (0, -1), (-1, 0) };

        // find valid moves in 4 directions
        for (int i = 0; i < directions.Length; i++)
        {
            (int, int) direction = directions[i];
            int xCheck = CurrentX + direction.Item1;
            int yCheck = CurrentY + direction.Item2;

            // check if selection falls off the board
            if (xCheck <= 7 && xCheck >= 0 && yCheck <= 7 && yCheck >= 0)
            {
                Chessman unit = BoardManager.Instance.Chessmans[xCheck, yCheck];
                if (unit == null)
                {
                    moveArr[xCheck, yCheck] = true;
                }
            }
        }
        return moveArr;
    }
}
