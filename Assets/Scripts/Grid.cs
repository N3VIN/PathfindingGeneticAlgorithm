using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private int m_Width;
    private int m_Height;
    private float m_CellSize;
    private Vector2 m_OriginPos;
    private int[,] m_GridArray;
    private TextMesh[,] m_DebugTextArray;
    private GameObject[,] m_Squares;
    private List<GameObject> m_PathSquares;

    public GameObject m_BlackSquare;
    public GameObject m_WhiteSquare;
    public GameObject m_GreySquare;
    public GameObject m_StartSquare;
    public GameObject m_EndSquare;
    public Vector2 m_StartPos;
    public Vector2 m_EndPos;
    public GeneticAlgorithm m_GeneticAlgorithm;

    private void PrintGrid()
    {
        for (int i = 0; i < m_GridArray.GetLength(0); i++)
        {
            for (int j = 0; j < m_GridArray.GetLength(1); j++)
            {
                Debug.Log(m_GridArray[i, j] + " ");
            }
        }
    }

    public void UpdateGrid(int width, int height, float cellSize, Vector2 originPos)
    {
        this.m_Width = width;
        this.m_Height = height;
        this.m_CellSize = cellSize;
        this.m_OriginPos = originPos;

        m_GridArray = new int[width, height];
        m_DebugTextArray = new TextMesh[width, height];
        m_Squares = new GameObject[width, height];

        for(int i=0; i < m_GridArray.GetLength(0); i++)
        {
            for(int j=0; j < m_GridArray.GetLength(1); j++)
            {
                if (m_WhiteSquare != null)
                {
                    CreateSquares(i, j, m_WhiteSquare);
                }

                m_DebugTextArray[i, j] = CreateWorldText(null, m_GridArray[i, j].ToString(), GetWorldPos(i, j) + new Vector2(cellSize, cellSize) * 0.5f, 20, Color.green, TextAnchor.MiddleCenter, TextAlignment.Left, 5000);
                Debug.DrawLine(GetWorldPos(i, j), GetWorldPos(i, j + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPos(i, j), GetWorldPos(i + 1, j), Color.white, 100f);          
            }
        }

        Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f);
        Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);

        m_PathSquares = new List<GameObject>();

        m_GeneticAlgorithm = new GeneticAlgorithm
        {
            m_Grid = this
        };

    }

    private GameObject GetCorrectSquare(int tile)
    {
        if (tile == 0)
        {
            return m_WhiteSquare;
        }
        else if (tile == 1)
        {
            return m_BlackSquare;
        }
        else if (tile == 2)
        {
            return m_GreySquare;
        }
        else if (tile == 3)
        {
            return m_StartSquare;
        }
        else if (tile == 4)
        {
            return m_EndSquare;
        }
        else
        {
            return null;
        }
    }

    private Vector2 GetWorldPos(int x, int y)
    {
        return new Vector2(x, y) * m_CellSize + m_OriginPos;
    }

    private void GetXY(Vector2 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - m_OriginPos).x / m_CellSize);
        y = Mathf.FloorToInt((worldPos - m_OriginPos).y / m_CellSize);
    }

    public void SetValue(int x, int y, int value)
    {
        if (x >= 0 && y >= 0 && x < m_Width && y < m_Height)
        {
            m_GridArray[x, y] = value;
            m_DebugTextArray[x, y].text = m_GridArray[x, y].ToString();

            Destroy(m_Squares[x, y]);
            CreateSquares(x, y, GetCorrectSquare(value));
        }
    }

    private void CreateSquares(int x, int y, GameObject go)
    {
        m_Squares[x, y] = Instantiate(go);
        m_Squares[x, y].transform.position = GetWorldPos(x, y) + new Vector2(m_CellSize, m_CellSize) * 0.5f;
        m_Squares[x, y].transform.localScale = new Vector2(m_CellSize, m_CellSize) * 0.75f;
    }

    private void CreatePathSquares(int x, int y, GameObject go)
    {
        GameObject pathSquare = Instantiate(go);
        pathSquare.transform.position = GetWorldPos(x, y) + new Vector2(m_CellSize, m_CellSize) * 0.5f;
        pathSquare.transform.localScale = new Vector2(m_CellSize, m_CellSize) * 0.75f;
        m_PathSquares.Add(pathSquare);
    }

    public Vector2Int SetValue(Vector2 worldPos, int value)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        SetValue(x, y, value);
        return new Vector2Int(x, y);
    }

    public int GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < m_Width && y < m_Height)
        {
            return m_GridArray[x, y];
        }
        else
        {
            return -1;
        }
    }

    public int GetValue(Vector2 worldPos)
    {
        int x, y;
        GetXY(worldPos, out x, out y);
        return GetValue(x, y);
    }

    private static TextMesh CreateWorldText(Transform parent, string text, Vector2 localPos, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
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

    private Vector2 Move(Vector2 position, int direction)
    {
        switch (direction)
        {          
             case 0: // Up
                if (position.y + 1 >= m_GridArray.GetLength(0) || m_GridArray[(int)(position.x), (int)position.y + 1] == 1)
                    break;
                else
                    position.y += 1;
                break;
            case 1: // Down
                if (position.y - 1 < 0 || m_GridArray[(int)(position.x), (int)position.y - 1] == 1)
                    break;
                else
                    position.y -= 1;
                break;
            case 2: // Right
                if (position.x + 1 >= m_GridArray.GetLength(1) || m_GridArray[(int)position.x + 1, (int)(position.y)] == 1)
                    break;
                else
                    position.x += 1;
                break;
            case 3: // West
                if (position.x - 1 < 0 || m_GridArray[(int)position.x - 1, (int)(position.y)] == 1)
                    break;
                else
                    position.x -= 1;
                break;
        }
        return position;
    }

    public void Solve()
    {
        if(m_GeneticAlgorithm.m_Busy)
        {
            m_GeneticAlgorithm.Epoch();
        }
        DrawPath();

    }

    private void DrawPath()
    {
        ClearPath();
        Genome bestGenome = m_GeneticAlgorithm.m_Genomes[m_GeneticAlgorithm.m_FittestGenome];
        List<int> bestDirection = m_GeneticAlgorithm.Decode(bestGenome.m_Bits);
        Vector2 pos = m_StartPos;

        foreach(var direction in bestDirection)
        {
            pos = Move(pos, direction);
            CreatePathSquares((int)pos.x, (int)pos.y, GetCorrectSquare(2)); // Grey.
        }

    }

    private void ClearPath()
    {
        foreach (GameObject pathTile in m_PathSquares)
        {
            Destroy(pathTile);
        }
        m_PathSquares.Clear();
    }

    public double Fitness(List<int> directions)
    {
        Vector2 pos = m_StartPos;

        for(int idx = 0; idx < directions.Count; idx++)
        {
            var nextDirection = directions[idx];
            pos = Move(pos, nextDirection);
        }

        Vector2 absPos = new Vector2( Math.Abs(pos.x - m_EndPos.x), Math.Abs(pos.y - m_EndPos.y));
        double result = 1 / (double)(absPos.x + absPos.y + 1);

        return result;
    }

}
