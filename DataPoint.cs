/*
 * DataPoint.cs
 */
using System;

namespace CNB {
	// MANAGE DATA POINTS (INSTANCES)
	public class DataPoint {
		//STORE ATTRIBUTE VALUE AS FLOAT TYPE
		public float[] Values;
		int AddCounter;
		// REAL CLASS LABEL
		int Class;
		//REAL SUBCLASS LABEL
		int SubClass;
		// PREDICATION LABEL
		public int UserLabel;
		// Class=-1 REPRESENTS CURRENT DATA POINT BEING UNAVAILABLE
		public DataPoint() {
			AddCounter = 0;
			Class = -1;
			SubClass = -1;
		}
		// CREATE A DATA POINT WITH ITS ATTRIBUTE VALUE ARRAY, CLASS LABEL AND SUBCLASS LABEL
		public DataPoint(int Class, int SubClass, float[] Values) {
			AddCounter = 0;
			this.Class = Class;
			this.SubClass = SubClass;
			this.Values = new float[Values.Length];
			for(int i = 0; i < Values.Length; i++) {
				this.Values[i] = Values[i];
			}
		}
		// CREATE A INITIAL POINT WITH ITS CLASS LABEL AND SUBCLASS LABEL
		public DataPoint(int Class, int SubClass, int Dimensionality, float set) {
			this.Class = Class;
			this.SubClass = SubClass;
			Values = new float[Dimensionality];
			ClearPoint(set);
		}
		// SET EACH ATTTRIBUTE VALUE TO BE 0 
		public void ClearPoint(float set) {
			AddCounter = 0;
			for(int i = 0; i < Values.Length; i++) { 
				Values[i] = set;
			}
		}
		// READ EACH ATTTRIBUTE VALUE
		public bool SetDataPoint(string Record, Nominal ClassName) {
			AddCounter = 0;
			Class = -1;  // RESET CURRENT DATA
			SubClass = -1;

			string[] Items;
			Items = Record.Split(',', ' '); //SPLIT EACH LINE
			if(Items.Length < 2) {//AT LEAST TWO FIELD: CONDITION ATTRIBUTE AND CLASS ATTRIBUTE
				return false;
			}
			Values = new float[Items.Length - 1];// DIMENSIONALITY IS Items.Length - 1
			//
			for(int i = 0; i < Items.Length - 1; i++) {
				if(!float.TryParse(Items[i], out Values[i])) {//CONVERT string TO float
					return false;
				}
			}
			Class = ClassName.AddName(Items[Items.Length - 1]);//CONVERT CLASS ATTRIBUTE TO LABEL BY METHOD IN Nominal.cs
			return Class >= 0; // CONVERT SUCCESSFULLY?
		}
		// ADD ATTRITUBE VALUES
		public bool AddValues(float[] Values) {
			if(Class < 0) {//DATA POINT IS NOT EXISTING
				return false;
			} 
			if(this.Values.Length != Values.Length) {//CHECK WHETHER DIMENSIONALITY IS SAME OR NOT
				return false;
			} 
			for(var i = 0; i < Values.Length; i++) {
				this.Values[i] += Values[i];
			}
			AddCounter++;
			return true;
		}
		// GET AVERAGE
		public bool Average() {
			if(AddCounter < 1) {
				return false;
			} 
			for(var i = 0; i < Values.Length; i++) {
				Values[i] /= AddCounter;
			}
			return true;
		}
		// REAL CLASS LABEL NUMBER
		public int GetClass() {
			return Class;
		}
		//GET SUBCLASS LABEL NUMBER
		public int GetSubClass() {
			return SubClass;
		}
		//SET SUBCLASS FOR DATA POINT
		public void SetSubClass(int SubClass) {
			this.SubClass = SubClass;
		}
		//SET POINT VALUE
		public void SetPoint(float[] Values) {
			this.Values = Values.Clone() as float[];
		}
		// READ DATA POINT'S ATTRIBUTE VALUE
		public float[] GetPoint() {
			return Values;
		}
		// GET NUMBER OF INSTANCES IN A SAME SET (THAT IS |C_k| OR |c_l|)
		public int GetCount() {
			return AddCounter;
		}
	}
}
