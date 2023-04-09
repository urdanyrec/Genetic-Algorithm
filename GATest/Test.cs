using System;
using btl.generic;
using static btl.generic.GA;

public class Test
{
	//  optimal solution for this is (0.5,0.5)
	public static double theActualFunction(double[] values)
	{
		if (values.GetLength(0) != 2)
			throw new ArgumentOutOfRangeException("should only have 2 args");

		double x = values[0];
		double y = values[1];
		//double f1 = Math.Pow(x + 2 * y - 7, 2) + Math.Pow(2 * x + y - 5, 2);
		//double f1 = Math.Pow(15 * x * y * (1 - x) * (1 - y) * Math.Sin(9 * Math.PI * x) * Math.Sin(9 * Math.PI * y), 2);
		double f1 = -Math.Cos(x) * Math.Cos(y) * Math.Exp(-(Math.Pow(x - Math.PI, 2) + Math.Pow(y - Math.PI, 2)));
		return f1;
	}

	public static void Main()
	{
		//  Crossover		= 80% 
		//  Mutation		=  5%
		//  Population size = 100
		//  Generations		= 2000
		//  Genome size		= 2
		GA geneticAlgorithm = new GA(0.5, 0.005, 0.1, 100, 20000, 2);		
		geneticAlgorithm.FitnessFunction = new GAFunction(theActualFunction);
		geneticAlgorithm.Elitism = false;
		geneticAlgorithm.Go();

		double[] values;
		double fitness;

		geneticAlgorithm.GetBest(out values, out fitness);
		Console.WriteLine("Best ({0}):", fitness);
		for (int i = 0 ; i < values.Length ; i++)
        {
			Console.WriteLine("{0} ", values[i]);
		}
	
		geneticAlgorithm.GetWorst(out values, out fitness);
		Console.WriteLine("\nWorst ({0}):", fitness);
		for (int i = 0 ; i < values.Length ; i++)
        {
			Console.WriteLine("{0} ", values[i]);
		}	
	}
}
