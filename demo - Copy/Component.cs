
public class Component
{
    public string Label { get; }
    public bool HasGnd { get; private set; } = false;
    public bool HasVcc { get; private set; } = false;

    //dictionary maps pin indices to Pin objects
    public Dictionary<int , Pin > Pins { get; } = new();

    // Dictionary mapping pin indices to their positions on the breadboard
    public Dictionary<int, (int row, int col)> PinPositions { get; } = new();

    // Adjacency list mapping componenxt to its neighboring components
    public Dictionary<int, List<Component>> AdjacencyList { get; private set; } = new();

    // array of tuples representing the positions of each pin on the breadboard
    private (int row, int col)[] displayPins { get; }

    // Constructor for placing a multi-pin component on the board
    public Component(int row, Breadboard board, string label, string side, int pinCount = 14)
    {

        Label = label;
        displayPins = new (int, int)[pinCount];

        // Define columns for left and right sides
        int leftColLeft = 6;                  // e.g., column 6 on a 30-col board
        int leftColRight = 8;                  // e.g., column 8
        int rightColLeft = 19;              // e.g., column 21
        int rightColRight = 21;              // e.g., column 23

        int leftCol, rightCol;

        // Choose columns based on the side
        if (side.ToLower() == "left")
        {
            leftCol = leftColLeft;
            rightCol = leftColRight;
        }
        else if (side.ToLower() == "right")
        {
            leftCol = rightColLeft;
            rightCol = rightColRight;
        }
        else
        {
            throw new ArgumentException("Side must be 'left' or 'right'");
        }

        for (int i = 0; i < pinCount / 2; i++)
        {
            int leftPinNum = i + 1;
            int rightPinNum = pinCount - i;

            Pins[i] = new Pin(this, leftPinNum, IsOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);
            Pins[pinCount - 1 - i] = new Pin(this, rightPinNum, IsOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);


            displayPins[i] = (row: row + i, col: leftCol);
            displayPins[pinCount - 1 - i] = (row: row + i, col: rightCol);

            

            PinPositions[i + 1] = displayPins[i];                          // Pin 1–7
            PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];   // Pin 14–8
        }

        foreach (var (r, c) in displayPins)
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

    private bool IsOutputPin(int pinNumber)
    {
        return pinNumber == 3 || pinNumber == 6 || pinNumber == 8 || pinNumber == 11;
    }
    public void CreateAdjacencyList(Breadboard board)
    {
        AdjacencyList.Clear();

        foreach (var (pinNumber, pos) in PinPositions)
        {
            var neighbors = new List<Component>();
            var directions = GetScanDirections(pos.col, board.Columns);

            foreach (var (dr, dc) in directions)
            {
                var neighborPos = (row: pos.row + dr, col: pos.col + dc);

                // Check for other components nearby
                if (board.TryGetComponentAt(neighborPos, out var neighborComp) && neighborComp != this)
                {
                    neighbors.Add(neighborComp);
                }

                // Check for Vcc and GND connection
                if (pinNumber == 14 && board.Vcc.Contains(neighborPos))
                {
                    HasVcc = true;
                }
                if (pinNumber == 7 && board.Gnd.Contains(neighborPos))
                {
                    HasGnd = true;
                }
            }

            // Store the list for this pin
            if (neighbors.Count > 0)
                AdjacencyList[pinNumber] = neighbors;
        }
    }


    private (int dr, int dc)[] GetScanDirections(int col, int totalColumns)
    {
        // Determine scan direction based on side of board
        return col < totalColumns / 2
            ? new[] { (0, -1), (0, -2), (0, -3), (0, -4) }
            : new[] { (0, 1), (0, 2), (0, 3), (0, 4) };
    }
}
