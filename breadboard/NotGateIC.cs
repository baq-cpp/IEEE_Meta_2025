public class NotGateIC
{
    public Dictionary<int, (int row, int col)> PinPositions { get; } = new();
    public Dictionary<(int row, int col), List<(int row, int col)>> AdjacencyList { get; private set; } = new();
    private (int row, int col)[] pins;
    private string label;

    public NotGateIC(int row, Breadboard board, string label = "NOT")
    {
        this.label = label;
        pins = new (int, int)[14];
        int rightCol = board.Columns / 2 + 1;
        int leftCol = board.Columns / 2 - 1;
        for (int i = 0; i < 7; i++)
        {
            pins[i] = (row + i, leftCol);
            pins[13 - i] = (row + i, rightCol);
            PinPositions[i + 1] = pins[i];
            PinPositions[14 - i] = pins[13 - i];
        }
        foreach (var (r, c) in pins)
        {
            if (board.IsOccupied(r, c))
                throw new InvalidOperationException($"Cannot place NOT gate: position ({r},{c}) is already occupied.");
            board.PlaceComponent(r, c, label);
        }
    }
    public void BuildAdjacencyList(Breadboard board)
    {
        AdjacencyList.Clear();
        foreach (var pos in pins)
        {
             (int dr, int dc)[] directions;
            var neighbors = new List<(int row, int col)>();
            if (pos.col< 7)
            {
                directions = new (int dr, int dc)[] { (0, -1), (0, -2), (0, -3), (0, -4) };
            }
            else
            {
                directions = new (int dr, int dc)[] { (0, 1), (0, 2), (0, 3), (0, 4) };
            }
            foreach (var (dr, dc) in directions)
                {
                    var neighbor = (row: pos.row + dr, col: pos.col + dc);
                    if (board.IsOccupied(neighbor.row, neighbor.col))
                    {
                        neighbors.Add(neighbor);
                    }
                }
            AdjacencyList[pos] = neighbors;
        }
    }
}