public static class Wire
{
    public static void Connect((int row, int col) a, (int row, int col) b, string ID, Breadboard board, string type)
    {
        var wire = new Component(ID);
        switch (type)
        {
            case "I":
            case "G":
                board.PlaceComponent(a.row, a.col, wire);
                board.PlaceComponent(b.row, b.col-1, wire);
                board.AddLogicalConnection(a, b);
                break;
            case "P":
                board.PlaceComponent(a.row, a.col, wire);
                board.PlaceComponent(b.row, b.col+1, wire);
                board.AddLogicalConnection(a, b);
                break;
            case "G2G":
                board.PlaceComponent(a.row, a.col - 1, wire);
                board.PlaceComponent(b.row, b.col - 1, wire);
                board.AddLogicalConnection(a, b);
                break;

        }
            
        //}


    }
}
