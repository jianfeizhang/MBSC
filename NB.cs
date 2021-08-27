/*
 * NB.cs
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace CNB {
	public static class NB {
		public static M_NB[] NB_M;
		//NATURAL LOGARITHM FOR POSTERIOR OF ASSIGNING AN INSTANCE GIVEN theta
		static double Log_Posterior(int n, int N, int D, IList<float> mean, IList<float> deviation, IList<float> sample) {
			double log_likelihood = 0.0;
			double log_prior = Math.Log(n / (double)N);//RELATIVE Log_prior
			for(int d=0; d<D; d++) {
				//SET deviation=10^-5 IN CASE IT VALUES OF 0
				if(deviation[d] == 0) {
					deviation[d] = 0.00001f;
				}
				log_likelihood += -0.5 * Math.Log(2 * Math.PI * deviation[d]) - (sample[d] - mean[d]) * (sample[d] - mean[d]) / (2 * deviation[d]);
			}
			return log_prior + log_likelihood;
		}
		//TRAINING PROCEDURE
		static public void NBTraining(int K, int D, IList C) {
			NB_M = new M_NB[K];//NB MODEL INCLUDES mean AND deviation
			var mean = new DataPoint[1];//ONE mean FOR A CLASS
			var dev = new DataPoint[1];
			DataPoint[] copymean, copydev;
			for(int k=0; k<K; k++) {
				mean[0] = new DataPoint(k, 0, D, 0);
				dev[0] = new DataPoint(k, 0, D, 0);
				//GET mean
				for(int i=0; i<((DataPoint[])C[k]).Length; i++) {
					mean[0].AddValues(((DataPoint[])C[k])[i].GetPoint());
				}
				mean[0].Average();
				
				//GET deviation
				for(int d=0; d<D; d++) {
					for(int i=0; i<((DataPoint[])C[k]).Length; i++) {
						dev[0].GetPoint()[d] += (float)Math.Pow(((DataPoint[])C[k])[i].GetPoint()[d] - mean[0].GetPoint()[d], 2) / (float)((DataPoint[])C[k]).Length;
					}
					if(dev[0].GetPoint()[d] == 0) {
						dev[0].GetPoint()[d] = 0.00001f;
					}
				}
				copymean = (DataPoint[])mean.Clone();
				copydev = (DataPoint[])dev.Clone();
				NB_M[k] = new M_NB(copymean, copydev);
			}
		}
		//TESTING PROCEDURE
		static public void NBClassifying(int N, int D, M_NB[] NB_M, ref DataPoint[] testSet) {
			double Max, p = 0.0;
			int preLabel = 0;
			for(int i=0; i<testSet.Length; i++) {
				Max = double.MinValue;
				for(int k=0; k<NB_M.Length; k++) {
					p = Log_Posterior(NB_M[k].mu[0].GetCount(), N, D, NB_M[k].mu[0].GetPoint(), NB_M[k].dev[0].GetPoint(), testSet[i].GetPoint());
					if(p > Max) {
						Max = p;
						preLabel = k;
					}
				}
				testSet[i].UserLabel = preLabel;
			}
		}
	}
}
