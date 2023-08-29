using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
	public static int m_sPopulationSize = 560;
	public static double m_sMutationRate = 0.01f;
	public static int m_sChromosomeLength = 280;

	public List<Genome> m_Genomes = new List<Genome>();
	public int m_Generation;
	public bool m_Busy = false;
	public Grid m_Grid;

	private double m_CrossoverRate = 0.7f;
	private int m_GeneLength = 2;
	public int m_FittestGenome;
	private double m_BestFitnessScore;
	private double m_TotalFitnessScore;

	public void Run()
    {
		CreateFirstPopulation();
		m_Busy = true;
    }

	private void CreateFirstPopulation()
    {
		m_Genomes.Clear();

		for(int i = 0; i < m_sPopulationSize; i++)
        {
			var baby = new Genome(m_sChromosomeLength);
			m_Genomes.Add(baby);
        }
    }

	public void Epoch()
    {
		if(!m_Busy)
			return;

		UpdateFitnessScores();

		if (!m_Busy)
			return;

		int noOfNewBabies = 0;

		List<Genome> babies = new List<Genome>();
		while(noOfNewBabies < m_sPopulationSize)
        {
			Genome mom = RouletteWheelSelection();
			Genome dad = RouletteWheelSelection();
			Genome baby1 = new Genome();
			Genome baby2 = new Genome();
			Crossover(mom.m_Bits, dad.m_Bits, baby1.m_Bits, baby2.m_Bits);
			Mutate(baby1.m_Bits);
			Mutate(baby2.m_Bits);
			babies.Add(baby1);
			babies.Add(baby2);

			noOfNewBabies += 2;
        }

		m_Genomes = babies;

		++m_Generation;
    }

	private void UpdateFitnessScores()
    {
		m_FittestGenome = 0;
		m_BestFitnessScore = 0;
		m_TotalFitnessScore = 0;

		for(int i = 0; i < m_sPopulationSize; i++)
        {
			List<int> directions = Decode(m_Genomes[i].m_Bits);

			m_Genomes[i].m_Fitness = m_Grid.Fitness(directions);

			m_TotalFitnessScore += m_Genomes[i].m_Fitness;

			if(m_Genomes[i].m_Fitness > m_BestFitnessScore)
            {
				m_BestFitnessScore = m_Genomes[i].m_Fitness;
				m_FittestGenome = i;

				if(m_Genomes[i].m_Fitness == 1)
                {
					Debug.Log("No of Generations: " + m_Generation);
					m_Busy = false;
					return;
                }
            }
        }
    }

	public List<int> Decode(List<int> bits)
    {
		List<int> directions = new List<int>();

		for(int i = 0; i < bits.Count; i+= m_GeneLength) // i is the index of the gene.
        {
			List<int> gene = new List<int>();

			for(int j = 0; j < m_GeneLength; j++) // j is the index of the bits.
            {
				gene.Add(bits[i + j]);
            }

			directions.Add(BitsToInt(gene));
        }

		return directions;
    }

	private int BitsToInt(List<int> gene)
    {
		int value = 0;
		int lambda = 1;

		for(int i = gene.Count; i > 0; i--)
        {
			value += gene[i - 1] * lambda;
			lambda *= 2;
        }

		return value;
    }

	private Genome RouletteWheelSelection()
    {
		double slice = UnityEngine.Random.value * m_TotalFitnessScore;
		double total = 0;
		int selectedGenome = 0;

		for(int i = 0; i < m_sPopulationSize; i++)
        {
			total += m_Genomes[i].m_Fitness;

			if (total > slice)
            {
				selectedGenome = i;
				break;
            }
        }

		return m_Genomes[selectedGenome];
    }

	private void Crossover(List<int> mom, List<int> dad, List<int> baby1, List<int> baby2)
    {
		if(UnityEngine.Random.value > m_CrossoverRate || mom == dad)
        {
			baby1.AddRange(mom);
			baby2.AddRange(mom);

			return;
        }

		int crossoverPoint = Random.Range(0, m_sChromosomeLength - 1);

		for(int i = 0; i < crossoverPoint; i++)
        {
			baby1.Add(mom[i]);
			baby2.Add(dad[i]);
        }

		for(int i = crossoverPoint; i < mom.Count; i++)
        {
			baby1.Add(dad[i]);
			baby2.Add(mom[i]);
		}
    }

	private void Mutate(List<int> bits)
    {
		for(int i = 0; i < bits.Count; i++)
        {
			if(UnityEngine.Random.value < m_sMutationRate)
            {
				if (bits[i] == 0)
				{
					bits[i] = 1;
				}
				else
                {
					bits[i] = 0;
                }
			}
        }
    }

}
