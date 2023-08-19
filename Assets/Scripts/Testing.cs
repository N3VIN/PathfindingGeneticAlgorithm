using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid grid;

    public GameObject blackSquare;
    public GameObject whiteSquare;
    private void Start()
    {
        grid = new Grid();
        grid.blackSquare = blackSquare;
        grid.whiteSquare = whiteSquare;
        grid.UpdateGrid(20, 20, 5f, new Vector3(-70, -75));

        //grid = new Grid(4, 2, 10f, new Vector3(-50, -50));

       
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            grid.SetValue(GetMouseWorldPos(), 1);
        }

        if (Input.GetMouseButtonDown(2))
        {
            grid.SetValue(GetMouseWorldPos(), 0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(GetMouseWorldPos()));
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0f;
        return worldPos;
    }

}
