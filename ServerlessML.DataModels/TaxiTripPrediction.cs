using Microsoft.ML.Data;

namespace ServerlessML.DataModels
{
    public class TaxiTripFarePrediction
    {
        [ColumnName("Score")]
        public float FareAmount;
    }
}
