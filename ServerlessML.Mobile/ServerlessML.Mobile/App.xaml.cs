using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ServerlessML.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Views.TaxiTripView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
