using System;
using System.Collections.Generic;

namespace breadboard
{
    public class Breadboard
    {
        public int Rows { get; }
        public int Columns
        {
            get { return columns; }
            set
            {
                columns = value;
                InitializeGrid();
            }
        }

        private int columns;
        private string[,] grid;

        public List<(int row, int col)> Vcc { get; } = new List<(int row, int col)>();
        public List<(int row, int col)> Gnd { get; } = new List<(int row, int col)>();

        private Dictionary<(int row, int col), Component> componentsGrid =
            new Dictionary<(int row, int col), Component>();

        private List<Component> registeredComponents = new List<Component>();
        private List<((int row, int col), (int row, int col))> logicalConnections =
            new List<((int row, int col), (int row, int col))>();

        public Dictionary<Component, List<Component>> AdjacencyList { get; private set; } =
            new Dictionary<Component, List<Component>>();

        public Breadboard(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }

        private void InitializeGrid()
        {
            grid = new string[Rows, columns];
            Vcc.Clear();
            Gnd.Clear();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    string cell = "[]"; // default filler

                    // Power rails
                    if (j == 0 || j == 12 || j == 14 || j == 26) // VCC
                    {
                        cell = "VCC";
                        Vcc.Add((i, j));
                    }
                    else if (j == 1 || j == 13 || j == 15 || j == 27) // GND
                    {
                        cell = "GND";
                        Gnd.Add((i, j));
                    }

                    grid[i, j] = cell;
                }
            }
        }

        public void PlaceComponent(int row, int col, Component component)
        {
            if (IsValidPosition(row, col))
            {
                grid[row, col] = component.Label;
                componentsGrid[(row, col)] = component;

                if (!registeredComponents.Contains(component))
                    registeredComponents.Add(component);
            }
        }

        public bool IsOccupied(int row, int col)
        {
            return componentsGrid.ContainsKey((row, col));
        }

        public void AddLogicalConnection((int row, int col) a, (int row, int col) b)
        {
            logicalConnections.Add((a, b));
        }

        //public void BuildAdjacencyList()
        //{
        //    AdjacencyList.Clear();

        //    foreach (var component in registeredComponents)
        //    {
        //        var connectedComponents = new HashSet<Component>();

        //        foreach (var pinEntry in component.AdjacencyList)
        //        {
        //            foreach (var neighborComp in pinEntry.Value)
        //            {
        //                if (neighborComp != component)
        //                {
        //                    connectedComponents.Add(neighborComp);
        //                }
        //            }
        //        }

        //        foreach (var connection in logicalConnections)
        //        {
        //            var a = connection.Item1;
        //            var b = connection.Item2;

        //            Component compA, compB;

        //            if (componentsGrid.TryGetValue(a, out compA) &&
        //                componentsGrid.TryGetValue(b, out compB))
        //            {
        //                if (compA == component && compB != component)
        //                    connectedComponents.Add(compB);
        //                else if (compB == component && compA != component)
        //                    connectedComponents.Add(compA);
        //            }
        //        }

        //        if (connectedComponents.Count > 0)
        //            AdjacencyList[component] = new List<Component>(connectedComponents);
        //    }
        //}

        public void Display()
        {
            const int cellWidth = 4;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    Console.Write("{0,-4}", grid[i, j]);
                }
                Console.WriteLine();
            }
        }

        public void DisplayGateConnections()
        {
            Console.WriteLine("Component-to-Component Connections:\n");

            var seen = new HashSet<(Component, Component)>();

            foreach (var entry in AdjacencyList)
            {
                var component = entry.Key;
                var neighbors = entry.Value;

                foreach (var neighbor in neighbors)
                {
                    var pair = (component, neighbor);
                    var reversePair = (neighbor, component);

                    if (seen.Contains(reversePair)) continue;

                    Console.WriteLine("{0} <--> {1}", component.Label, neighbor.Label);
                    seen.Add(pair);
                }
            }
        }

        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < Rows && col >= 0 && col < Columns;
        }

        public string GetGridCell(int row, int col)
        {
            return grid[row, col];
        }

        public bool TryGetComponentAt((int row, int col) pos, out Component component)
        {
            return componentsGrid.TryGetValue(pos, out component);
        }

        //public void checkDuplicates()
        //{
        //    for (int i = 0; i < registeredComponents.Count; i++)
        //    {
        //        foreach (var j in registeredComponents)
        //        {
        //            if (i.Label == j.Label)
        //            {
        //                count++;
        //            }
        //        }
        //    }
        //}
        public void BuildAdjacencyList()
        {
            // 1) Make sure each component's per-pin adjacency is up to date
            foreach (var comp in registeredComponents)
            {
                comp.CreateAdjacencyList(this);
            }

            // 2) Merge into a board-level undirected graph
            AdjacencyList.Clear();

            foreach (var component in registeredComponents)
            {
                var connected = new HashSet<Component>();

                // From component-level pin adjacencies
                foreach (var kv in component.AdjacencyList)        // kv: pinIndex -> List<Component2>
                {
                    var list = kv.Value;
                    if (list == null) continue;
                    foreach (var n in list)
                    {
                        if (n != null && n != component)
                            connected.Add(n);
                    }
                }

                // From board-level logicalConnections (grid cell pairs)
                foreach (var connection in logicalConnections)
                {
                    var a = connection.Item1;
                    var b = connection.Item2;

                    if (componentsGrid.TryGetValue(a, out var compA) &&
                        componentsGrid.TryGetValue(b, out var compB))
                    {
                        if (ReferenceEquals(compA, component) && !ReferenceEquals(compB, component))
                            connected.Add(compB);
                        else if (ReferenceEquals(compB, component) && !ReferenceEquals(compA, component))
                            connected.Add(compA);
                    }
                }

                if (connected.Count > 0)
                    AdjacencyList[component] = new List<Component>(connected);
            }
        }
    }

}
