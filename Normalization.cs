/*
 * Normalization.cs
 */
using System;
using System.Text;
using System.IO;

namespace CNB {
	public static class Normalization {
		/// <summary>
		/// FORMATTED ARFF FILE.
		/// </summary>
		/// <param name="normal">CHOOSE ONE OF NORAMLIZATION METHOD.</param>
		/// <param name="K">NUMBER OF CLASSES.</param>
		/// <param name="N">NUMBER OF INSTANCES.</param>
		/// <param name="D">NUMBER OF ATTRIBUTES.</param>
		/// <param name="Points">INPUT INSTANCES.</param>
		/// <param name="Class">CLASS ATTRIBUTE</param>
		/// <param name="WriteFileName">WRITE FILE NAME.</param>
		/// <param name="relation">@RELATION FIELD IN FILE HEADER.</param>
		static public void ChangeToNormalFile(int normal, int K, int N, int D, DataPoint[] Points, Nominal Class, string WriteFileName, string relation) {
			switch(normal) {
				case 1:
					MinMaxNormal(N, D, ref Points);
					break;
				case 2:
					ZscoreNormal(N, D, ref Points);
					break;
			}
			var sw = new StreamWriter(WriteFileName);
			sw.WriteLine("@RELATION " + relation);

			var classGroup = new StringBuilder(Class.GetName(0));
			for(int k=1; k<K; k++) {
				classGroup.Append("," + Class.GetName(k));
			}
			for(int d=1; d<=D; d++) {
				sw.WriteLine("@ATTRIBUTE A" + d + "\tREAL");
			}
			sw.WriteLine("@ATTRIBUTE class\t{" + classGroup + "}");
			sw.WriteLine("@DATA");
			for(int i=0; i<N; i++) {
				for(int d=0; d<D; d++) {
					sw.Write("{0},", Points[i].GetPoint()[d]);
				}
				sw.WriteLine(Class.GetName(Points[i].GetClass()));
			}
			sw.Close();
		}
		//MIN-MAX-SCALING NORMALIZATION
		static void MinMaxNormal(int N, int D, ref DataPoint[] Points) {
			float max, min, x, rang;
			for(int d=0; d<D; d++) {
				max = float.MinValue;
				min = float.MaxValue;
				for(int i=0; i<N; i++) {
					x = Points[i].GetPoint()[d];
					if(x > max) {
						max = x;
					}
					if(x < min) {
						min = x;
					}
				}
				rang = max - min;
				for(int i=0; i<N; i++) {
					if(rang == 0) {
						Points[i].GetPoint()[d] = 1;//SET AS 1 IN CASE THE DTH VARIABLE TAKES ONLY ONE VALUE
					} else {
						x = Points[i].GetPoint()[d];
						Points[i].GetPoint()[d] = (x - min) / rang;
					}
				}
			}
		}
		//Z-SCORE NORMALIZATION
		static void ZscoreNormal(int N, int D, ref DataPoint[] Points) {
			float sum, mean, sd;
			for(int d=0; d<D; d++) {
				sum = 0;
				for(int i=0; i<N; i++) {
					sum += Points[i].GetPoint()[d];
				}
				mean = sum / N;
				sd = 0;
				for(int i=0; i<N; i++) {
					sd += 1 / (float)N * (Points[i].GetPoint()[d] - mean) * (Points[i].GetPoint()[d] - mean);
				}
				sd = (float)Math.Sqrt(sd);
				for(int i=0; i<N; i++) {
					if(sd == 0) {
						Points[i].GetPoint()[d] = 0;
						continue;
					}
					Points[i].GetPoint()[d] = (Points[i].GetPoint()[d] - mean) / sd;
				}
			}
		}
	}
}
