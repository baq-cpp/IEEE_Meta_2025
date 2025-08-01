public class Wire
{
    public static void Connect((int row, int col) a, (int row, int col) b,string ID, Breadboard board)
    {
        if ( !board.IsOccupied(a.row, a.col) && !board.IsOccupied(b.row, b.col))
        {
            board.PlaceComponent(a.row, a.col, ID);
            board.PlaceComponent(b.row, b.col, ID);
            board.AddLogicalConnection(a, b);
        }
        else
        {
            board.PlaceComponent(a.row, a.col, ID);
            board.PlaceComponent(b.row, b.col, ID);
            board.AddLogicalConnection(a, b);
        }


    }
}
