//  All code copyright (c) 2003 Barry Lapthorn
//  Website:  http://www.lapthorn.net
//
//  Disclaimer:  
//  All code is provided on an "AS IS" basis, without warranty. The author 
//  makes no representation, or warranty, either express or implied, with 
//  respect to the code, its quality, accuracy, or fitness for a specific 
//  purpose. Therefore, the author shall not have any liability to you or any 
//  other person or entity with respect to any liability, loss, or damage 
//  caused or alleged to have been caused directly or indirectly by the code
//  provided.  This includes, but is not limited to, interruption of service, 
//  loss of data, loss of profits, or consequential damages from the use of 
//  this code.
//
//
//  $Author: barry $
//  $Revision: 1.1 $
//
//  $Id: GA.cs,v 1.1 2003/08/19 20:59:05 barry Exp $

using System;
using System.Collections;
using System.IO;

namespace btl.generic
{
    public class GA
	{
		private ArrayList thisGeneration;
		private ArrayList nextGeneration;
		private ArrayList fitnessTable;
		private string strFitness;
		private double totalFitness;

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

		public GA(double crossoverRate, double mutationRateMin, double mutationRateMax, int populationSize, int generationSize, int genomeSize)
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
			if (outputFitness != null)
				outputFitness.Close();
		}

		/// <summary>
		/// After ranking all the genomes by fitness, use a 'roulette wheel' selection
		/// method.  This allocates a large probability of selection to those with the 
		/// highest fitness.
		/// </summary>
		/// <returns>Random individual biased towards highest fitness</returns>
		private int RouletteSelection()
		{
			double randomFitness = random.NextDouble() * totalFitness;
			int idx = -1;
			int first = 0;
			int last = PopulationSize -1;
			int mid = (last - first) / 2;

			//  ArrayList's BinarySearch is for exact values only
			//  so do this by hand.
			while (idx == -1 && first <= last)
			{
				if (randomFitness < (double)fitnessTable[mid])
				{
					last = mid;
				}
				else if (randomFitness > (double)fitnessTable[mid])
				{
					first = mid;
				}
				mid = (first + last) / 2;
				//  lies between i and i+1
				if ((last - first) == 1)
					idx = last;
			}
			return idx;
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
				/*if (i == 0)
                {
					Genome gpi = new Genome(GenomeSize);
					gpi.Genes[0] = 3.14;
					gpi.Genes[1] = 3.14;
					thisGeneration.Add(gpi);
				}*/
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
				int pidx1 = RouletteSelection();
				int pidx2 = RouletteSelection();
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
			Genome g = ((Genome)thisGeneration[PopulationSize-1]);
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
			Genome g = ((Genome)thisGeneration[n]);
			values = new double[g.Length];
			g.GetValues(ref values);
			fitness = (double)g.Fitness;
		}
	}
}
