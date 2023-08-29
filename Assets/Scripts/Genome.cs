using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    public List<int> m_Bits = new List<int>();
    public double m_Fitness = 0 ;

    public Genome()
    {
    }

    public Genome(int numBits)
    {
        for(int i = 0; i < numBits; i++ )
        {
            m_Bits.Add(UnityEngine.Random.Range(0, 2));
        }
    }

}
