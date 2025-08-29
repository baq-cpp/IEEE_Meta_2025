using System;
using System.Collections.Generic;
using UnityEngine;

public class Breadboard{

    private static Breadboard _instance;
    public static Breadboard Instance => _instance ??= new Breadboard();
    public int Rows { get; private set; } = 63;
    public int Columns { get; private set; } = 28;
    public bool IsInitialized { get; private set; }

    private string[,] grid;

    public List<(int row, int col)> Vcc { get; } = new();
    public List<(int row, int col)> Gnd { get; } = new();

    private readonly Dictionary<(int row, int col), Component2> componentsGrid = new();
    private readonly List<Component2> registeredComponents = new();
    private readonly List<((int row, int col), (int row, int col))> logicalConnections = new();

    public Dictionary<Component2, List<Component2>> AdjacencyList { get; private set; } = new();

    // Private constructor — only Instance can create
    private Breadboard()
    {
        InitializeGrid();
    }

    

    // public Breadboard(int rows, int columns)
    // {
    //     Rows = rows;
    //     Columns = columns;
    // }
        private void InitializeGrid()
    {
        grid = new string[Rows, Columns];
        Vcc.Clear();
        Gnd.Clear();

        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
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
        IsInitialized = true;
    }

    public void PlaceComponent(int row, int col, Component2 component)
    {
         if (!IsInitialized)
        {
            Debug.LogError("[Breadboard] PlaceComponent called before initialization.");
            return;
        }
        if (IsValidPosition(row, col))
        {
            grid[row, col] = component.Label;
            componentsGrid[(row, col)] = component;

            if (!registeredComponents.Contains(component))
                registeredComponents.Add(component);
        }
    }

    public bool UnplaceAt(int row, int col)
    {
        if (!IsInitialized) return false;
        if (!IsValidPosition(row, col)) return false;

        // If nothing there, nothing to do
        if (!componentsGrid.Remove((row, col))) return false;

        // Reset the visible text
        if (Vcc.Contains((row, col))) grid[row, col] = "VCC";
        else if (Gnd.Contains((row, col))) grid[row, col] = "GND";
        else grid[row, col] = "[]";

        return true;
    }

    public int UnplaceComponent(Component2 component)
    {
        if (!IsInitialized || component == null) return 0;

        var toRemove = new List<(int, int)>();
        foreach (var kv in componentsGrid)
            if (ReferenceEquals(kv.Value, component))
                toRemove.Add(kv.Key);

        int count = 0;
        foreach (var (r, c) in toRemove)
            if (UnplaceAt(r, c)) count++;

        return count;
    }


    public bool IsOccupied(int row, int col)
    {
        return componentsGrid.ContainsKey((row, col));
    }

    public void AddLogicalConnection((int row, int col) a, (int row, int col) b)
    {
        logicalConnections.Add((a, b));
    }

    public Dictionary<Component2, List<Component2>> BuildAdjacencyList()
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
            var connected = new HashSet<Component2>();

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
                AdjacencyList[component] = new List<Component2>(connected);
        }
                return AdjacencyList;
        
     }

    public void Display()
    {
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
        Debug.Log("Component-to-Component Connections:\n");

        var seen = new HashSet<(Component2, Component2)>();

        foreach (var entry in AdjacencyList)
        {
            var component = entry.Key;
            var neighbors = entry.Value;

            foreach (var neighbor in neighbors)
            {
                var pair = (component, neighbor);
                var reversePair = (neighbor, component);

                if (seen.Contains(reversePair)) continue;

                Debug.Log($"{component.Label} <--> {neighbor.Label}");
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

    public bool TryGetComponentAt((int row, int col) pos, out Component2 component)
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // private void Awake()
    // {
    //     if (Instance != null && Instance != this)
    //     {
    //         Debug.LogWarning("[Breadboard] Duplicate instance; destroying this one.");
    //         Destroy(gameObject);
    //         return;
    //     }

    //     Instance = this;

    //     // Initialize once here so the board is ready before user interactions.
    //     InitializeGrid();
    
    
}
