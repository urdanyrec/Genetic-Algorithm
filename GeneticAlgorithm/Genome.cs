using System;
using System.Collections;

namespace GeneticAlgo
{
	/// <summary>
	/// Summary description for Genome.
	/// </summary>
	public class Genome
	{
		private static Random random = new Random();

		public double Fitness { get; set; }
		public static double MutationRateMax { get; set; }
		public static double MutationRateMin { get; set; }
		public int Length { get; set; }
		public double[] Genes { get; set; }


		public Genome(int length)
		{
			Length = length;
			Genes = new double[length];
			CreateGenes();
		}

		public Genome(int length, bool createGenes)
		{
			Length = length;
			Genes = new double[length];
			if (createGenes)
				CreateGenes();
		}		 

		private void CreateGenes()
		{
			for (int i = 0; i < Length; i++)
				Genes[i] = random.NextDouble();
		}

		public void Crossover(ref Genome genome2, out Genome child1, out Genome child2)
		{
			int pos = (int)(random.NextDouble() * (double)Length);
			child1 = new Genome(Length, false);
			child2 = new Genome(Length, false);
			for(int i = 0 ; i < Length ; i++)
			{
				if (i < pos)
				{
					child1.Genes[i] = Genes[i];
					child2.Genes[i] = genome2.Genes[i];
				}
				else
				{
					child1.Genes[i] = genome2.Genes[i];
					child2.Genes[i] = Genes[i];
				}
			}
		}

		public void Mutate()
		{
			for (int pos = 0 ; pos < Length; pos++)
			{
				var mutationRate = random.NextDouble() * (MutationRateMax - MutationRateMin) + MutationRateMin;
				if (random.NextDouble() < mutationRate)
                {
					if (random.Next(0, 2) != 0)
						Genes[pos] = (Genes[pos] + (random.NextDouble() / 5));
					else
						Genes[pos] = (Genes[pos] - (random.NextDouble() / 5));
				}					
			}
		}

		public void Output()
		{
			for (int i = 0 ; i < Length ; i++)
			{
				System.Console.WriteLine("{0:F4}", Genes[i]);
			}
			System.Console.Write("\n");
		}

		public void GetValues(ref double[] values)
		{
			for (int i = 0 ; i < Length ; i++)
				values[i] = Genes[i];
		}
	}
}
