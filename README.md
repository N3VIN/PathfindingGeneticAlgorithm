
# PathfindingGeneticAlgorithm
A research project on Pathfinding using Genetic Algorithm.


## Demo




![gif](https://i.gyazo.com/33011fcb3725d4cc2db90ad70d9ac82c.gif)


## What is Genetic Algorithm?

Genetic algorithm is a method used to solve problems of optimization by copying the process of natural selection. 
Genetic Algorithm attempts to generating a solution by using some operations which we will discuss shortly.

Genetic Algorithm don't guarantee a best solution. Over time, the Algorithm might converge to a solution.




## How Does Genetic Algorithm work?

The Process starts by finding a way to encode the solution to the problem as a “digital” chromosome.
Then, you create a start population of random chromosomes and evolve them over time by “breeding”
the fittest individuals and adding a little mutation.

The “digital” chromosomes are encoded as a series of binary bits also called gene. In the beginning you always create a population of chromosomes and the bits of the chromosomes are set at random.

![gif](https://4.bp.blogspot.com/-PHR62YR3N0U/W-oqIrXZRLI/AAAAAAAACdU/nZmvrLbDp_MhIcfd7jrxoc-rV92Q6eDAgCLcBGAs/s400/Genes%252C%2BChromosomes%2Band%2BPopulation.jpg)

There are few stages in Genetic Algorithm.

- Fitness Function.
- Roulette wheel Selection.
- Crossover.
- Mutation.

## Implementation with explanation.

The Demo was implemented in `Unity3D` using `C#`.

#### Internal Structure.

The map has a custom Grid Class that handles everything related to the visuals. It allows for easy traversal through the different arrays.
It also manages Instanstiating the prefabs.

There is a simple Implementation for the genome class with relevent information.

```c#
public class Genome
{
    public List<int> m_Bits = new List<int>();
    public double m_Fitness = 0 ;
}
```

### Fitness Function

Fitness Function determines how fit the member of the population is. There is a fitness score associated with every individual. The more fit the member is, higher the fitness score hence the chances of it getting selected are high.

```c#
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
```
This Function returns a fitness score based on the member distance from the exit.
The `absPos` holds the position of the member.
We get the fitness score `result` by adding the X position and the Y position of the member and calculating the inverse.


### Roulette wheel Selection

Roulette wheel Selection is a process of choosing the member from the population from existing generation in a way that it is directly proportional to the fitness of the member. The members selected are called parents because they will be used to create the next generation.

```c#
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
```
This function returns a genome using a probability proportional to its fitness.
The `slice` holds a random number from 0 to the total of all fitness score `m_TotalFitnessScore`.

The loop iterates through genomes adding the fitness score. When the score is more than the `slice`, it returns the genome `selectedGenome`.


### Crossover

Crossover is a process of combining the genetic information from the parents to produce the new offspring. It is a way to generate new solutions from an existing population.
It can be seen as the crossover that happens in sexual reproduction in biology.

```c#
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
```
This function splits the chromosomes and swaps the bits to create 2 new chromosomes.
`crossoverPoint` is a random point chosen to split the chromosomes.
The bits are then swapped from each parent in the for loops and added to `baby1` and `baby2`.

### Mutation

Mutation is a process of arranging the genes in the chromosome to produce a totally new chromosome. Just Like Crossover, Mutation can be seen as the process that happens in evolutionary biology.

```c#
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
```
This function goes through the chromosome and flips the bits according to the `m_sMutationRate`.

### Epoch

```c#
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
```

This function is the main loop for the genetic algorithm.
Inside the `while` loop Genome `mom` and `dad` are being created with the `RouletteWheelSelection()` function which returns the fittest genome.
`Crossover()` function is being called where it combines the genetic information from `mom` and `dad` and stores it in `baby1` and `baby2`.
`Mutate()` function is called where it flips the bits of the chromosomes according to the mutation rate.

The new Genome `babies` is added to the population and this process keeps repeating until the population reaches the starting population.
## Result

![gif](https://i.gyazo.com/cb26eea582cceb40ca855d5c8c69d79e.png)

Mutation Rate = 0.001
Population Size = 140
Chromosome Length = 70
Num of generations = 5 

![gif](https://i.gyazo.com/45308ccdfac0b7f7a80510022eaa7d6c.png)
Mutation Rate = 0.01
Population Size = 280
Chromosome Length = 140
Num of generations = 26 
## Future work

The Pathfinder can be improved by implementing diffrent encoding techniques and adopting specialised operators and Mathematical functions for specific need which will help maintain diversity while also having the fitter genomes.