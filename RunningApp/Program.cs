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
			GeneticAlgorithm geneticAlgorithm;
			TestValues testValues;
			Task.Run(async () =>
			{
				geneticAlgorithm = await GoWrapper();
				testValues = geneticAlgorithm.Values;
				for (int i = 0; i < geneticAlgorithm.Values.RunningTimes.Count; i++)
				{
					await Task.Run(() => 
					{ 
						Console.WriteLine("Test {0}", i);
						Console.WriteLine("BestFitness {0}", testValues.BestFitness[i]);
						Console.WriteLine("RunningTime {0}", testValues.RunningTimes[i]);
						Console.WriteLine("GenerationCounts {0}", testValues.GenerationCounts[i]);
						Console.WriteLine("");
					});
				}
				Console.WriteLine("");
				Console.WriteLine("Summary");
				Console.WriteLine("BestFitness");
				Console.WriteLine("Min {0}", testValues.BestFitness.Min());
				Console.WriteLine("Max {0}", testValues.BestFitness.Max());
				Console.WriteLine("Average {0}", testValues.BestFitness.Average());
				Console.WriteLine("");
				Console.WriteLine("RunningTimes");
				Console.WriteLine("Min {0}", testValues.RunningTimes.Min());
				Console.WriteLine("Max {0}", testValues.RunningTimes.Max());
				Console.WriteLine("Average {0}", testValues.RunningTimes.Average());
				Console.WriteLine("");
				Console.WriteLine("GenerationCounts");
				Console.WriteLine("Min {0}", testValues.GenerationCounts.Min());
				Console.WriteLine("Max {0}", testValues.GenerationCounts.Max());
				Console.WriteLine("Average {0}", testValues.GenerationCounts.Average());

			}).GetAwaiter().GetResult();
		}

		public static async ValueTask<GeneticAlgorithm> GoWrapper()
        {
			GeneticAlgorithm geneticAlgorithm = new GeneticAlgorithm(0.5, 0.7, 0.9, 20, 5000000, 2);
			geneticAlgorithm.FitnessFunction = new GeneticAlgorithm.GAFunction(theActualFunction);
			geneticAlgorithm.Elitism = false;

			for (int i = 0; i < 5; i++)
				await Task.Run(() => geneticAlgorithm.Go());		
			
			return geneticAlgorithm;
		}
    }
}
