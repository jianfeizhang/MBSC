/*
  * Model.cs
  */
namespace CNB {
	//"M_CNB"MODEL FOR CNB
	public class M_CNB {
		public DataPoint[] CNBmu, CNBw;
		public float[] CNBdev;

		public M_CNB(DataPoint[] CNBmu, DataPoint[] CNBw, float[] CNBdev) {
			this.CNBmu = CNBmu;
			this.CNBw = CNBw;
			this.CNBdev = CNBdev;
		}
	}
	//"M_NB" MODEL FOR NB
	public class M_NB {
		public DataPoint[] mu, dev;

		public M_NB(DataPoint[] mu, DataPoint[] dev) {
			this.mu = mu;
			this.dev = dev;
		}
	}
}