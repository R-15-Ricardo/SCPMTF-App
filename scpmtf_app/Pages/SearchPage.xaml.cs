using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace scpmtf_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        Regex rgx;
        public SearchPage()
        {
            InitializeComponent();
            rgx = new Regex("^[0-9]*$");
        }

        /*protected override void OnAppearing()
        {
            InitializeComponent();
        }*/

        private async void searchSendBtn_Clicked(object sender, EventArgs e)
        {
            if (itemNumberEntry.Text == null) return;
            if (itemNumberEntry.Text == "") return;
            if (!rgx.IsMatch(itemNumberEntry.Text))
            {
                MainContent.Children.Add(new Label { Text = "Ingrese un identificador válido.", TextColor = Color.Red });
                return;
            }

            await Navigation.PushAsync(new InfoPage(int.Parse(itemNumberEntry.Text)));
        }
    }
}