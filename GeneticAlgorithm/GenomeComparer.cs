using System;
using System.Collections;

namespace GeneticAlgo
{
	/// <summary>
	/// Compares genomes by fitness
	/// </summary>
	public sealed class GenomeComparer : IComparer
	{
		public GenomeComparer()
		{
		}

		public int Compare(object x, object y)
		{
			if ( !(x is Genome) || !(y is Genome))
				throw new ArgumentException("Not of type Genome");

			if (((Genome)x).Fitness > ((Genome)y).Fitness)
				return 1;
			else if (((Genome)x).Fitness == ((Genome)y).Fitness)
				return 0;
			else
				return -1;

		}
	}
}
