using System.IO;

namespace ServerlessML.Trainer
{
    public static class DataPaths
    {
        public static readonly string DataFolder = @"C:\Xamarin\Demos\NETDocsShow\ServerlessML\Data";
        public static readonly string TrainingData = Path.Combine(DataFolder, "taxi-fare-train.csv");
        public static readonly string ValidationData = Path.Combine(DataFolder, "taxi-fare-test.csv");
        public static readonly string OutputMLModel = Path.Combine(DataFolder, "MLModel.zip");
    }
}
