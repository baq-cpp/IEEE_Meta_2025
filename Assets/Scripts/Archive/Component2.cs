using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
public class Component2
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

        public string Label { get; }
        public bool HasGnd { get; private set; } = false;
        public bool HasVcc { get; private set; } = false;

        // Maps pin indices to Pin objects
        public Dictionary<int, Pin> Pins { get; } = new Dictionary<int, Pin>();

        // Maps pin indices to their positions on the breadboard
        public Dictionary<int, (int row, int col)> PinPositions { get; } =
            new Dictionary<int, (int row, int col)>();

        // Maps pin index -> list of neighboring components
        public Dictionary<int, List<Component2>> AdjacencyList { get; private set; } =
            new Dictionary<int, List<Component2>>();

        // Array of tuples representing the positions of each pin on the breadboard
        private (int row, int col)[] displayPins { get; }

        public Component2(GameObject componentObj, int row, int col, Breadboard board)
        {
            
            Label = componentObj.name;

        if (Label == "NOT")
        {
            int pinCount = 14;

            displayPins = new (int, int)[pinCount];

            // Define columns for left and right sides
            int leftCol = col - 1;   // e.g., column 8
            int rightCol = col;
            //int rightColLeft = 20;  // e.g., column 21
            //int rightColRight = 21;  // e.g., column 23

            for (int i = 0; i < pinCount / 2; i++)
            {
                int leftPinNum = i + 1;            // 1..7
                int rightPinNum = pinCount - i;     // 14..8

                Pins[i] = new Pin(this, leftPinNum,
                    IsOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);

                Pins[pinCount - 1 - i] = new Pin(this, rightPinNum,
                    IsOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);

                displayPins[i] = (row: row + i, col: rightCol);
                displayPins[pinCount - 1 - i] = (row: row + i, col: leftCol);

                PinPositions[i + 1] = displayPins[i];                          // Pin 1–7 
                PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
            }
            for (int k = 0; k < displayPins.Length; k++)
            {
                var p = displayPins[k];
                if (board.IsOccupied(p.row, p.col))
                    throw new InvalidOperationException(
                        string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
                                    Label, p.row, p.col));
                PlaceWithLog(board, row, col);
            }
        }
        else if (Label == "SWITCH")
        {
            int pinCount = 14;

            displayPins = new (int, int)[pinCount];

            int leftCol = col - 1;   // e.g., column 8
            int rightCol = col;

            for (int i = 0; i < pinCount / 2; i++)
            {
                int leftPinNum = i + 1;            // 1..7
                int rightPinNum = pinCount - i;     // 14..8

                Pins[i] = new Pin(this, leftPinNum, Pin.Direction.In);

                Pins[pinCount - 1 - i] = new Pin(this, rightPinNum, Pin.Direction.Out);

                displayPins[i] = (row: row + i, col: rightCol);
                displayPins[pinCount - 1 - i] = (row: row + i, col: leftCol);

                PinPositions[i + 1] = displayPins[i];                          // Pin 1–7 
                PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
            }
            for (int k = 0; k < displayPins.Length; k++)
            {
                var p = displayPins[k];
                if (board.IsOccupied(p.row, p.col))
                    throw new InvalidOperationException(
                        string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
                                    Label, p.row, p.col));
                PlaceWithLog(board, row, col);
            }
        }
        else if (Label == "R330" || Label == "R1000")
        {
            int resistorValue = Label == "R330" ? 330 : 1000;
            int pinCount = 2;
            displayPins = new (int, int)[pinCount];
            Pins[0] = new Pin(this, 0, Pin.Direction.In);
            Pins[1] = new Pin(this, 1, Pin.Direction.Out);
            PlaceWithLog(board, row, col);
            PlaceWithLog(board, row, col + 5);// Resistor spans two columns
        }
        else if (Label == "LED")
        {
            int pinCount = 2;
            displayPins = new (int, int)[pinCount];
            Pins[0] = new Pin(this, 0, Pin.Direction.In);
            Pins[1] = new Pin(this, 1, Pin.Direction.Out);
            PlaceWithLog(board, row, col);

        }
        else
        {
            int pinCount = 14;

            displayPins = new (int, int)[pinCount];

            // Define columns for left and right sides
            int leftCol = col;   // e.g., column 6 on a 30-col board
            int rightCol = col + 1;   // e.g., column 8

            for (int i = 0; i < pinCount / 2; i++)
            {
                int leftPinNum = i + 1;            // 1..7
                int rightPinNum = pinCount - i;     // 14..8

                Pins[i] = new Pin(this, leftPinNum,
                    IsNotOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);

                Pins[pinCount - 1 - i] = new Pin(this, rightPinNum,
                    IsNotOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);

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
                                    Label, p.row, p.col));
                PlaceWithLog(board, p.row, p.col);
            }
        }
             
        }

        // public Component(int row, int col, Breadboard board, string label, int pinCount = 14)
        // {
        //     if (label == "NOT")
        //     {
        //         Console.WriteLine("Placing NOT gate at ({0},{1})", row, col);
        //         Label = label;
        //         displayPins = new (int, int)[pinCount];

        //         // Define columns for left and right sides
        //         int leftCol = col - 1 ;   // e.g., column 8
        //         int rightCol = col ;
        //         //int rightColLeft = 20;  // e.g., column 21
        //         //int rightColRight = 21;  // e.g., column 23

        //         for (int i = 0; i < pinCount / 2; i++)
        //         {
        //             int leftPinNum = i + 1;            // 1..7
        //             int rightPinNum = pinCount - i;     // 14..8

        //             Pins[i] = new Pin(this, leftPinNum,
        //                 IsOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);

        //             Pins[pinCount - 1 - i] = new Pin(this, rightPinNum,
        //                 IsOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);

        //             displayPins[i] = (row: row + i, col: rightCol);
        //             displayPins[pinCount - 1 - i] = (row: row + i, col: leftCol);

        //             PinPositions[i + 1] = displayPins[i];                          // Pin 1–7 
        //             PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
        //         }
        //         for (int k = 0; k < displayPins.Length; k++)
        //         {
        //             var p = displayPins[k];
        //             if (board.IsOccupied(p.row, p.col))
        //                 throw new InvalidOperationException(
        //                     string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
        //                                   label, p.row, p.col));
        //             board.PlaceComponent(p.row, p.col, this);
        //         }
        //     }
        //     else if (label == "SWITCH"){
        //         Label = label;
        //         displayPins = new (int, int)[pinCount];

        //         int leftCol = col - 1;   // e.g., column 8
        //         int rightCol = col;

        //         for (int i = 0; i < pinCount / 2; i++)
        //             {
        //                 int leftPinNum = i + 1;            // 1..7
        //                 int rightPinNum = pinCount - i;     // 14..8

        //                 Pins[i] = new Pin(this, leftPinNum, Pin.Direction.In );

        //                 Pins[pinCount - 1 - i] = new Pin(this, rightPinNum, Pin.Direction.Out);

        //                 displayPins[i] = (row: row + i, col: rightCol);
        //                 displayPins[pinCount - 1 - i] = (row: row + i, col: leftCol);

        //                 PinPositions[i + 1] = displayPins[i];                          // Pin 1–7 
        //                 PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
        //         }
        //         for (int k = 0; k < displayPins.Length; k++)
        //         {
        //             var p = displayPins[k];
        //             if (board.IsOccupied(p.row, p.col))
        //                 throw new InvalidOperationException(
        //                     string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
        //                                   label, p.row, p.col));
        //             board.PlaceComponent(p.row, p.col, this);
        //         }
        //     }
        //     else
        //     {
        //         Label = label;
        //         displayPins = new (int, int)[pinCount];

        //         // Define columns for left and right sides
        //         int leftCol = col;   // e.g., column 6 on a 30-col board
        //         int rightCol = col + 1;   // e.g., column 8

        //         for (int i = 0; i < pinCount / 2; i++)
        //         {
        //             int leftPinNum = i + 1;            // 1..7
        //             int rightPinNum = pinCount - i;     // 14..8

        //             Pins[i] = new Pin(this, leftPinNum,
        //                 IsNotOutputPin(leftPinNum) ? Pin.Direction.Out : Pin.Direction.In);

        //             Pins[pinCount - 1 - i] = new Pin(this, rightPinNum,
        //                 IsNotOutputPin(rightPinNum) ? Pin.Direction.Out : Pin.Direction.In);

        //             displayPins[i] = (row: row + i, col: leftCol);
        //             displayPins[pinCount - 1 - i] = (row: row + i, col: rightCol);

        //             PinPositions[i + 1] = displayPins[i];                          // Pin 1–7 
        //             PinPositions[pinCount - i] = displayPins[pinCount - 1 - i];    // Pin 14–8
        //         }

        //         // Place on board (no tuple deconstruction)
        //         for (int k = 0; k < displayPins.Length; k++)
        //         {
        //             var p = displayPins[k];
        //             if (board.IsOccupied(p.row, p.col))
        //                 throw new InvalidOperationException(
        //                     string.Format("Cannot place {0} gate: position ({1},{2}) is already occupied.",
        //                                   label, p.row, p.col));
        //             board.PlaceComponent(p.row, p.col, this);
        //         }
        //     }
        // }

        // Constructor for standalone components (e.g., wires)
        public Component2(string label)
        {
            Label = label;
        }

        private void PlaceWithLog(Breadboard board, int row, int col)
        {
            board.PlaceComponent(row, col, this);
            // UnityEngine.Debug.Log($"Placed {Label} at ({row},{col})");
        }

        private bool IsOutputPin(int pinNumber)
        {
            return pinNumber == 3 || pinNumber == 6 || pinNumber == 8 || pinNumber == 11;
        }

        private bool IsNotOutputPin(int pinNumber)
        {
            return pinNumber == 2 || pinNumber == 4 || pinNumber == 6 || pinNumber == 8 || pinNumber == 10 || pinNumber == 12;
        }

        public void CreateAdjacencyList(Breadboard board)
        {
            AdjacencyList.Clear();

            // Iterate without tuple deconstruction
            foreach (KeyValuePair<int, (int row, int col)> kvp in PinPositions)
            {
                int pinNumber = kvp.Key;
                var pos = kvp.Value;

                List<Component2> neighbors = new List<Component2>();
                var directions = GetScanDirections(pos.col, board.Columns);

                // Iterate directions without deconstruction
                for (int d = 0; d < directions.Length; d++)
                {
                    var dir = directions[d];
                    var neighborPos = (row: pos.row + dir.dr, col: pos.col + dir.dc);

                    // Check for other components nearby
                    Component2 neighborComp;
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

    public static (int row, int col) ParseOrderedPair(string pair)
    {
    
        // Split by comma
        string[] parts = pair.Split(',');

        if (parts.Length != 2)
            throw new System.ArgumentException($"Invalid pair format: {pair}");

        // Parse row and col (handles leading zeros like "01")
        int row = int.Parse(parts[0]);
        int col = int.Parse(parts[1]);

        return (row, col);
    }



}
