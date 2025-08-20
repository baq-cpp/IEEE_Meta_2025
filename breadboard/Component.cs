using System;
using System.Collections.Generic;

namespace breadboard
{
    public class Component
    {
        public string Label { get; }
        public bool HasGnd { get; private set; } = false;
        public bool HasVcc { get; private set; } = false;

        // Maps pin indices to Pin objects
        public Dictionary<int, Pin> Pins { get; } = new Dictionary<int, Pin>();

        // Maps pin indices to their positions on the breadboard
        public Dictionary<int, (int row, int col)> PinPositions { get; } =
            new Dictionary<int, (int row, int col)>();

        // Maps pin index -> list of neighboring components
        public Dictionary<int, List<Component>> AdjacencyList { get; private set; } =
            new Dictionary<int, List<Component>>();

        // Array of tuples representing the positions of each pin on the breadboard
        private (int row, int col)[] displayPins { get; }

        // Constructor for placing a multi-pin component on the board
        //public Component(int row, Breadboard board, string label, string side, int pinCount = 14)
        //{
        //    Label = label;
        //    displayPins = new (int, int)[pinCount];

        //    // Define columns for left and right sides
        //    int leftColLeft = 6;   // e.g., column 6 on a 30-col board
        //    int leftColRight = 8;   // e.g., column 8
        //    int rightColLeft = 21;  // e.g., column 21
        //    int rightColRight = 23;  // e.g., column 23

        //    int leftCol, rightCol;

        //    string sideLower = side == null ? "" : side.ToLower();
        //    if (sideLower == "left")
        //    {
        //        leftCol = leftColLeft;
        //        rightCol = leftColRight;
        //    }
        //    else if (sideLower == "right")
        //    {
        //        leftCol = rightColLeft;
        //        rightCol = rightColRight;
        //    }
        //    else
        //    {
        //        throw new ArgumentException("Side must be 'left' or 'right'");
        //    }

        //    for (int i = 0; i < pinCount / 2; i++)
        //    {
        //        int leftPinNum = i + 1;            // 1..7
        //        int rightPinNum = pinCount - i;     // 14..8

        //        Pins[i] = new Pin(this, leftPinNum,
        //            IsOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);

        //        Pins[pinCount - 1 - i] = new Pin(this, rightPinNum,
        //            IsOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);

        //        displayPins[i] = (row: row + i, col: leftCol);
        //        displayPins[pinCount - 1 - i] = (row: row + i, col: rightCol);

        //        PinPositions[i + 1] = displayPins[i];                          // Pin 1–7
        //        PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
        //    }

        //    // Place on board (no tuple deconstruction)
        //    for (int k = 0; k < displayPins.Length; k++)
        //    {
        //        var p = displayPins[k];
        //        if (board.IsOccupied(p.row, p.col))
        //            throw new InvalidOperationException(
        //                string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
        //                              label, p.row, p.col));
        //        board.PlaceComponent(p.row, p.col, this);
        //    }
        //}

        public Component(int row, Breadboard board, string label, string side, int pinCount = 14)
        {
            Label = label;
            displayPins = new (int, int)[pinCount];

            // Define columns for left and right sides
            int leftColLeft = 6;   // e.g., column 6 on a 30-col board
            int leftColRight = 7;   // e.g., column 8
            int rightColLeft = 20;  // e.g., column 21
            int rightColRight = 21;  // e.g., column 23

            int leftCol, rightCol;

            string sideLower = side == null ? "" : side.ToLower();
            if (sideLower == "left")
            {
                leftCol = leftColLeft;
                rightCol = leftColRight;
            }
            else if (sideLower == "right")
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
                int leftPinNum = i + 1;            // 1..7
                int rightPinNum = pinCount - i;     // 14..8

                Pins[i] = new Pin(this, leftPinNum,
                    IsOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);

                Pins[pinCount - 1 - i] = new Pin(this, rightPinNum,
                    IsOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);

                displayPins[i] = (row: row + i, col: leftCol);
                displayPins[pinCount - 1 - i] = (row: row + i, col: rightCol);

                PinPositions[i + 1] = displayPins[i];                          // Pin 1–7
                PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
            }

            // Place on board (no tuple deconstruction)
            for (int k = 0; k < displayPins.Length; k++)
            {
                var p = displayPins[k];
                if (board.IsOccupied(p.row, p.col))
                    throw new InvalidOperationException(
                        string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
                                      label, p.row, p.col));
                board.PlaceComponent(p.row, p.col, this);
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

            // Iterate without tuple deconstruction
            foreach (KeyValuePair<int, (int row, int col)> kvp in PinPositions)
            {
                int pinNumber = kvp.Key;
                var pos = kvp.Value;

                List<Component> neighbors = new List<Component>();
                var directions = GetScanDirections(pos.col, board.Columns);

                // Iterate directions without deconstruction
                for (int d = 0; d < directions.Length; d++)
                {
                    var dir = directions[d];
                    var neighborPos = (row: pos.row + dir.dr, col: pos.col + dir.dc);

                    // Check for other components nearby
                    Component neighborComp;
                    if (board.TryGetComponentAt(neighborPos, out neighborComp) && neighborComp != this)
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
                ? new (int, int)[] { (0, -1), (0, -2), (0, -3), (0, -4) }
                : new (int, int)[] { (0, 1), (0, 2), (0, 3), (0, 4) };
        }
    }
}

