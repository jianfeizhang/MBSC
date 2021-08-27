/*
 * Parameter.cs
 */
using System;
using System.Collections.Generic;

namespace CNB {
	public class Estimation {
		static double sumWeight, alpha;

		public Estimation(double sum, double Alpha) {
			sumWeight = sum;
			alpha = Alpha;
		}
		//WHETHER TO BE CHANGED
		public static bool PointChanged(int D, IList<float> p1, IList<float> p2) {
			double changed = 0, Maxchanged = 0;
			for(int d=0; d<D; d++) {
				changed = Math.Abs(p1[d] - p2[d]);
				if(changed > Maxchanged) {
					Maxchanged = changed;
				}
			}
			return changed > 0.0001;
		}
		//RETURE WEIGHT VALUE
		static double Weighting(int l, int n_l, int D, ref float[] deviation, ref DataPoint[] weight, float[] X) {
			deviation[l] = 0;
			for(int d=0; d<D; d++) {
				deviation[l] += weight[l].GetPoint()[d] * X[d] / (float)(D * n_l);//UPDATE deviation[l][d]
			}
			if(deviation[l] == 0) {
				deviation[l] = 0.000001f;
			}
			double w, fraction, maxW = double.MinValue;
			double lambda = NewtonDownhill(D, n_l, l, X, deviation);//GET LAMBDA
			for(int d=0; d<D; d++) {
				fraction = X[d] / (deviation[l] * n_l);
				w = (2.0 * alpha - 1.0) / (lambda + fraction);
				if(w < 0) {
					Console.WriteLine("negative weight");
				}
				weight[l].GetPoint()[d] = (float)w;
				if(w > maxW) {
					maxW = w;
				}
			}
			return maxW;
		}

		static void GetX(int n_l, int l, int D, ref float[] X, IList<DataPoint> mean, IList<DataPoint> Points) {
			for(int d=0; d<D; d++) {//GET X_{ld} OF THE lth SUBCLASS
				X[d] = 0;
				for(int i=0; i<n_l; i++) {
					if(l == Points[i].GetSubClass()) {
						X[d] += (float)Math.Pow(Points[i].GetPoint()[d] - mean[l].GetPoint()[d], 2);//NOTATION "X" USED IN PAPER
					}
				}
			}
		}
		//GET all \theta OF M_k
		public static void GetParameters(int k, int ell_k, int D, DataPoint[] Points, out DataPoint[] u, out DataPoint[] w, out float[] dev) {
			var mean = new DataPoint[ell_k];
			var weight = new DataPoint[ell_k];
			var deviation = new float[ell_k];
			var X = new float[D];
			int n_l, steps;
			double oldW, newW;
			//SET TO AN INSTANCE OF AN OBJECT
			for(int l=0; l<ell_k; l++) {
				mean[l] = new DataPoint(k, l, D, 0);
				weight[l] = new DataPoint(k, l, D, (float)(sumWeight / D));//INITIALIZE EACH WEIGHT AS sumWeight/D
			}

			for(int i=0; i<Points.Length; i++) {//COMPUTE THE mean[] ACCORDING TO SUBCLASS LABELS
				mean[Points[i].GetSubClass()].AddValues(Points[i].GetPoint());
			}

			for(int l=0; l<ell_k; l++) {
				mean[l].Average();//GET MEAN OF THE lth SUBCLASS

				n_l = mean[l].GetCount();
				GetX(n_l, l, D, ref X, mean, Points);

				steps = 0;
				newW = -1;
				do {
					steps++;
					oldW = newW;
					newW = Weighting(l, n_l, D, ref deviation, ref weight, X);
				} while(Math.Abs(oldW-newW)>1e-4);
			}

			u = mean.Clone() as DataPoint[];
			w = weight.Clone() as DataPoint[];
			dev = deviation.Clone() as float[];
		}

		static double func(int D, int n_l, int l, IList<float> X, IList<float> deviation, double lambda) {
			double sum = sumWeight;
			for(int d=0; d<D; d++) {
				sum -= (2.0 * alpha - 1.0) / (lambda + X[d] / (n_l * deviation[l]));
			}
			return  sum;
		}

		static double df(int D, int n_l, int l, IList<float> X, IList<float> deviation, double lambda) {
			double sum = 0;
			for(int d=0; d<D; d++) {
				sum += (2.0 * alpha - 1.0) / Math.Pow(lambda + X[d] / (n_l * deviation[l]), 2);
			}
			return sum;
		}
		//NEWTON-DOWNHILL METHOD
		static double NewtonDownhill(int D, int n_l, int l, float[] X, float[] deviation) {
			int iteriations = 0, h = 0;
			double root1 = 1, root2 = 0, root = 0.0, f1, f2, diff, down = 1.0;
			if(df(D, n_l, l, X, deviation, root1) == 0) {
				Console.Write("error");
			}
			f1 = func(D, n_l, l, X, deviation, root1);
			while(Math.Abs(f1)>1e-5) {
				if(iteriations > 100) {
					Console.WriteLine("iteriations>100");
					break;
				}
				do {
					h++;
					if(h > 50) {
						Console.WriteLine("h>50");
						break;
					}
					f1 = func(D, n_l, l, X, deviation, root1);
					diff = df(D, n_l, l, X, deviation, root1);
					if(diff == 0) {
						Console.WriteLine("diff=0");
						break;
					}
					root2 = root1 - down * (f1 / diff);
					root = root1;
					root1 = root2;
					f2 = func(D, n_l, l, X, deviation, root2);
					down = down / 2.0;
				} while(Math.Abs(f2)>Math.Abs(f1)) ;
				iteriations++;
				down = 1.0;
			}
			return root;
		}
	}
}

