using UnityEngine;
using System;

public class Wire
{


    public static void Connect((int row, int col) a, (int row, int col) b, string ID, string type)
    {
        var board = Breadboard.Instance;
        if (!board.IsInitialized)
        {
            Debug.LogError("[Wire.Connect] Breadboard not initialized yet.");
            return;
        }

        var wire = new Component2(ID);

        switch (type)
        {
            case "I":
            case "G":
                {
                    var bLeft = (b.row, col: b.col - 1);
                    if (!In(board, a) || !In(board, bLeft))
                    {
                        Debug.LogError($"[Wire.Connect] I/G OOB. a=({a.row},{a.col}) b-1=({bLeft.row},{bLeft.col})");
                        return;
                    }
                    board.PlaceComponent(a.row, a.col, wire);
                    board.PlaceComponent(bLeft.row, bLeft.col, wire);
                    board.AddLogicalConnection(a, b);
                    break;
                }
            case "P":
                {
                    var bRight = (b.row, col: b.col + 1);
                    if (!In(board, a) || !In(board, bRight))
                    {
                        Debug.LogError($"[Wire.Connect] P OOB. a=({a.row},{a.col}) b+1=({bRight.row},{bRight.col})");
                        return;
                    }
                    board.PlaceComponent(a.row, a.col, wire);
                    board.PlaceComponent(bRight.row, bRight.col, wire);
                    board.AddLogicalConnection(a, b);
                    break;
                }
            case "G2G":
                {
                    if (!In(board, a) || !In(board, b))
                    {
                        Debug.LogError($"[Wire.Connect] G2G OOB. a=({a.row},{a.col}) b=({b.row},{b.col})");
                        return;
                    }
                    board.PlaceComponent(a.row, a.col, wire);
                    board.PlaceComponent(b.row, b.col, wire);
                    board.AddLogicalConnection(a, b);
                    break;
                }
            default:
                Debug.LogError($"[Wire.Connect] Unknown type '{type}'.");
                break;
        }
    }
     private static bool In(Breadboard board, (int row, int col) p)
        => p.row >= 0 && p.row < board.Rows && p.col >= 0 && p.col < board.Columns;
}
