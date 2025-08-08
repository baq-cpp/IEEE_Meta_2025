class Program
{
    static void Main()
    {
        //    var board = new Breadboard(63, 15);
        //    var andGate = new AndGateIC(2, board);
        //    var xorGate = new XorGateIC(11, board);
        //    Wire.Connect(board.vcc[0], (2,2),"A", board);
        //    //Wire.Connect(andGate.PinPositions[2], xorGate.PinPositions[2], board);
        //    Wire.Connect(board.vcc[1], andGate.PinPositions[2],"B", board);



        //    board.Display();
        //    board.BuildAdjacencyList();

        //    // For each gate, for each pin, check adjacency (excluding self-connections and same-gate connections)
        //    var gates = new[] { ("AND", andGate.PinPositions), ("XOR", xorGate.PinPositions) };
        //    foreach (var (name, pins) in gates)
        //    {
        //        foreach (var pinKvp in pins)
        //        {
        //            var pinNum = pinKvp.Key;
        //            var pos = pinKvp.Value;
        //            if (board.AdjacencyList.TryGetValue(pos, out var neighbors))
        //            {
        //                foreach (var neighbor in neighbors)
        //                {
        //                    foreach (var (otherName, otherPins) in gates)
        //                    {
        //                        var otherPin = otherPins.FirstOrDefault(p => p.Value == neighbor);
        //                        // Exclude self-connections and same-gate connections
        //                        if (!otherPin.Equals(default(KeyValuePair<int, (int, int)>)) &&
        //                            name != otherName) // Only connect pins from different gates
        //                        {
        //                            Console.WriteLine($"{name} pin {pinNum} is connected to: {otherName} pin {otherPin.Key}");
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        var board = new Breadboard(63, 15);

        // Place gates
        var andGate = new Component(0, board, "AND");
        var xorGate = new Component(11, board, "XOR");

        // Connect power and ground for AND gate
        Wire.Connect(board.Vcc[0], andGate.PinPositions[14], "Vcc", board, "P");
        Wire.Connect(board.Gnd[0], andGate.PinPositions[7], "GND", board, "G");

        // Connect ground for XOR gate (Vcc intentionally left disconnected for testing?)
        Wire.Connect(board.Gnd[2], xorGate.PinPositions[7], "GND", board, "G");

        // Inputs to AND gate
        Wire.Connect(board.Vcc[2], andGate.PinPositions[1], "B", board, "I");
        Wire.Connect(board.Vcc[4], andGate.PinPositions[2], "C", board, "I");

        // Connect AND output to XOR input
        Wire.Connect(andGate.PinPositions[3], xorGate.PinPositions[1], "BC", board, "G2G");

        // Second XOR input
        Wire.Connect(board.Vcc[6], xorGate.PinPositions[2], "A", board, "I");

        // Build adjacency lists AFTER all wires are connected
        andGate.BuildAdjacencyList(board);
        xorGate.BuildAdjacencyList(board);
        board.BuildAdjacencyList();

        // Display breadboard and connections
        board.Display();
        board.DisplayGateConnections();
        // Optional if implemented
    }
}