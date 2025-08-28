using System;

namespace breadboard
{
    internal static class Wire
    {
        private static int wireCounter = 0; // ensures each wire label is unique

        public static void Connect((int row, int col) a, (int row, int col) b, string ID, Breadboard board, string type)
        {
            // Create a unique label for each wire instance
            var wire = new Component(ID);

            switch (type)
            {
                case "I": // Input wire
                case "G": // GND wire
                    {
                        board.PlaceComponent(a.row, a.col, wire);

                        board.PlaceComponent(b.row, b.col - 1, wire);


                        board.AddLogicalConnection(a, b);
                        break;
                    }

                case "P": // Power wire
                    {
                        board.PlaceComponent(a.row, a.col, wire);
                        board.PlaceComponent(b.row, b.col + 1, wire);

                        board.AddLogicalConnection(a, b);
                        break;
                    }

                case "G2G": // Gate-to-Gate wire
                    {
                        // Place wires offset from each pin
                        //int colA = a.col < board.Columns / 2 ? a.col + 1 : a.col - 1;
                        //int colB = b.col < board.Columns / 2 ? b.col + 1 : b.col - 1;

                        board.PlaceComponent(a.row, a.col - 1, wire);
                        board.PlaceComponent(b.row, b.col - 1, wire);

                        board.AddLogicalConnection(a, b);
                        break;
                    }

                default:
                    throw new ArgumentException($"Unknown wire type: {type}");
            }
        }


     }

}