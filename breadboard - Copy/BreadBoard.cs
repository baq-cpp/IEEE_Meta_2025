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

    private void InitializeGrid()
    {
        grid = new string[Rows, columns];
        Vcc.Clear();
        Gnd.Clear();

        int half = columns / 2;

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                bool inPowerRow = i > 6 && i < 56;
                bool isVccCol = j == 1 || j == half + 1;
                bool isGndCol = j == 0 || j == columns - 2;
                bool isGap = j == half / 2 || j == half + (half / 2);

                if (isVccCol && inPowerRow)
                {
                    grid[i, j] = "Vcc";
                    Vcc.Add((i, j));
                }
                else if (isGndCol && inPowerRow)
                {
                    grid[i, j] = "GND";
                    Gnd.Add((i, j));
                }
                else if (isGap)
                {
                    grid[i, j] = " ";
                }
                else
                {
                    grid[i, j] = "[] ";
                }
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
            foreach (var (pin, neighbors) in component.AdjacencyList)
            {
                if (!AdjacencyList.ContainsKey(component))
                    AdjacencyList[component] = new List<Component>();

                foreach (var neighborPos in neighbors)
                {
                    if (componentsGrid.TryGetValue(neighborPos, out var neighborComp))
                    {
                        if (!AdjacencyList[component].Contains(neighborComp))
                            AdjacencyList[component].Add(neighborComp);
                    }
                }
            }
        }

        foreach (var (a, b) in logicalConnections)
        {
            if (componentsGrid.TryGetValue(a, out var compA) && componentsGrid.TryGetValue(b, out var compB))
            {
                if (!AdjacencyList.ContainsKey(compA))
                    AdjacencyList[compA] = new List<Component>();
                if (!AdjacencyList.ContainsKey(compB))
                    AdjacencyList[compB] = new List<Component>();

                if (!AdjacencyList[compA].Contains(compB))
                    AdjacencyList[compA].Add(compB);
                if (!AdjacencyList[compB].Contains(compA))
                    AdjacencyList[compB].Add(compA);
            }
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
        Console.WriteLine("Gate Pin Connections:\n");

        foreach (var gate in registeredComponents)
        {
            string label = gate.Label;

            foreach (var (pinNum, pos) in gate.PinPositions)
            {
                // Power: show if gate logic has detected Vcc/GND
                if (pinNum == 14 && gate.HasVcc)
                    Console.WriteLine($"{label} pin 14 <--> Vcc");
                if (pinNum == 7 && gate.HasGnd)
                    Console.WriteLine($"{label} pin 7 <--> GND");

                // Gate-to-Gate connections
                if (!componentsGrid.TryGetValue(pos, out var thisComp)) continue;
                if (!AdjacencyList.TryGetValue(thisComp, out var neighbors)) continue;

                foreach (var neighborComp in neighbors)
                {
                    if (neighborComp == gate) continue;

                    foreach (var (otherPin, otherPos) in neighborComp.PinPositions)
                    {
                        if (pos == otherPos)
                        {
                            Console.WriteLine($"{label} pin {pinNum} <--> {neighborComp.Label} pin {otherPin}");
                        }
                    }
                }
            }
        }
    }

    private bool IsValidPosition(int row, int col)
        => row >= 0 && row < Rows && col >= 0 && col < Columns;

    public string GetGridCell(int row, int col) => grid[row, col];
}
