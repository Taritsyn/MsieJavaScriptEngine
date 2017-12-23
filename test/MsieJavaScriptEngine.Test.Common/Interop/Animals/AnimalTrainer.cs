namespace MsieJavaScriptEngine.Test.Common.Interop.Animals
{
	public sealed class AnimalTrainer
	{
		public string ExecuteVoiceCommand(IAnimal animal)
		{
			return animal.Cry();
		}
	}
}