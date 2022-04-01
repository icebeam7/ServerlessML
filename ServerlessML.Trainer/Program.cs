using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.ML;

using ServerlessML.DataModels;

namespace ServerlessML.Trainer
{
    class Program
    {
        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext(seed: 0);
            IDataView dataView;
            ITransformer mlModel;

            mlModel = TrainData(mlContext, out dataView);

            ValidateData(mlContext, mlModel);

            PredictValue(mlContext, mlModel);

            SaveModel(mlContext, mlModel, dataView);
        }

        static ITransformer TrainData(MLContext mlContext, out IDataView dataView)
        {
            dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(
                DataPaths.TrainingData,
                separatorChar: ',',
                hasHeader: true);

            var transforms = mlContext.Transforms;

            var pipeline = transforms.CopyColumns(
                    outputColumnName: "Label",
                    inputColumnName: "FareAmount")
                .Append(transforms.Categorical.OneHotEncoding(
                    outputColumnName: "VendorIdEncoded",
                    inputColumnName: "VendorId"))
                .Append(transforms.Categorical.OneHotEncoding(
                    outputColumnName: "RateCodeEncoded",
                    inputColumnName: "RateCode"))
                .Append(transforms.Categorical.OneHotEncoding(
                    outputColumnName: "PaymentTypeEncoded",
                    inputColumnName: "PaymentType"))
                .Append(transforms.Concatenate(
                    "Features",
                    "VendorIdEncoded",
                    "RateCodeEncoded",
                    "PassengerCount",
                    "TripDistance",
                    "PaymentTypeEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());

            var model = pipeline.Fit(dataView);
            return model;
        }

        static void ValidateData(MLContext mlContext, ITransformer mlModel)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<TaxiTrip>(
                DataPaths.ValidationData,
                separatorChar: ',',
                hasHeader: true);

            var predictions = mlModel.Transform(dataView);

            var metrics = mlContext.Regression.Evaluate(
                predictions,
                "Label",
                "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation        ");
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");
        }

        static void PredictValue(MLContext mlContext, ITransformer mlModel)
        {
            var sampleTrip = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0 // To predict. Actual/Observed = 15.5
            };

            var predictionFunction = mlContext.Model
                .CreatePredictionEngine<TaxiTrip, TaxiTripFarePrediction>(mlModel);
            
            var prediction = predictionFunction.Predict(sampleTrip);

            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Value prediction with new data          *");
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"       Predicted fare: {prediction.FareAmount:0.####}, actual fare: 15.5");
        }

        static void SaveModel(MLContext mlContext, ITransformer mlModel, IDataView dataView)
        {
            mlContext.Model.Save(mlModel, dataView.Schema, DataPaths.OutputMLModel);
        }
    }
}

//using Microsoft.WindowsAzure.Storage;
//await UploadModel();
/*
private static async Task UploadModel(string file)
{
    string connectionString = "";
    var storageAccount = CloudStorageAccount.Parse(connectionString);

    var client = storageAccount.CreateCloudBlobClient();
    var container = client.GetContainerReference("data");

    var blob = container.GetBlockBlobReference("Model.zip");
    await blob.UploadFromFileAsync(file);
}*/
