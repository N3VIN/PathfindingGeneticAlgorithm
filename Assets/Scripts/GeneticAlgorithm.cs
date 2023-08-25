using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{
    public List<Genome> genomes;
    public List<Genome> prevGenGenomes;
	public int populationSize = 140;
	public double crossoverRate = 0.7f;
	public double mutationRate = 0.001f;
	public int chromosomeLength = 70;
	public int geneLength = 2;
	public int fittestGenome;
	public double bestFitnessScore;
	public double totalFitnessScore;
	public int generation;
	public bool busy;
	public Grid grid;

	public GeneticAlgorithm()
	{
		busy = false;
		genomes = new List<Genome>();
		prevGenGenomes = new List<Genome>();
	}

	public void Run()
    {
		CreateFirstPopulation();
		busy = true;
    }

	public void CreateFirstPopulation()
    {
		genomes.Clear();

		for(int i = 0; i < populationSize; i++)
        {
			var baby = new Genome(chromosomeLength);
			genomes.Add(baby);
        }
    }

	public void Epoch()
    {
		if(!busy)
        {
			return;
        }

		UpdateFitnessScores();

		if(!busy)
        {
			prevGenGenomes.Clear();
			prevGenGenomes.AddRange(genomes);
			return;
        }

		int noOfNewBabies = 0;

		List<Genome> babies = new List<Genome>();
		while(noOfNewBabies < populationSize)
        {
			Genome mom = RouletteWheelSelection();
			Genome dad = RouletteWheelSelection();
			Genome baby1 = new Genome();
			Genome baby2 = new Genome();
			Crossover(mom.bits, dad.bits, baby1.bits, baby2.bits);
			Mutate(baby1.bits);
			Mutate(baby2.bits);
			babies.Add(baby1);
			babies.Add(baby2);

			noOfNewBabies += 2;
        }

		prevGenGenomes.Clear();
		prevGenGenomes.AddRange(genomes);

		genomes = babies;

		generation++;
    }

	public void UpdateFitnessScores()
    {
		fittestGenome = 0;
		bestFitnessScore = 0;
		totalFitnessScore = 0;

		for(int i = 0; i < populationSize; i++)
        {
			List<int> directions = Decode(genomes[i].bits);

			genomes[i].fitness = grid.Fitness(directions); // complete this.

			totalFitnessScore += genomes[i].fitness;

			if(genomes[i].fitness > bestFitnessScore)
            {
				bestFitnessScore = genomes[i].fitness;
				fittestGenome = i;

				if(genomes[i].fitness == 1)
                {
					busy = false;
					return;
                }
            }
        }
    }

	public List<int> Decode(List<int> bits)
    {
		List<int> directions = new List<int>();

		for(int i = 0; i < bits.Count; i+= geneLength) // i is the index of the gene.
        {
			List<int> gene = new List<int>();

			for(int j = 0; j < geneLength; j++) // j is the index of the bits.
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
		double slice = UnityEngine.Random.value * totalFitnessScore;
		double total = 0;
		int selectedGenome = 0;

		for(int i = 0; i < populationSize; i++)
        {
			total += genomes[i].fitness;

			if (total > slice)
            {
				selectedGenome = i;
				break;
            }
        }

		return genomes[selectedGenome];
    }

	private void Crossover(List<int> mom, List<int> dad, List<int> baby1, List<int> baby2)
    {
		if(UnityEngine.Random.value > crossoverRate || mom == dad)
        {
			baby1.AddRange(mom);
			baby2.AddRange(mom);

			return;
        }

		var random = new System.Random();

		int crossoverPoint = random.Next(0, chromosomeLength - 1);

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
			if(UnityEngine.Random.value < mutationRate)
            {
				//bits[i] = bits[i] == 0 ? 1 : 0;
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
