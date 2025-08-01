class Program
{
    static void Main()
    {
        var board = new Breadboard(63, 15);
        var andGate = new AndGateIC(2, board);
        var xorGate = new XorGateIC(11, board);
        Wire.Connect(board.vcc[0], (2,2),"A", board);
        //Wire.Connect(andGate.PinPositions[2], xorGate.PinPositions[2], board);
        Wire.Connect(board.vcc[1], andGate.PinPositions[2],"B", board);



        board.Display();
        board.BuildAdjacencyList();

        // For each gate, for each pin, check adjacency (excluding self-connections and same-gate connections)
        var gates = new[] { ("AND", andGate.PinPositions), ("XOR", xorGate.PinPositions) };
        foreach (var (name, pins) in gates)
        {
            foreach (var pinKvp in pins)
            {
                var pinNum = pinKvp.Key;
                var pos = pinKvp.Value;
                if (board.AdjacencyList.TryGetValue(pos, out var neighbors))
                {
                    foreach (var neighbor in neighbors)
                    {
                        foreach (var (otherName, otherPins) in gates)
                        {
                            var otherPin = otherPins.FirstOrDefault(p => p.Value == neighbor);
                            // Exclude self-connections and same-gate connections
                            if (!otherPin.Equals(default(KeyValuePair<int, (int, int)>)) &&
                                name != otherName) // Only connect pins from different gates
                            {
                                Console.WriteLine($"{name} pin {pinNum} is connected to: {otherName} pin {otherPin.Key}");
                            }
                        }
                    }
                }
            }
        }
    }
}