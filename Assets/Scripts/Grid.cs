using System;
using System.Linq;
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
    public GameObject greySquare;
    public GameObject startSquare;
    public GameObject endSquare;
    public Vector2 startPos;
    public Vector2 endPos;
    public GeneticAlgorithm geneticAlgorithm;
    private List<GameObject> pathSquares;


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
                    CreateSquares(i, j, whiteSquare);
                }

                debugTextArray[i, j] = CreateWorldText(null, gridArray[i, j].ToString(), GetWorldPos(i, j) + new Vector3(cellSize, cellSize) * 0.5f, 20, Color.green, TextAnchor.MiddleCenter, TextAlignment.Left, 5000);
                Debug.DrawLine(GetWorldPos(i, j), GetWorldPos(i, j + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPos(i, j), GetWorldPos(i + 1, j), Color.white, 100f);          
            }
        }

        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);

        pathSquares = new List<GameObject>();

        geneticAlgorithm = new GeneticAlgorithm
        {
            grid = this
        };

    }

    public GameObject GetCorrectSquare(int tile)
    {
        if (tile == 0)
        {
            return whiteSquare;
        }
        else if (tile == 1)
        {
            return blackSquare;
        }
        else if (tile == 2)
        {
            return greySquare;
        }
        else if (tile == 3)
        {
            return startSquare;
        }
        else if (tile == 4)
        {
            return endSquare;
        }
        else
        {
            return null;
        }
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

            Destroy(squares[x, y]);
            CreateSquares(x, y, GetCorrectSquare(value));
        }
    }

    private void CreateSquares(int x, int y, GameObject go)
    {
        squares[x, y] = Instantiate(go);
        squares[x, y].transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
        squares[x, y].transform.localScale = new Vector3(cellSize, cellSize) * 0.75f;
    }

    private void CreatePathSquares(int x, int y, GameObject go)
    {
        GameObject pathSquare = Instantiate(go);
        pathSquare.transform.position = GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * 0.5f;
        pathSquare.transform.localScale = new Vector3(cellSize, cellSize) * 0.75f;
        pathSquares.Add(pathSquare);
    }

    public Vector2Int SetValue(Vector3 worldPos, int value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetValue(x, y, value);
        return new Vector2Int(x, y);
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

    public Vector2 Move(Vector2 position, int direction)
    {
        switch (direction)
        {
            case 0: // North
                if (position.y - 1 < 0 || gridArray[(int)(position.y - 1), (int)position.x] == 1)
                {
                    break;
                }
                else
                {
                    position.y -= 1;
                }
                break;
            case 1: // South
                if (position.y + 1 >= gridArray.GetLength(0) || gridArray[(int)(position.y + 1), (int)position.x] == 1)
                {
                    break;
                }
                else
                {
                    position.y += 1;
                }
                break;
            case 2: // East
                if (position.x + 1 >= gridArray.GetLength(1) || gridArray[(int)position.y, (int)(position.x + 1)] == 1)
                {
                    break;
                }
                else
                {
                    position.x += 1;
                }
                break;
            case 3: // West
                if (position.x - 1 < 0 || gridArray[(int)position.y, (int)(position.x - 1)] == 1)
                {
                    break;
                }
                else
                {
                    position.x -= 1;
                }
                break;
        }
        return position;
    }

    public void Solve()
    {
        if(geneticAlgorithm.busy)
        {
            geneticAlgorithm.Epoch();
        }
        DrawPath();

        //int x, y;
        //GetXY(pathSquares.Last().transform.position, out x, out y);
        //Debug.Log("Generation: " + geneticAlgorithm.generation + " (" + x + "," + y + ")");
    }

    private void DrawPath()
    {
        ClearPath();
        //Debug.Log("path cleared");
        Genome bestGenome = geneticAlgorithm.genomes[geneticAlgorithm.fittestGenome];
        List<int> bestDirection = geneticAlgorithm.Decode(bestGenome.bits);
        Vector2 pos = startPos;

        foreach(var direction in bestDirection)
        {
            pos = Move(pos, direction);
            CreatePathSquares((int)pos.x, (int)pos.y, GetCorrectSquare(2)); // Grey.
        }

    }

    private void ClearPath()
    {
        foreach (GameObject pathTile in pathSquares)
        {
            Destroy(pathTile);
        }
        pathSquares.Clear();
    }

    public double Fitness(List<int> directions)
    {
        Vector2 pos = startPos;

        for(int idx = 0; idx < directions.Count; idx++)
        {
            var nextDirection = directions[idx];
            pos = Move(pos, nextDirection);
        }

        Vector2 absPos = new Vector2( Math.Abs(pos.x - endPos.x), Math.Abs(pos.y - endPos.y));
        double result = 1 / (double)(absPos.x + absPos.y + 1);

        if (result == 1)
        {
            Debug.Log("TestRoute result=" + result + ",(" + pos.x + "," + pos.y + ")");
        }

        return result;
    }

}
