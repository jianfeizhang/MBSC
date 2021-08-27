/*
 * Evaluation.cs
 */
using System;

namespace CNB {
	public static class Evaluation {
		//GET QUALITY OF M_k AND THE TP, TPFP
		static public double Quality(int cls, int D, out DataPoint[] U, DataPoint[] C_k, M_CNB[] M1, DataPoint[] mean, DataPoint[] weight, float[] deviation) {
			double p = 0;
			var tpfp = new int[deviation.Length];//NUMBER OF POSITIVE PREDICTIONS
			var tp = new int[deviation.Length];//NUMBER OF CORRECT POSTIVE PREDICTIONS
			var recall = new double[deviation.Length];
			var predision = new double[deviation.Length];
			for(var l=0; l<tp.Length; l++) {
				tpfp[l] = tp[l] = 0;
				recall[l] = predision[l] = 0.0;
			}
			int n = 0, probably;
			for(int i=0; i<C_k.Length; i++) {
				double Log_MaxM1, Log_MaxMk;
				Log_MaxM1 = Log_MaxMk = double.MinValue;
				for(int k=0; k<M1.Length; k++) {//GET MAX IN M^(1) WITH CLASS BEING BAR_k
					if(k == cls) {//M^(1)_(k)
						continue;
					}
					p = Testing.p_Mk_x(C_k.Length, D, M1[k].CNBdev.Length, out probably, M1[k].CNBmu, M1[k].CNBw, M1[k].CNBdev, C_k[i].GetPoint());
					if(p > Log_MaxM1) {
						Log_MaxM1 = p;
					}
				}
				Log_MaxMk = Testing.p_Mk_x(C_k.Length, D, deviation.Length, out probably, mean, weight, deviation, C_k[i].GetPoint());
				if(Log_MaxMk > Log_MaxM1) {
					n++;
				}

				if(C_k[i].GetSubClass() == probably) {
					tp[probably]++;
					recall[probably] = tp[probably] / (double)mean[probably].GetCount();
				}
				tpfp[probably]++;
				predision[probably] = tp[probably] / (double)tpfp[probably];
			}

			int min = 0;//GET SUBCLASS IINDEX (SAYING "min" BELOW) OF c_l WITH THE LOWEST F(theta_l)
			double f = 0.0, MinValue = double.MaxValue;
			for(int l=0; l<deviation.Length; l++) {
				if(recall[l] + predision[l] == 0) {
					f = 0;
				} else {
					f = 2.0 * recall[l] * predision[l] / (recall[l] + predision[l]);
				}
				if(MinValue > f) {
					MinValue = f;
					min = l;
				}
			}
			//ADD A NEW CENTROID, THIS CENTROID IS AN INSTANCE OF THE c_l WITH THE LOWEST F(theta_l)
			U = (DataPoint[])mean.Clone();
			Array.Resize(ref U, mean.Length + 1);
			double dist, minDist, maxDist;
			int minLabel = 0, maxLabel = 0;
			minDist = double.MaxValue;
			maxDist = double.MinValue;
			for(int i=0; i<C_k.Length; i++) {
				if(C_k[i].GetSubClass() == min) {
					dist = InitializeGaussian.Distance(D, C_k[i].GetPoint(), mean[min].GetPoint());
					if(dist < minDist) {//GET INTANCE  NEAR NEXT TO mean_min OF c_min
						minDist = dist;
						minLabel = i;
					}
					if(dist > maxDist) {//GET INTANCE AWAY FROM mean_min OF c_min
						maxDist = dist;
						maxLabel = i;
					}
				}
			}
			U[min].SetPoint(C_k[maxLabel].GetPoint());//RESET mean_min
			U[mean.Length] = new DataPoint(mean[0].GetClass(), mean.Length, C_k[minLabel].GetPoint());//OPEN UP A NEW SUBCLASS FOR THIS CENTROID

			return  n / (double)C_k.Length;
		}
	}
}
