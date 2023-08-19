using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int width;
    private int height;
    private float cellSize;
    private Vector3 originPos;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;
    private GameObject[,] squares;

    public GameObject blackSquare;
    public GameObject whiteSquare;

   public void UpdateGrid(int width, int height, float cellSize, Vector3 originPos)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos;

        gridArray = new int[width, height];
        debugTextArray = new TextMesh[width, height];
        squares = new GameObject[width, height];

        for(int i=0; i < gridArray.GetLength(0); i++)
        {
            for(int j=0; j < gridArray.GetLength(1); j++)
            {
                if (whiteSquare != null)
                {
                    //squares[i, j] = Instantiate(whiteSquare);
                    //squares[i, j].transform.position = GetWorldPos(i, j) + new Vector3(cellSize, cellSize) * 0.5f;
                    //squares[i, j].transform.localScale = new Vector3(cellSize, cellSize) * 0.5f;
                    CreateSquares(i, j, whiteSquare);
                }

                debugTextArray[i, j] = CreateWorldText(null, gridArray[i, j].ToString(), GetWorldPos(i, j) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.green, TextAnchor.MiddleCenter, TextAlignment.Left, 5000);
                Debug.DrawLine(GetWorldPos(i, j), GetWorldPos(i, j + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPos(i, j), GetWorldPos(i + 1, j), Color.white, 100f);          
            }
        }

        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);

    }

    private Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPos;
    }

    private void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - originPos).x / cellSize);
        y = Mathf.FloorToInt((worldPos - originPos).y / cellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = gridArray[x, y].ToString();
            if (value == 1)
            {
                Destroy(squares[x, y]);
                CreateSquares(x, y, blackSquare);
            }
            else if(value == 0)
            {
                Destroy(squares[x, y]);
                CreateSquares(x, y, whiteSquare);
            }
        }
    }

    private void CreateSquares(int x, int y, GameObject go)
    {
        squares[x, y] = Instantiate(go);
        squares[x, y].transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
        squares[x, y].transform.localScale = new Vector3(cellSize, cellSize) * 0.75f;
    }

    public void SetValue(Vector3 worldPos, int value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetValue(x, y, value);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return gridArray[x, y];
        }
        else
        {
            return -1;
        }
    }

    public int GetValue(Vector3 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetValue(x, y);
    }

    private static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPos, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameobject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameobject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPos;
        TextMesh textMesh = gameobject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

}
