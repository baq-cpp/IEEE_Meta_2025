public class XorGateIC
{
    public Dictionary<int, (int row, int col)> PinPositions { get; } = new();
    private (int row, int col)[] pins;
    private string label;

    public XorGateIC(int row, Breadboard board, string label = "XOR")
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
                throw new InvalidOperationException($"Cannot place XOR gate: position ({r},{c}) is already occupied.");
            board.PlaceComponent(r, c, label);
        }
    }
}