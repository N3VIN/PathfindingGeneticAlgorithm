using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    public List<int> bits;
    public double fitness;

    public Genome()
    {
        fitness = 0;
        bits = new List<int>();
    }

    public Genome(int numBits)
    {
        fitness = 0;
        bits = new List<int>();

        for(int i = 0; i < numBits; i++ )
        {
            var randomNum = new System.Random(DateTime.Now.GetHashCode() * SystemInfo.processorFrequency.GetHashCode());
            bits.Add(randomNum.Next(0, 2));
        }
    }

}
