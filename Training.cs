/*
 * Training.cs
 */
using System;
using System.Collections;

namespace CNB {
	public static class Training {
		public static ArrayList Category;
		//ALL CATEGORIES
		public static M_CNB[] M;
		//MIX-MODEL FOR TIER II
		public static M_CNB[] M1;
		//SINGLE-MODEL FOR TIER I
		public static int S_M;

		public static void OptimizeModel(int NumberOfAttributes, int NumberOfClasses, DataPoint[] Tr) {
			DataPoint[] mean, weight;
			float[] deviation;
			M1 = new M_CNB[NumberOfClasses];
			M = new M_CNB[NumberOfClasses];
			var ell = new int[NumberOfClasses];

			Category = DataFile.GetCategories(Tr, NumberOfClasses);//GET EACH CATEGORY
			var U = new DataPoint[1];//INITIALIZATION
			for(int k=0; k<NumberOfClasses; k++) {//GET M^{(1)}_k
				U[0] = new DataPoint(((DataPoint[])Category[k])[0].GetClass(), ((DataPoint[])Category[k])[0].GetSubClass(), ((DataPoint[])Category[k])[0].GetPoint());//SET THE FIRST POINT AS THE CENTROID IF \ell_k=1
				ModelLearning.ProbabilityClustering(k, 1, NumberOfAttributes, U, (DataPoint[])Category[k], out mean, out weight, out deviation);
				M1[k] = new M_CNB(mean, weight, deviation);
				ell[k] = 1;
			}
			if(S_M == 1) {//MIX-MODEL
				double q, newq;
				for(int k=0; k<NumberOfClasses; k++) {
					mean = (DataPoint[])M1[k].CNBmu.Clone();
					weight = (DataPoint[])M1[k].CNBw.Clone();
					deviation = (float[])M1[k].CNBdev.Clone();

					newq = Evaluation.Quality(k, NumberOfAttributes, out U, (DataPoint[])Category[k], M1, mean, weight, deviation);
					do {
						q = newq;
						if(q == 1 || ((DataPoint[])Category[k]).Length == ell[k]++) {
							break;
						}
						ModelLearning.ProbabilityClustering(k, ell[k], NumberOfAttributes, U, (DataPoint[])Category[k], out mean, out weight, out deviation);
						newq = Evaluation.Quality(k, NumberOfAttributes, out U, (DataPoint[])Category[k], M1, mean, weight, deviation);
					} while(q<newq);
					M[k] = new M_CNB(mean, weight, deviation);
					Console.WriteLine("Class {0} has {1} models", k + 1, M[k].CNBmu.Length);
				}
			}
		}
	}
}
