//namespace breadboard
//{
//    public static class Gate
//    {
//        public static Component CreateGate(string gateType, int row, Breadboard board)
//        {
//            return gateType switch
//            {
//                "AND" => new Component(row, board, "AND", 14),
//                "OR" => new Component(row, board, "OR", 14),
//                "NOT" => new Component(row, board, "NOT", 14),
//                "NAND" => new Component(row, board, "NAND", 14),
//                "NOR" => new Component(row, board, "NOR", 14),
//                "XOR" => new Component(row, board, "XOR", 14),
//                _ => throw new ArgumentException($"Unknown gate type: {gateType}")
//            };
//        }
//    }
//}

