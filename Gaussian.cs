/*
 * Gaussian.cs
 */
using System;
using System.Collections.Generic;

namespace CNB {
	public static class InitializeGaussian {
		//EUCLIDEAN DISTANCE BETWEEN TWO INTANCES
		static public double Distance(int D, IList<float> p1, IList<float> p2) {
			double dist = 0;
			for(int d = 0; d< D; d++) {
				dist += (p1[d] - p2[d]) * (p1[d] - p2[d]);
			}
			return Math.Sqrt(dist);
		}
		//WITHIN CLASS k, ASSIGN EACH POINT TO THE NEAREST CENTROID.
		static void Assignment(int ell_k, int D, ref DataPoint[] Points, IList<DataPoint> OldCentroid, IList<DataPoint> NewCentroid) {
			double MinDist, dist;
			int nearest = 0;
			var NumPoints = Points.Length;//NUMBER OF POINTS
			for(int i=0; i<NumPoints; i++) {//FOR EACH POINT
				MinDist = double.MaxValue;
				for(int l=0; l<ell_k; l++) {//FOR EACH CENTROID
					dist = Distance(D, Points[i].GetPoint(), OldCentroid[l].GetPoint());
					if(dist < MinDist) {
						MinDist = dist;
						nearest = l;
					}
				}
				Points[i].SetSubClass(OldCentroid[nearest].GetSubClass());//SET THIS POINT'S SUBCLASS LABEL TO THE NEAREST CENTROID
				NewCentroid[nearest].AddValues(Points[i].GetPoint());
			}
		}
		//CHECK \ell_k CENTROIDS WHETHER TO BE CHANGED
		static public bool PointsChanged(int D, DataPoint[] p1, IList<DataPoint> p2) {
			float changed, Maxchanged;
			changed = Maxchanged = 0;
			for(int l=0; l<p1.Length; l++) {
				for(int d=0; d<D; d++) {
					changed = Math.Abs(p1[l].GetPoint()[d] - p2[l].GetPoint()[d]);
					if(changed > Maxchanged) {
						Maxchanged = changed;
					}
				}
			}
			return Maxchanged > 0.0001;
		}
		//CLUSTERING
		static public void Clustering(int k, int ell_k, int D, IList<DataPoint> U, ref DataPoint[] C_k) {
			var v = new DataPoint[ell_k];
			var newv = new DataPoint[ell_k];
			int l, iterations;

			for(l=0; l<ell_k; l++) {//COPY U TO newv
				newv[l] = new DataPoint(U[l].GetClass(), U[l].GetSubClass(), U[l].GetPoint());
			}

			for(l=0; l<ell_k; l++) {
				v[l] = new DataPoint(k, l, D, 0);
			}

			iterations = 0;//ITERATIONS
			while(PointsChanged(D,v,newv)) {
				iterations++;
				for(l=0; l<ell_k; l++) {
					v[l] = new DataPoint(newv[l].GetClass(), newv[l].GetSubClass(), newv[l].GetPoint());
					newv[l].ClearPoint(0);//SET EACH ENTRY OF newv TO BE 0
				}
				Assignment(ell_k, D, ref C_k, v, newv);
				for(l=0; l<ell_k; l++) {
					newv[l].Average();
				}
			}
		}
	}
}
