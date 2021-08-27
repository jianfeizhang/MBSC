/*
 * DataFile.cs
 */
using System;
using System.IO;
using System.Collections;

namespace CNB {
	public static class DataFile {
		// ALL INPUT INSTANCES
		static public DataPoint[] Points;
		// #OF INSTANCES
		static public int Number;

		static DataFile() {
			Points = new DataPoint[1];
			Number = 0;
		}
		// READ ARFF FILE AND RETURN THE NUMBER OF DATA POINTS
		// -1: UNEXPECTED FILE FORMAT
		static public void ReadFromARFFData(string DataSetFileName, Nominal Class) {
			Number = 0;
			StreamReader Reader = new StreamReader(DataSetFileName);
			var DataStarted = false;
			DataPoint OnePoint = new DataPoint();
			try {
				//A TEMPORARY DATA POINT
				while(true) {
					var Record = Reader.ReadLine();
					switch(Record) {
						case null:
							break;
					}
					Record = Record.Trim().ToLower();//REMOVE SPACE CHARACTER AND THEN TO BE LOWERCASE
					if(Record.Length < 1 || Record[0] == '%') {
						continue;
					}
					if(!DataStarted) {
						if(Record == "@data") {
							DataStarted = true;
						}
						continue;
					}
					if(!OnePoint.SetDataPoint(Record, Class)) {//STOP IN CASE OF UNEXPECTED FILE FORMAT
						break;
					}
					if(Points.Length < Number + 1) {
						Array.Resize(ref Points, Number + 1);//RESET ARRAY SIZE TO SAVE POINT
					}
					Points[Number++] = new DataPoint(OnePoint.GetClass(), OnePoint.GetSubClass(), OnePoint.GetPoint());//here subclass=-1
				}
				Reader.Close();
			} catch {
			}
		}
		//GET ALL CATEGORIES
		static public ArrayList GetCategories(DataPoint[] Points, int NumOfClasses) {
			var CategorySet = new ArrayList();//EACH ELEMENT IS Points[] SET
			int n;

			for(int k=0; k<NumOfClasses; k++) {
				var C_k = new  DataPoint[1];
				n = 0;
				for(var i=0; i<Points.Length; i++) {
					if(k == Points[i].GetClass()) {
						Array.Resize(ref C_k, n + 1);
						C_k[n++] = new DataPoint(k, 0, Points[i].GetPoint());
					}
				}
				CategorySet.Add(C_k);
			}
			return CategorySet;
		}
	}
}

