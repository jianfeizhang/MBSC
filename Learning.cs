/*
 * Learning.cs
 */
using System;
using System.Collections.Generic;

namespace CNB {
	public static class ModelLearning {
		//RELATIVE NATURAL LOGARITHM FOR POSTERIOR OF ASSIGNING THE INSTANCE GIVEN THE theta
		static public double ReLog_Posterior(int n, int N, int D, IList<float> mean, IList<float> weight, float deviation, IList<float> sample) {
			double log_likelihood = 0.0;
			double log_prior = Math.Log(n / (double)N);
			for(int d=0; d<D; d++) {
				log_likelihood += 0.5 * Math.Log(weight[d]) - 0.5 * Math.Log(2 * Math.PI * deviation) - weight[d] * (sample[d] - mean[d]) * (sample[d] - mean[d]) / (2 * deviation);
			}
			return log_prior + log_likelihood;
		}
		//GET ALL POSSIBLE CLUSTERS OF C_k
		static public void ProbabilityClustering(int k, int ell_k, int D, DataPoint[] U, DataPoint[] C_k, out DataPoint[] mean, out DataPoint[] weight, out float[] deviation) {
			InitializeGaussian.Clustering(k, ell_k, D, U, ref C_k);
			Estimation.GetParameters(k, ell_k, D, C_k, out mean, out weight, out deviation);
		}
		//ASSIGN EACH INSTANCE TO THE MOST PROBABLE CLASS
		static void ProAssigment(int D, DataPoint[] mean, IList<DataPoint> weight, IList<float> deviation, ref DataPoint[] points) {
			double Log_Max, postP = 0.0;
			int probable = 0;
			for(int i=0; i<points.Length; i++) {
				Log_Max = double.MinValue;
				for(int l=0; l<mean.Length; l++) {
					postP = ReLog_Posterior(mean[l].GetCount(), points.Length, D, mean[l].GetPoint(), weight[l].GetPoint(), deviation[l], points[i].GetPoint());
					if(postP > Log_Max) {
						Log_Max = postP;
						probable = l;
					}
				}
				points[i].SetSubClass((mean[probable].GetSubClass()));
			}
		}
	}
}
