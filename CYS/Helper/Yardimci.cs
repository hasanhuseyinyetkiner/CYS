namespace CYS.Helper
{
	public class Yardimci
	{
		public string[] rfidList = new string[]
		{
			"edbe2334ddbaff32","acbe7634ddbaaff2","acbe7639dd3aaff2", "aebe7639dd3ddaa2", "aebe7639337ddaa2"
		};

		public float NextFloat(float min, float max)
		{
			System.Random random = new System.Random();
			double val = (random.NextDouble() * (max - min) + min);
			return (float)val;
		}
	}
}
