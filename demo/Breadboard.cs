using System;
using System.Collections.Generic;

public class Breadboard
{
    public int Rows { get; }
    public int Columns
    {
        get => columns;
        set
        {
            columns = value;
            InitializeGrid();
        }
    }

    private int columns;
    private string[,] grid;

    public List<(int row, int col)> Vcc { get; } = new();
    public List<(int row, int col)> Gnd { get; } = new();

    private Dictionary<(int row, int col), Component> componentsGrid = new();
    private List<Component> registeredComponents = new();
    private List<((int row, int col), (int row, int col))> logicalConnections = new();

    public Dictionary<Component, List<Component>> AdjacencyList { get; private set; } = new();

    public Breadboard(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
    }

    //private void InitializeGrid()
    //{
    //    grid = new string[Rows, columns];
    //    Vcc.Clear();
    //    Gnd.Clear();

    //    int half = columns / 2;
    //    for (int i = 0; i < Rows; i++)
    //    {
    //        for (int j = 0; j < columns; j++)
    //        {
    //            if (j == 0 || j == columns - 2 || j == half)
    //            {
    //                grid[i, j] = "GND";
    //                Gnd.Add((i, j));
    //            }
    //            else if (j == 1 || j == half -1 || j == columns -1)
    //            {
    //                Vcc.Add((i, j));
    //                grid[i, j] = "VCC";
    //            }
    //            else if (j == (int)Math.Ceiling(half / 2.0) || j == (int)Math.Ceiling(half + (half / 2.0)))
    //            {
    //                grid[i, j] = " ";
    //            }
    //            else
    //            {
    //                grid[i, j] = "[] ";
    //            }
    //        }
    //    }
    //}
    private void InitializeGrid()
    {
        grid = new string[Rows, columns];
        Vcc.Clear();
        Gnd.Clear();

        int half = columns / 2;                // expect 14 when columns == 28
        int leftGND = 0, leftVCC = 1;
        int midGND = half, midVCC = half - 1;
        int rightGND = columns - 2, rightVCC = columns - 1;

        // For the exact pattern: 5 brackets, gap, 5 brackets inside each half
        // Left half indices (0..half):
        // [] blocks: 2..6, gap at 7, 8..12, then VCC(13), GND(14)
        int leftFirstBlockStart = 2;
        int leftFirstBlockEnd = leftFirstBlockStart + 5 - 1; // 6
        int leftGap = leftFirstBlockEnd + 1;       // 7
        int leftSecondBlockStart = leftGap + 1;                 // 8
        int leftSecondBlockEnd = leftSecondBlockStart + 5 - 1;// 12

        // Right half indices (half..columns-1):
        // GND(half), then [] 5x, gap, [] 5x, then GND/VCC at the very end
        int rightFirstBlockStart = half + 1;                  // 15
        int rightFirstBlockEnd = rightFirstBlockStart + 5 - 1; // 19
        int rightGap = rightFirstBlockEnd + 1;       // 20
        int rightSecondBlockStart = rightGap + 1;                 // 21
        int rightSecondBlockEnd = rightSecondBlockStart + 5 - 1;// 25

        bool exactPatternPossible =
            half >= 14 && columns >= 28 &&
            leftSecondBlockEnd < midVCC &&
            rightSecondBlockEnd < rightGND;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Defaults
                string cell = "[]";

                // Exact pattern (preferred)
                if (exactPatternPossible)
                {
                    // Power rails
                    if (j == leftGND || j == midGND || j == rightGND)
                    {
                        cell = "GND";
                        Gnd.Add((i, j));
                    }
                    else if (j == leftVCC || j == midVCC || j == rightVCC)
                    {
                        cell = "VCC";
                        Vcc.Add((i, j));
                    }
                    // Gaps
                    else if (j == leftGap || j == rightGap)
                    {
                        cell = " ";
                    }
                    // Bracket zones (implicitly [])
                    else if ((j >= leftFirstBlockStart && j <= leftFirstBlockEnd) ||
                             (j >= leftSecondBlockStart && j <= leftSecondBlockEnd) ||
                             (j >= rightFirstBlockStart && j <= rightFirstBlockEnd) ||
                             (j >= rightSecondBlockStart && j <= rightSecondBlockEnd))
                    {
                        cell = "[]";
                    }
                }
                else
                {
                    // Fallback: general layout if someone changes 'columns'
                    if (j == 0 || j == half || j == columns - 2)
                    {
                        cell = "GND";
                        Gnd.Add((i, j));
                    }
                    else if (j == 1 || j == half - 1 || j == columns - 1)
                    {
                        cell = "VCC";
                        Vcc.Add((i, j));
                    }
                    else if (j == (int)Math.Ceiling(half / 2.0) ||
                             j == half + (int)Math.Ceiling(half / 2.0))
                    {
                        cell = " ";
                    }
                    else
                    {
                        cell = "[]";
                    }
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

    public bool IsOccupied(int row, int col) => componentsGrid.ContainsKey((row, col));

    public void AddLogicalConnection((int row, int col) a, (int row, int col) b)
    {
        logicalConnections.Add((a, b));
    }
    public void BuildAdjacencyList()
    {
        AdjacencyList.Clear();

        foreach (var component in registeredComponents)
        {
            var connectedComponents = new HashSet<Component>();

            foreach (var (pinNumber, neighborComponents) in component.AdjacencyList)
            {
                foreach (var neighborComp in neighborComponents)
                {
                    if (neighborComp != component)
                    {
                        connectedComponents.Add(neighborComp);
                    }
                }
            }

            foreach (var (a, b) in logicalConnections)
            {
                if (componentsGrid.TryGetValue(a, out var compA) &&
                    componentsGrid.TryGetValue(b, out var compB))
                {
                    if (compA == component && compB != component)
                        connectedComponents.Add(compB);
                    else if (compB == component && compA != component)
                        connectedComponents.Add(compA);
                }
            }

            if (connectedComponents.Count > 0)
                AdjacencyList[component] = new List<Component>(connectedComponents);
        }
    }

    public void Display()
    {
        const int cellWidth = 4;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Console.Write($"{grid[i, j],-cellWidth}");
            }
            Console.WriteLine();
        }
    }

    public void DisplayGateConnections()
    {
        Console.WriteLine("Component-to-Component Connections:\n");

        var seen = new HashSet<(Component, Component)>();

        foreach (var (component, neighbors) in AdjacencyList)
        {
            foreach (var neighbor in neighbors)
            {
                // Avoid duplicates: only print if not already seen in reverse
                var pair = (component, neighbor);
                var reversePair = (neighbor, component);

                if (seen.Contains(reversePair)) continue;

                Console.WriteLine($"{component.Label} <--> {neighbor.Label}");
                seen.Add(pair);
            }
        }
    }


    private bool IsValidPosition(int row, int col)
        => row >= 0 && row < Rows && col >= 0 && col < Columns;

    public string GetGridCell(int row, int col) => grid[row, col];

    public bool TryGetComponentAt((int row, int col) pos, out Component component)
    {
        return componentsGrid.TryGetValue(pos, out component);
    }

}