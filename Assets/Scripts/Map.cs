using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private Grid grid;
    private bool solve = false;

    public GameObject blackSquare;
    public GameObject whiteSquare;
    public GameObject greySquare;
    public GameObject startSquare;
    public GameObject endSquare;

    private void Start()
    {
        grid = new Grid();
        grid.blackSquare = blackSquare;
        grid.whiteSquare = whiteSquare;
        grid.greySquare = greySquare;
        grid.startSquare = startSquare;
        grid.endSquare = endSquare;
        grid.UpdateGrid(20, 20, 5f, new Vector3(-70, -75));



       
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.startPos = grid.SetValue(GetMouseWorldPos(), 3);
            Debug.Log(grid.startPos);
        }

        if (Input.GetMouseButtonDown(2))
        {
            var temp = grid.SetValue(GetMouseWorldPos(), 1);
            Debug.Log(temp);
        }

        if (Input.GetMouseButtonDown(1))
        {
            grid.endPos = grid.SetValue(GetMouseWorldPos(), 4);
            Debug.Log(grid.endPos);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            solve = true;
            grid.geneticAlgorithm.Run();
        }

        if (solve)
        {
            grid.Solve();
        }

    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        return worldPos;
    }

}
