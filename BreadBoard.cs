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
            for (int j = 0; j < Columns; j++)
            {
                if (j == 0 || j == columns - 1 || j == half)
                {
                    grid[i, j] = "GND";
                    Gnd.Add((i, j));
                }
                else if (j == 1 || j == 14 || j == columns)
                {
                    Vcc.Add((i, j));
                    grid[i, j] = "VCC";
                }
                else if (j == (int) Math.Ceiling(half / 2.0) || j == (int)Math.Ceiling(half + (half / 2.0)))
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

    //public void BuildAdjacencyList()
    //{
    //    AdjacencyList.Clear();

    //    foreach (var component in registeredComponents)
    //    {
    //        foreach (var (pin, neighbors) in component.AdjacencyList)
    //        {
    //            if (!AdjacencyList.ContainsKey(component))
    //                AdjacencyList[component] = new List<Component>();

    //            foreach (var neighborPos in neighbors)
    //            {
    //                if (componentsGrid.TryGetValue(neighborPos, out var neighborComp))
    //                {
    //                    if (!AdjacencyList[component].Contains(neighborComp))
    //                        AdjacencyList[component].Add(neighborComp);
    //                }
    //            }
    //        }
    //    }

    //    foreach (var (a, b) in logicalConnections)
    //    {
    //        if (componentsGrid.TryGetValue(a, out var compA) && componentsGrid.TryGetValue(b, out var compB))
    //        {
    //            if (!AdjacencyList.ContainsKey(compA))
    //                AdjacencyList[compA] = new List<Component>();
    //            if (!AdjacencyList.ContainsKey(compB))
    //                AdjacencyList[compB] = new List<Component>();

    //            if (!AdjacencyList[compA].Contains(compB))
    //                AdjacencyList[compA].Add(compB);
    //            if (!AdjacencyList[compB].Contains(compA))
    //                AdjacencyList[compB].Add(compA);
    //        }
    //    }
    //}

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
