using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;

using ServerlessML.Mobile.Models;
using ServerlessML.Mobile.Services;

namespace ServerlessML.Mobile.ViewModels
{
    public class TaxiTripViewModel : BaseViewModel
    {
        private TaxiTrip taxiTrip;

        public TaxiTrip TaxiTrip
        {
            get { return taxiTrip; }
            set { SetProperty(ref taxiTrip, value); }
        }

        public ICommand PredictCommand { private set; get; }

        private async Task Predict()
        {
            var prediction = await PredictionService.Predict(TaxiTrip);
            TaxiTrip.FareAmount = prediction.FareAmount;

            await App.Current.MainPage.DisplayAlert(
                "Prediction", 
                $"Trip Fare: {TaxiTrip.FareAmount:C2}", 
                "OK");
        }

        public TaxiTripViewModel()
        {
            TaxiTrip = new TaxiTrip()
            {
                VendorId = "VTS",
                RateCode = "1",
                PassengerCount = 1,
                TripTime = 1140,
                TripDistance = 3.75f,
                PaymentType = "CRD",
                FareAmount = 0
            };

            PredictCommand = new Command(async () => await Predict());
        }
    }
}
