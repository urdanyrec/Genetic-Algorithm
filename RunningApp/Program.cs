using GeneticAlgo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningApp
{
    internal class Program
    {
		public static double theActualFunction(double[] values)
		{
			if (values.GetLength(0) != 2)
				throw new ArgumentOutOfRangeException("should only have 2 args");

			double x = values[0];
			double y = values[1];
			return -Math.Cos(x) * Math.Cos(y) * Math.Exp(-(Math.Pow(x - Math.PI, 2) + Math.Pow(y - Math.PI, 2)));
		}

		public static void Main(string[] args)
        {
			//  optimal solution for this is (0.5,0.5)
			GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(0.5, 0.005, 0.1, 100, 5000, 2);
			geneticAlgorithm.FitnessFunction = new GeneticAlgorithm.GAFunction(theActualFunction);
			geneticAlgorithm.Elitism = false;
			geneticAlgorithm.Go();

			Console.WriteLine("Время выполнения алгоритма: " + geneticAlgorithm.Stopwatch.ElapsedMilliseconds.ToString() + " мс");

			double[] values;
			double fitness;

			geneticAlgorithm.GetBest(out values, out fitness);
			Console.WriteLine("Best ({0}):", fitness);
			for (int i = 0; i < values.Length; i++)
			{
				Console.WriteLine("{0} ", values[i]);
			}

			geneticAlgorithm.GetWorst(out values, out fitness);
			Console.WriteLine("\nWorst ({0}):", fitness);
			for (int i = 0; i < values.Length; i++)
			{
				Console.WriteLine("{0} ", values[i]);
			}
		}
    }
}
