using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;

using Newtonsoft.Json;

using ServerlessML.Mobile.Models;

namespace ServerlessML.Mobile.Services
{
    public static class PredictionService
    {
        const string PredictionEndpoint = "https://serverlessmlpredictionapi.azurewebsites.net";
        const string TaxiFarePredictionService = "api/PredictionAPI";
        const string FunctionCode = "xDBr1oIOYW1TJEiI9CF/GSy34IsVNArtadYQY72IBAmnTBDgG/SvAw==";

        private static readonly HttpClient client = CreateHttpClient();

        private static HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(PredictionEndpoint);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient;
        }

        public async static Task<TaxiTripPrediction> Predict(TaxiTrip trip)
        {
            try
            {
                var json = JsonConvert.SerializeObject(trip);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var url = $"{TaxiFarePredictionService}?code={FunctionCode}";

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var prediction = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TaxiTripPrediction>(prediction);
                }
            }
            catch (Exception ex)
            {
            }

            return default;
        }
    }
}