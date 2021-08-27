/*
   * Nominal.cs
   */
namespace CNB {
	// CONVERT nominal ATTRIBUTE TO int
	public class Nominal {
		//ASSUME 10 LABELS, 0-9
		const int MaxNominalName = 10;
		string[] Names;
		// CURRENT NUMBER OF NAMES
		int Number;

		public Nominal() {
			//INITILIZATION
			Names = new string[MaxNominalName];
			Number = 0;
		}
		//ADD A NAME AND RETURN ITS NUMBER
		public int AddName(string NewName) {
			//REMOVE SPACE CHARACTER
			NewName = NewName.Trim();
			//NewName EXISTS IN Names OR NOT?
			for(int i = 0; i < Number; i++) {
				if(Names[i] == NewName) {//RETURN NUMBER 
					return i;
				}
			}
			if(Number >= MaxNominalName) {//FAIL TO ADD NAME
				return -1;
			}
			Names[Number] = NewName;
			return Number++; //RETURN THE NUMBER OF LABELS
		}
		// CHECK NAME ACCORDING TO NUMBER
		public string GetName(int Code) {
			if(Code < 0 || Code >= Number) {//ERROR
				return "";
			} 
			return Names[Code];
		}
		// NUMBER OF CLASSES' LABELS
		public int GetNumber() {
			return Number;
		}
	}
}
