using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridManage : MonoBehaviour
{
    [SerializeField] public int _width, _height, _RailsHeight,_RailsWidth; //creates assets in the inspector

    [SerializeField] private GameObject _spherePrefab;//creates asset in the inspector

    [SerializeField] private Vector3 _gridOffset = Vector3.zero;//change to start grid at set corrdinates

    [SerializeField] private Vector3 _RailOffset = Vector3.zero;//change to start rails at set coordinates
    

         



    void Start()
    {
        generateGrid();// spawns grid

        

    }

    void generateGrid()
    {

        float RowGap = 0.0422f;
        

        for (int x = 0; x < _width; x++)//loop for width
        {

            for (int y = 0; y < _height; y++)//loop for height 
            {
                float TerminalSpacing = 0.02286f;
                float HeightSpacing = 0.0235f;
                float BridgeSpace = 0.211f;


                float Ypos = y * TerminalSpacing;
                float zPos = x * HeightSpacing;



                if (x >= 5) zPos += RowGap;

                if (x >= 10) zPos += BridgeSpace;
                if (x >= 15) zPos += RowGap;


                Vector3 spawnPos = new Vector3(Ypos, 0.0f, zPos)+_gridOffset;
                var sphere = Instantiate(_spherePrefab, spawnPos, Quaternion.identity, transform);
                sphere.name = $"{y},{x}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                Tiles tiles = sphere.GetComponent<Tiles>();
                tiles.Init(isOffset);




            }
        }




        for (int x = 0; x < _RailsWidth; x++)
        {
            for (int y = 0; y < _RailsHeight; y++)
            {

                float TerminalSpacing = 0.02286f;
                float HeightSpacing = 0.0235f;

                float zPos = x * TerminalSpacing;

                float Ypos = y* HeightSpacing;

                float CenterRail = 0.36321f;
                float SpaceCenter = 0.0332f;

                if (x >= 2) zPos += CenterRail;
                if (x >= 4) zPos += SpaceCenter;
                if (x >= 6) zPos += CenterRail;



                int groupCount = y / 5;
                if (groupCount > 0 && groupCount <= 50)
                {
                    Ypos += groupCount * 0.0188f; // adjust gap per group 0.02700 0.04986
                }


                Vector3 spawnPos = new Vector3(Ypos, 0.0f, zPos)+_RailOffset;
                var sphere = Instantiate(_spherePrefab, spawnPos, Quaternion.identity, transform);
                sphere.name = $"{y},{x}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                Tiles tiles = sphere.GetComponent<Tiles>();
                tiles.Init(isOffset);
            }
        }

    


       
    }

    
}
