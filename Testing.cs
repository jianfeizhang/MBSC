/*
 * Testing.cs
 */
using System;
using System.Collections.Generic;

namespace CNB {
	public static class Testing {

		//TESTING PROCEDURE
		static public void Classifying(int N, int D, M_CNB[] M, ref DataPoint[] testSet) {
			double Max, p = 0.0;
			int preLabel = 0, proSubCls;
			for(int i=0; i<testSet.Length; i++) {
				Max = double.MinValue;
				for(int k=0; k<M.Length; k++) {
					p = p_Mk_x(N, D, M[k].CNBdev.Length, out proSubCls, M[k].CNBmu, M[k].CNBw, M[k].CNBdev, testSet[i].GetPoint());
					if(p > Max) {
						Max = p;
						preLabel = k;
					}
				}
				testSet[i].UserLabel = preLabel;
			}
		}
		//GET RELATIVE ln(p(M_k|x))
		static public double p_Mk_x(int N, int D, int ell_k, out int proSubCls, IList<DataPoint> u, IList<DataPoint> w, IList<float> dev, float[] x) {
			double p = 0, max = double.MinValue;
			proSubCls = -1;
			for(int l=0; l<ell_k; l++) {
				p = ModelLearning.ReLog_Posterior(u[l].GetCount(), N, D, u[l].GetPoint(), w[l].GetPoint(), dev[l], x);
				if(p > max) {
					max = p;
					proSubCls = l;
				}
			}
			return max;
		}
		//GET SIZE OF EACH AFTER-PREDICATED CLASS, RATHER THAN THE TRUE CLASS SIZE
		static int[] GetUserClassSize(int K, int N, IList<DataPoint> test) {
			var size = new int[K];
			for(int k=0; k<K; k++) {
				size[k] = 0;
			}
			for(int i=0; i<N; i++) {
				size[test[i].UserLabel]++;
			}
			return size;
		}
		//RETURN F1-MEASURE
		static double F1Measure(int K, int TrueLabel, DataPoint[] Points, IList<int> userClassSize, out int ClassSize) {
			double[] F1 = new double[K]; 

			for(int k = 0; k < K; k++) {
				F1[k] = 0;
			}

			ClassSize = 0;
			int N = Points.Length;//TESTING DATA SIZE
			for(int i = 0; i < N; i++) {
				if(Points[i].GetClass() != TrueLabel) {
					continue;
				}
				ClassSize++;
				F1[Points[i].UserLabel] += 1.0;
			}

			// COMPUTE RECALL AND PRECISION IN TERMS OF EACH CLASS THEN RETURN F1-MEASURE
			double recall, precision;
			for(int k = 0; k < K; k++) {
				if(F1[k] > 0) {
					recall = F1[k] / ClassSize;
					precision = F1[k] / userClassSize[k];
					F1[k] = 2.0 * recall * precision / (precision + recall);
				}
			}
			// SEARCH FOR MAX F1
			double MaxF1 = 0;
			for(int k = 0; k < K; k++) {
				if(F1[k] > MaxF1) { 
					MaxF1 = F1[k];
				} 
			}
			return MaxF1;
		}
		// COMPUTE F-SCORE
		static public void FScore(int K, DataPoint[] Points, Nominal Class) {
			var N = Points.Length;//TESTING SET SIZE
			var userClassSize = GetUserClassSize(K, N, Points);
			int ClassSize;   // TRUE CLASS SIZE
			double F1, fscore = 0;

			for(int k = 0; k < K; k++) {
				F1 = F1Measure(K, k, Points, userClassSize, out ClassSize);
				Console.WriteLine("Class {0}: Size={1}, F1={2:F4}", Class.GetName(k), ClassSize, F1);
				fscore += (double)ClassSize / N * F1;
			}
			Console.WriteLine("FScore={0:F4}", fscore);
		}
		//RETURN F1_k(k=TrueLabel) AND GET recall_k, precision_k
		static double F1Measure(int TrueLabel, DataPoint[] test, IList<int> UserClassSize, out int ClassSize, out double recall, out double precision) {
			double F1 = 0;
			ClassSize = 0;
			for(int i=0; i<test.Length; i++) {
				if(test[i].GetClass() == TrueLabel) {
					ClassSize++;
					if(test[i].GetClass() == test[i].UserLabel) {
						F1 += 1.0;
					}
				}
			}
			recall = F1 / ClassSize;
			if(UserClassSize[TrueLabel] == 0) {
				precision = 0;
			} else {
				precision = F1 / UserClassSize[TrueLabel];
			}
			if(recall + precision == 0) {
				return 0;
			}
			{
				return 2.0 * recall * precision / (recall + precision);
			}
		}
		//GET Micro-F1 AND Macro-F1
		static public void MicroMacro(int K, DataPoint[] Te) {
			var UserClassSize = GetUserClassSize(K, Te.Length, Te);
			int ClassSize;
			double MacroF1, MicroF1, recall, precision, sumRec, sumPre;
			sumRec = sumPre = MacroF1 = MicroF1 = 0;
			for(int k=0; k<K; k++) {
				if(F1Measure(k, Te, UserClassSize, out ClassSize, out recall, out precision) == 0) {
					MacroF1 += 0;
				} else {
					MacroF1 += 1.0 / K * F1Measure(k, Te, UserClassSize, out ClassSize, out recall, out precision);
				}
				sumRec += 1.0 / K * recall;
				if(precision == 0) {
					sumPre += 0;
				} else {
					sumPre += 1.0 / K * precision;
				}
			}
			MicroF1 = 2.0 * sumRec * sumPre / (sumRec + sumPre);
			Console.WriteLine("MicroF1={0:F4}\nMacroF1={1:F4}", MicroF1, MacroF1);
		}
	}
}

