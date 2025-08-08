using breadboard;


public class Component
{
    public string Label { get; }
    public bool HasGnd { get; private set; } = false;
    public bool HasVcc { get; private set; } = false;

    //dictionary maps pin indices to Pin objects
    public Dictionary<int , Pin > Pins { get; } = new();

    // Dictionary mapping pin indices to their positions on the breadboard
    public Dictionary<int, (int row, int col)> PinPositions { get; } = new();

    // Adjacency list mapping component to its neighboring components
    public Dictionary<int, List<Component>> AdjacencyList { get; private set; } = new();

    // array of tuples representing the positions of each pin on the breadboard
    private (int row, int col)[] pins { get; }

    // Constructor for placing a multi-pin component on the board
    public Component(int row, Breadboard board, string label, int pinCount = 14)
    {
        Label = label;
        pins = new (int, int)[pinCount];

        int rightCol = board.Columns / 2 + 1;
        int leftCol = board.Columns / 2 - 1;

        for (int i = 0; i < pinCount / 2; i++)
        {
            pins[i] = (row: row + i, col: leftCol);
            pins[pinCount - 1 - i] = (row: row + i, col: rightCol);

            PinPositions[i + 1] = pins[i];
            PinPositions[pinCount - i] = pins[pinCount - 1 - i];
        }

        foreach (var (r, c) in pins)
        {
            if (board.IsOccupied(r, c))
                throw new InvalidOperationException($"Cannot place {label} gate: position ({r},{c}) is already occupied.");
            board.PlaceComponent(r, c, this);
        }
    }

    // Constructor for standalone components (e.g., wires)
    public Component(string label)
    {
        Label = label;
    }

    public void BuildAdjacencyList(Breadboard board)
    {
        AdjacencyList.Clear();

        // Temporary adjacency list: (row, col) → List<(row, col)>
        var localAdj = new Dictionary<(int row, int col), List<(int row, int col)>>();

        foreach (var pos in pins)
        {
            var neighbors = new List<(int row, int col)>();
            var directions = GetScanDirections(pos.col, board.Columns);

            foreach (var (dr, dc) in directions)
            {
                var neighbor = (row: pos.row + dr, col: pos.col + dc);

                if (board.IsOccupied(row: neighbor.row, col: neighbor.col))
                    neighbors.Add(neighbor);

                if (PinPositions.TryGetValue(14, out var pin14Pos) && pos == pin14Pos && board.Vcc.Contains(neighbor))
                {
                    HasVcc = true;
                    neighbors.Add(neighbor);
                }

                if (PinPositions.TryGetValue(7, out var pin7Pos) && pos == pin7Pos && board.Gnd.Contains(neighbor))
                {
                    HasGnd = true;
                    neighbors.Add(neighbor);
                }
            }

            localAdj[pos] = neighbors;
        }

        // Now convert to Component-to-Component level
        var componentNeighbors = new HashSet<Component>();

        foreach (var (pinPos, neighborPositions) in localAdj)
        {
            foreach (var neighborPos in neighborPositions)
            {
                if (board.TryGetComponentAt(neighborPos, out var neighborComp) && neighborComp != this)
                {
                    componentNeighbors.Add(neighborComp);
                }
            }
        }

        AdjacencyList[this] = new List<Component>(componentNeighbors);
    }


    private (int dr, int dc)[] GetScanDirections(int col, int totalColumns)
    {
        // Determine scan direction based on side of board
        return col < totalColumns / 2
            ? new[] { (0, -1), (0, -2), (0, -3), (0, -4) }
            : new[] { (0, 1), (0, 2), (0, 3), (0, 4) };
    }
}
