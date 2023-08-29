using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    private Grid m_Grid;
    private bool m_Solve = false;

    public GameObject m_BlackSquare;
    public GameObject m_WhiteSquare;
    public GameObject m_GreySquare;
    public GameObject m_StartSquare;
    public GameObject m_EndSquare;
    public double m_MutationRate = 0.01f;
    public int m_PopulationSize = 560;
    public int m_ChromosomeLength = 280;

    private void Start()
    {
        //grid = new Grid();
        m_Grid = gameObject.AddComponent<Grid>();
        m_Grid.m_BlackSquare = m_BlackSquare;
        m_Grid.m_WhiteSquare = m_WhiteSquare;
        m_Grid.m_GreySquare = m_GreySquare;
        m_Grid.m_StartSquare = m_StartSquare;
        m_Grid.m_EndSquare = m_EndSquare;
        m_Grid.UpdateGrid(20, 20, 5f, new Vector3(-70, -75));

        GeneticAlgorithm.m_sMutationRate = m_MutationRate;
        GeneticAlgorithm.m_sPopulationSize = m_PopulationSize;
        GeneticAlgorithm.m_sChromosomeLength = m_ChromosomeLength;     
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            m_Grid.m_StartPos = m_Grid.SetValue(GetMouseWorldPos(), 3);
        }

        if (Input.GetMouseButtonDown(2))
        {
            var temp = m_Grid.SetValue(GetMouseWorldPos(), 1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            m_Grid.m_EndPos = m_Grid.SetValue(GetMouseWorldPos(), 4);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Solve = true;
            m_Grid.m_GeneticAlgorithm.Run();

        }

        if (m_Solve)
        {
            m_Grid.Solve();
        }

    }

    private Vector2 GetMouseWorldPos()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector2(worldPos.x, worldPos.y);
    }

}
