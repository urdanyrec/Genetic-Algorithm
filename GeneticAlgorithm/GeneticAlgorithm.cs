using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace GeneticAlgo
{
    public class GeneticAlgorithm
	{
		private ArrayList thisGeneration;
		private ArrayList nextGeneration;
		private ArrayList fitnessTable;
		private string strFitness;
		private double totalFitness;
		public Stopwatch Stopwatch = new Stopwatch();

		static Random random = new Random();

		public delegate double GAFunction(double[] values);
		public GAFunction FitnessFunction { get; set; }
		public int PopulationSize { get; set; }
		public int GenomeSize { get; set; }
		public double CrossoverRate { get; set; }
		public double MutationRateMin { get; set; }
		public double MutationRateMax { get; set; }	
		public string FitnessFile { get; set; }
		public int GenerationSize { get; set; }

		/// <summary>
		/// Keep previous generation's fittest individual in place of worst in current
		/// </summary>
		public bool Elitism { get; set; }

		public GeneticAlgorithm(double crossoverRate, double mutationRateMin, double mutationRateMax, int populationSize, int generationSize, int genomeSize)
		{
			Elitism = true;
			MutationRateMin = mutationRateMin;
			MutationRateMax = mutationRateMax;
			CrossoverRate = crossoverRate;
			PopulationSize = populationSize;
			GenerationSize = generationSize;
			GenomeSize = genomeSize;
			strFitness = Environment.CurrentDirectory + @"\lalala.txt";
		}

		/// <summary>
		/// Method which starts the GA executing.
		/// </summary>
		public void Go()
		{
			Stopwatch.Restart();

			if (FitnessFunction == null)
				throw new ArgumentNullException("Need to supply fitness function");
			if (GenomeSize == 0)
				throw new IndexOutOfRangeException("Genome size not set");

			//  Create the fitness table.
			fitnessTable = new ArrayList();
			thisGeneration = new ArrayList(GenerationSize);
			nextGeneration = new ArrayList(GenerationSize);
			Genome.MutationRateMin = MutationRateMin;
			Genome.MutationRateMax = MutationRateMax;


			CreateGenomes();
			RankPopulation();

			StreamWriter outputFitness = null;
			bool write = false;
			if (FitnessFunction != null)
			{
				write = true;
				outputFitness = new StreamWriter(strFitness);
			}

			for (int i = 0; i < GenerationSize; i++)
			{
				CreateNextGeneration();
				RankPopulation();
				if (write)
				{
					if (outputFitness != null)
					{
						double d = (double)((Genome)thisGeneration[PopulationSize-1]).Fitness;
						outputFitness.WriteLine("{0},{1}",i,d);
						Console.WriteLine("{0},{1}", i, d);
					}
				}
			}
			Stopwatch.Stop();

			if (outputFitness != null)
				outputFitness.Close();
		}

		/// <summary>
		/// After ranking all the genomes by fitness, use a 'roulette wheel' selection
		/// method.  This allocates a large probability of selection to those with the 
		/// highest fitness.
		/// </summary>
		/// <returns>Random individual biased towards highest fitness</returns>
		private int TournireSelection()
		{
			List<int> selectedNumbers = new List<int>();
			while (selectedNumbers.Count < 4)
            {
				int iter = random.Next(fitnessTable.Count);
				if(!selectedNumbers.Exists(element => element == iter))
					selectedNumbers.Add(iter);
			}
			double maxFitness = Math.Abs((double)fitnessTable[selectedNumbers[0]]);
			int maxNumber = selectedNumbers[0];
			for (int i = 1; i < selectedNumbers.Count - 1; i++)
            {
				if (maxFitness > Math.Abs(selectedNumbers[i]))
                {
					maxFitness = (double)fitnessTable[selectedNumbers[i]];
					maxNumber = selectedNumbers[i];
				}				
            }
			return maxNumber;
		}

		/// <summary>
		/// Rank population and sort in order of fitness.
		/// </summary>
		private void RankPopulation()
		{
			totalFitness = 0;
			for (int i = 0; i < PopulationSize; i++)
			{
				Genome g = ((Genome)thisGeneration[i]);
				g.Fitness = FitnessFunction(g.Genes);
				totalFitness += g.Fitness;
			}
			thisGeneration.Sort(new GenomeComparer());

			//  now sorted in order of fitness.
			double fitness = 0.0;
			fitnessTable.Clear();
			for (int i = 0; i < PopulationSize; i++)
			{
				fitness += ((Genome)thisGeneration[i]).Fitness;
				fitnessTable.Add((double)fitness);
			}
		}

		/// <summary>
		/// Create the *initial* genomes by repeated calling the supplied fitness function
		/// </summary>
		private void CreateGenomes()
		{
			for (int i = 0; i < PopulationSize ; i++)
			{
				Genome g = new Genome(GenomeSize);
				thisGeneration.Add(g);
			}
		}

		private void CreateNextGeneration()
		{
			nextGeneration.Clear();
			Genome g = null;
			if (Elitism)
				g = (Genome)thisGeneration[PopulationSize - 1];

			for (int i = 0 ; i < PopulationSize ; i+=2)
			{
				int pidx1 = TournireSelection();
				int pidx2 = TournireSelection();
				Genome parent1, parent2, child1, child2;
				parent1 = ((Genome) thisGeneration[pidx1]);
				parent2 = ((Genome) thisGeneration[pidx2]);

				if (random.NextDouble() < CrossoverRate)
				{
					parent1.Crossover(ref parent2, out child1, out child2);
				}
				else
				{
					child1 = parent1;
					child2 = parent2;
				}
				child1.Mutate();
				child2.Mutate();

				nextGeneration.Add(child1);
				nextGeneration.Add(child2);
			}
			if (Elitism && g != null)
				nextGeneration[0] = g;

			thisGeneration.Clear();
			for (int i = 0 ; i < PopulationSize; i++)
				thisGeneration.Add(nextGeneration[i]);
		}

		public void GetBest(out double[] values, out double fitness)
		{
			Genome g = ((Genome)thisGeneration[0]);
			values = new double[g.Length];
			g.GetValues(ref values);
			fitness = (double)g.Fitness;
		}

		public void GetWorst(out double[] values, out double fitness)
		{
			GetNthGenome(0, out values, out fitness);
		}

		public void GetNthGenome(int n, out double[] values, out double fitness)
		{
			if (n < 0 || n > PopulationSize-1)
				throw new ArgumentOutOfRangeException("n too large, or too small");
			Genome g = ((Genome)thisGeneration[PopulationSize - 1]);
			values = new double[g.Length];
			g.GetValues(ref values);
			fitness = (double)g.Fitness;
		}
	}
}
