using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using breadboard;


namespace breadboard
{
    class Program
    {
        static void Main()
        {


            Breadboard board = new Breadboard(63, 28);

            // Place gates
            Component andGate = new Component(6,6, board, "74LS08");
            Component xorGate = new Component(19, 21, board, "XOR");
            Component notGate = new Component(19, 6, board, "NOT");

            // Connect power and ground for AND gate
            Wire.Connect(board.Vcc[0], andGate.PinPositions[14], "Vcc", board, "P");
            Wire.Connect(board.Gnd[0], andGate.PinPositions[7], "GND", board, "G");

            // Connect ground for XOR gate (Vcc intentionally left disconnected for testing?)
            Wire.Connect(board.Gnd[2], xorGate.PinPositions[7], "GND", board, "G");
            Wire.Connect(board.Gnd[4], xorGate.PinPositions[14], "VCC", board, "P");


            // Inputs to AND gate
            Wire.Connect(board.Vcc[2], andGate.PinPositions[1], "A", board, "I");
            Wire.Connect(board.Vcc[4], andGate.PinPositions[2], "B", board, "I");

            // Connect AND output to XOR input
            Wire.Connect(andGate.PinPositions[3], xorGate.PinPositions[1], "BC", board, "G2G");

            // Second XOR input
            Wire.Connect(board.Vcc[6], xorGate.PinPositions[2], "A", board, "I");

            // Build adjacency lists AFTER all wires are connected
            //andGate.CreateAdjacencyList(board);
            //xorGate.CreateAdjacencyList(board);
            board.BuildAdjacencyList();

            // Display breadboard and connections
            board.Display();
            board.DisplayGateConnections();

        }
    }
}
    
    
