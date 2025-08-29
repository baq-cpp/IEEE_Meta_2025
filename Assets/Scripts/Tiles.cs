using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public enum BreadBoardSections
    {
        Powerrailtop,
        powerRailBottom,
        TerminalLeft,
        TerminalRight,
        Gap
    }
public class Tiles : MonoBehaviour
{

    private bool isselected = false;
   // [SerializeField] private Color _baseColor, _OffsetColor; //creates assets in the inspector the different colors/materials
    [SerializeField] private Renderer _renderer; //creates assets in the inspector what will color be added to

    [SerializeField] private GameObject _highlight;//creates assets in the inspector

    [SerializeField] private GameObject _Selected; // creates object in inspector for select highlight

    private List<GameObject> _coordinates = new List<GameObject>();

    public int Row { get; private set; }
    public int Col { get; private set; }


    public void Init(bool isOffset)
    {
        // _renderer.material.color = isOffset ? _OffsetColor : _baseColor; // makes the spheres alternate colors
    }

    public void OnHoverEntered(HoverEnterEventArgs args) // makes white box appear when hovering
    {
        //Debug.Log("Hover Entered"); //used to test if method is firing
        _highlight.SetActive(true);

    }

    public void OnHoverExited(HoverExitEventArgs args)// makes white box dissapper after leaving hover area
    {
        //Debug.Log("Hover Exited"); //used to test if method is firing
        _highlight.SetActive(false);

    }

    public void OnSelectEntered(SelectEnterEventArgs args)  //will set selected to red
    {

        isselected = !isselected;

        _Selected.SetActive(isselected);

        if (isselected)
        {
            TilesManage.Instance.TileSelected(gameObject);
        }


        //_highlight.SetActive(false); //will deactivate the hover effect if item is selected

       // TilesManage.Instance.TileSelected(gameObject);



    }

    public void OnSelectExited(SelectExitEventArgs args)  //will take away selected effect
    {
       // _Selected.SetActive(false);

    }

    public void OnActivate(ActivateEventArgs args)
    {
        _Selected.SetActive(true);
    }

    void DisplayPins(HashSet<(GameObject, GameObject)> collection)
    {

        string output = "{";
        foreach (var pair in collection)
        {
            string nameA = pair.Item1 != null ? $"({pair.Item1.transform.position.x},{pair.Item1.transform.position.y})" : "null";
            string nameB = pair.Item2 != null ? $"({pair.Item2.transform.position.x},{pair.Item2.transform.position.y})" : "null";
            output += $"({nameA},{nameB}) ";
        }
        output += " }";

        Debug.Log(output);
    }
    
    public void SetCoords(int row, int col)
    {
        Row = row;
        Col = col;
    }
}
