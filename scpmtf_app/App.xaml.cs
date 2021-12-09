using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace scpmtf_app
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage()) { BarBackgroundColor = Color.Black, HeightRequest = 10 };
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
