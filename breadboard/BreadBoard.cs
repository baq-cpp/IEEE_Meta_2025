using System;
using System.Collections.Generic;

public class Breadboard
{
    public int Rows { get; }
    public List<(int row, int col)> Gnd { get; set; } = new List<(int, int)>();
    public List<(int row, int col)> vcc { get; set; } = new List<(int, int)>();
    private int columns;
    public int Columns
    {
        get => columns;
        set
        {
            columns = value;
            grid = new string[Rows, columns];
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if ((j == 1) || (j == columns - 1))
                    {
                        if (i > 6 && i < 56)
                        {
                            grid[i, j] = "Vcc"; // Vertical power rail
                            vcc.Add((i, j));
                        }
                    }
                    else if (j == columns-2 || j == 0)
                    {
                        if (i > 6 && i < 56)
                        {
                            grid[i, j] = "GND"; // Right power rail
                            Gnd.Add((i, j));
                        }
                        
                    }
                    else if ( j == (float)(columns / 2))
                    {
                        grid[i, j] = " "; // Middle column, usually empty
                    }
                    else
                    {
                        grid[i, j] = "[] "; // Open pin
                    }
                }
            }
            
        }
    }

    private string[,] grid;
    private Dictionary<(int row, int col), string> components = new();
    private List<string> componentList = new();
    public IReadOnlyList<string> ComponentList => componentList.AsReadOnly();
    public Dictionary<(int row, int col), List<(int row, int col)>> AdjacencyList { get; private set; } = new();
    private List<((int row, int col), (int row, int col))> logicalConnections = new();

    public Breadboard(int rows, int columns)
    {
        Rows = rows;
        Columns = columns; // Uses the setter
    }

    public void PlaceComponent(int row, int col, string symbol)
    {
        if (row >= 0 && row < Rows && col >= 0 && col < Columns)
        {
            grid[row, col] = symbol;
            components[(row, col)] = symbol;
            if (symbol != "1" && !componentList.Contains(symbol))
                componentList.Add(symbol);
        }
    }

    // Build adjacency list for all placed components
    public void BuildAdjacencyList()
    {
        AdjacencyList.Clear();
        foreach (var pos in components.Keys)
        {
            var neighbors = new List<(int row, int col)>();
            var directions = new (int dr, int dc)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            foreach (var (dr, dc) in directions)
            {
                var neighbor = (pos.row + dr, pos.col + dc);
                if (components.ContainsKey(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
            AdjacencyList[pos] = neighbors;
        }

        // Add logical connections as adjacency
        foreach (var (a, b) in logicalConnections)
        {
            if (!AdjacencyList.ContainsKey(a))
                AdjacencyList[a] = new List<(int, int)>();
            if (!AdjacencyList.ContainsKey(b))
                AdjacencyList[b] = new List<(int, int)>();
            if (!AdjacencyList[a].Contains(b))
                AdjacencyList[a].Add(b);
            if (!AdjacencyList[b].Contains(a))
                AdjacencyList[b].Add(a);
        }
    }

    public void Display()
    {
        const int cellWidth = 4; // Set the width for each cell
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                Console.Write($"{grid[i, j],-cellWidth}");
            }
            Console.WriteLine();
        }
    }

    public bool IsOccupied(int row, int col)
    {
        return components.ContainsKey((row, col));
    }

    public void AddLogicalConnection((int row, int col) a, (int row, int col) b)
    {
        logicalConnections.Add((a, b));
    }

    public string GetComponentSymbol((int row, int col) pos)
    {
        return components.TryGetValue(pos, out var symbol) ? symbol : "None";
    }

    public (string component, int pin) GetComponentAndPin((int row, int col) pos)
    {
        // Check each gate for the pin mapping
        // For simplicity, you can keep a list of all gate instances in Breadboard and check each
        // For this example, let's assume you pass the gate instances to Program.cs and check there
        return ("Unknown", -1);
    }
}