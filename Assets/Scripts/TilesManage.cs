using UnityEngine;
using System.Collections.Generic;

public class TilesManage : MonoBehaviour
{

    public static TilesManage Instance;

    [HideInInspector] public GameObject _FirstSelected;
    [HideInInspector] public GameObject _SecondSelected;

    [SerializeField] private GameObject _linePrefab;

    [SerializeField]
    private Color[] _LineColors = new Color[]{
        Color.red, Color.blue,Color.green, new Color(0.5f,0f,1f)
    };

    private int _CurrentColorIndex = 0;

    public HashSet<(GameObject, GameObject)> Pins = new HashSet<(GameObject, GameObject)>();

    private Dictionary<(GameObject, GameObject), GameObject> _PinLines = new Dictionary<(GameObject, GameObject), GameObject>();/////////

    private Dictionary<GameObject, int> _SocketUsage = new Dictionary<GameObject, int>();////////////////////////////////

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TileSelected(GameObject SelectedTile)
    {
        if (_FirstSelected == null)
        {
            _FirstSelected = SelectedTile;
        }
        else if (_SecondSelected == null && SelectedTile != _FirstSelected)
        {
            _SecondSelected = SelectedTile;

            var pair = _FirstSelected.GetInstanceID() < _SecondSelected.GetInstanceID() ? (_FirstSelected, _SecondSelected) : (_SecondSelected, _FirstSelected);

            if (Pins.Contains(pair))
            {

                Pins.Remove(pair);
                //Debug.Log($"Removed pin:({_FirstSelected.name},{_SecondSelected.name})");

                if (_PinLines.TryGetValue(pair, out var lineObj))
                {
                    Destroy(lineObj);
                    _PinLines.Remove(pair);
                }

                DecrementSocketUsage(pair.Item1);
                DecrementSocketUsage(pair.Item2);


            }
            else
            {
                Pins.Add(pair);
                //Debug.Log($"New Pin Added: ({_FirstSelected.name},{_SecondSelected.name})");

                GameObject newline = Instantiate(_linePrefab);
                LineRenderer lineRenderer = newline.GetComponent<LineRenderer>();

                Vector3 _Offset = Vector3.up * 0.02f;//makes it float
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));//force new material created making each line different color

                Color _SolidColor = _LineColors[_CurrentColorIndex];//these two lines are in charge of geting different color per line
                _CurrentColorIndex = (_CurrentColorIndex + 1) % _LineColors.Length;

                lineRenderer.startColor = _SolidColor;
                lineRenderer.endColor = _SolidColor;

                lineRenderer.startWidth = 0.01f;//make wire appear more round & size
                lineRenderer.endWidth = 0.01f;
                lineRenderer.numCapVertices = 8;
                lineRenderer.numCornerVertices = 8;

                Vector3 start = _FirstSelected.transform.position + _Offset;
                Vector3 end = _SecondSelected.transform.position + _Offset;

                int _pointCount = 10; //smooth arc using many pionts
                lineRenderer.positionCount = _pointCount;

                for (int i = 0; i < _pointCount; i++)
                {
                    float t = i / (float)(_pointCount - 1);
                    Vector3 point = Vector3.Lerp(start, end, t);

                    float Height = -4f * Mathf.Pow(t - 0.5f, 2) + 1f;
                    float archheight = 0.02f;

                    point += Vector3.up * Height * archheight;
                    lineRenderer.SetPosition(i, point);
                }


                newline.transform.parent = null;

                _PinLines[pair] = newline;

                IncrementSocketUsage(pair.Item1);
                IncrementSocketUsage(pair.Item2);

            }
            _FirstSelected = null;
            _SecondSelected = null;

        }
        else
        {
            _FirstSelected = SelectedTile;
            _SecondSelected = null;


        }
        DisplayPins();
        
    }


public void IncrementSocketUsage(GameObject tile)//////////////////////////////
    {
        if (tile == null) return;

        if (_SocketUsage.ContainsKey(tile))
            _SocketUsage[tile]++;
        else
            _SocketUsage[tile] = 1;
    }

public void DecrementSocketUsage(GameObject tile)/////////////////////////////////////////
    {
        if (tile == null) return;

        if (_SocketUsage.ContainsKey(tile))
        {
            _SocketUsage[tile]--;
            if (_SocketUsage[tile] <= 0)
                _SocketUsage.Remove(tile);
        }
    }

    public bool IsTileSocket(GameObject tile)//////////////////////////////////////////
    {
        return _SocketUsage.ContainsKey(tile) && _SocketUsage[tile] > 0;
    }

    private void DisplayPins()
    {
        string output = "Pins: {";

        foreach (var pair in Pins)
        {
            //get names
            string nameA = pair.Item1 != null ? pair.Item1.name : "null";
            string nameB = pair.Item2 != null ? pair.Item2.name : "null";

            //get positions
            string posA = pair.Item1 != null ? $"({pair.Item1.transform.position.x},{pair.Item1.transform.position.z})" : "null";
            string posB = pair.Item2 != null ? $"({pair.Item2.transform.position.x},{pair.Item2.transform.position.z})" : "null";


            output += $"[{nameA}{posA},{nameB}{posB}]";

        }
        output += "  }";

        Debug.Log(output);

    }
}
