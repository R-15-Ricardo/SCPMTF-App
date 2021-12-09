using Newtonsoft.Json;
using scpmtf_app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace scpmtf_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPage : ContentPage
    {
        int pageItemNo;
        bool inmutableSummary = false;
        ScpGpt3Summary currentSummary;
        string redacted = "https://pm1.narvii.com/7662/94498bc3acddab1063be483b8eb2d32d3341df94r5-500-512_00.jpg";
        public InfoPage(int itNo)
        {
            InitializeComponent();
            CargarSCP(itNo);
        }

        private async void CargarSCP(int itNo)
        {
            pageItemNo = itNo;

            string scpUrl = $"https://scpmtf-app.uc.r.appspot.com/api/items/scp-{itNo}";
            string summaryUrl = $"https://scpmtf-app.uc.r.appspot.com/api/summary/scp-{itNo}";

            HttpClient client = new HttpClient();

            var Result = await client.GetAsync(scpUrl);
            if (!Result.IsSuccessStatusCode)
            {
                MainInfo.Children.Clear();
                MainInfo.Children.Add(new Label { Text = "[ACCESSO DENEGADO]", TextColor = Color.Red, VerticalOptions = LayoutOptions.CenterAndExpand,HorizontalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold, FontSize = 35});
                MainInfo.Children.Add(new Label { Text = "Solicite accesso con su director de sitio.", TextColor = Color.Red, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold, FontSize = 15 });
                return;
            }
            ScpObject scp = JsonConvert.DeserializeObject<ScpObject>(Result.Content.ReadAsStringAsync().Result);

            Result = await client.GetAsync(summaryUrl);
            if (!Result.IsSuccessStatusCode)
            {
                MainInfo.Children.Clear();
                MainInfo.Children.Add(new Label { Text = "ERROR INTERNO > [Engine GPT-3:DaVinci]", TextColor = Color.Red, HorizontalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.Bold, FontSize = 25 });
                MainInfo.Children.Add(GenerarInfo(scp));
                return;
            }
            ScpGpt3Summary summary = JsonConvert.DeserializeObject<ScpGpt3Summary>(Result.Content.ReadAsStringAsync().Result);
            currentSummary = summary;

            MainInfo.Children.Clear();
            MainInfo.Children.Add(GenerarInfo(scp,summary));
        }

        private Grid GenerarInfo(ScpObject item)
        {
            Grid scpInfo = new Grid {
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(4, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(4, GridUnitType.Star) }
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                Padding = new Thickness(20, 20, 20, 20)
            };

            scpInfo.Children.Add(new Label { Text = $"SCP - {item.ItemNo}" }, 0, 0);
            scpInfo.Children.Add(new Label { Text = $"Clase del objeto: {item.ObjectClass}" }, 0, 1);
            scpInfo.Children.Add(new Image { Source = (item.ImageAttachment == "") ? redacted : item.ImageAttachment, WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.Fill }, 0, 2);
            scpInfo.Children.Add(new Label { Text = $"Descripción: {item.Description}" }, 0, 3);

            return scpInfo;
        }

        private Layout GenerarInfo(ScpObject item, ScpGpt3Summary mtfsummary)
        {
            Grid scpInfo = new Grid
            {
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(3, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(6, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(4, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(3, GridUnitType.Star) }
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                },
                HorizontalOptions = LayoutOptions.Center,
                Padding = new Thickness(25, 10, 25, 10),
                Margin = new Thickness(0, 0, 0, 15),
            };

            Frame headerFrame = new Frame { BackgroundColor = Color.FromHex("#26FFFFFF"), Padding = new Thickness(5,3,5,3), HasShadow=true};
            StackLayout header = new StackLayout { VerticalOptions = LayoutOptions.CenterAndExpand, Padding = new Thickness(10,5,10,5)};
            header.Children.Add(new Label { Text = $"SCP - {item.ItemNo}", FontAttributes = FontAttributes.Bold, FontSize = 25, TextColor = Color.White, TextDecorations = TextDecorations.Underline});
            header.Children.Add(new Label { Text = $"Clase del objeto: {item.ObjectClass}", FontSize = 20, TextColor = Color.White });
            headerFrame.Content = header;

            Frame summaryContainer = new Frame { HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor=Color.LightYellow};
            if (mtfsummary.Id != 0)
            {
                inmutableSummary = true;
                summaryContainer.BackgroundColor = Color.LightGray;
            }
            summaryContainer.Content = new Label { Text = mtfsummary.CombatSummary, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 15 };

            var swipeSummary = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
            var tapSummary = new TapGestureRecognizer();
            swipeSummary.Swiped += SwipeSummary_Swiped;
            tapSummary.Tapped += TapSummary_Tapped;
            summaryContainer.GestureRecognizers.Add(tapSummary);
            summaryContainer.GestureRecognizers.Add(swipeSummary);

            scpInfo.Children.Add(headerFrame, 0, 0);
            scpInfo.Children.Add(new ActivityIndicator { Color = Color.LightGray, IsVisible = true, IsRunning = true, Scale = 0.5 }, 0, 1);
            scpInfo.Children.Add(new Image { Source = (item.ImageAttachment == "") ? redacted : item.ImageAttachment, WidthRequest = 300, HeightRequest = 300, Aspect = Aspect.Fill}, 0, 1);
            scpInfo.Children.Add(summaryContainer, 0, 2);
            scpInfo.Children.Add(new ScrollView { Content = new Label { Text = $"Descripción: {item.Description}", FontSize = 15, TextColor = Color.White } }, 0, 3);

            return scpInfo;
        }

        private async void TapSummary_Tapped(object sender, EventArgs e)
        {
            if (inmutableSummary) return;

            Frame container = (Frame)sender;
            bool answer = await DisplayAlert("Confirmación de DM", "¿Confirma que la información de combate es acertada?", "Aceptar y Enviar", "Decartar");

            if (!answer) return;

            string toSend = JsonConvert.SerializeObject(currentSummary);
            var content = new StringContent(toSend, Encoding.UTF8, "application/json");
            string summaryUrl = $"https://scpmtf-app.uc.r.appspot.com/api/summary";

            HttpClient client = new HttpClient();
            var lastContent = container.Content;
            container.BackgroundColor = Color.DarkGray;
            container.Content = new ActivityIndicator { Color = Color.LightGray, IsVisible = true, IsRunning = true, Scale = 0.5 };
            var Result = await client.PostAsync(summaryUrl, content);

            if (!Result.IsSuccessStatusCode)
            {
                await DisplayAlert("Error de servidor", "No se pudo enviar el reporte", "Aceptar");
                container.Content = lastContent;
                return;
            }

            container.Content = lastContent;
            container.BackgroundColor = Color.LightGray;
            inmutableSummary = true;
        }

        private async void SwipeSummary_Swiped(object sender, SwipedEventArgs e)
        {
            if (inmutableSummary) return;

            Frame container = (Frame)sender;

            container.Content = new ActivityIndicator { Color = Color.Gray, IsVisible = true, IsRunning = true, Scale=0.5};

            string summaryUrl = $"https://scpmtf-app.uc.r.appspot.com/api/summary/scp-{pageItemNo}";

            HttpClient client = new HttpClient();

            var Result = await client.GetAsync(summaryUrl);
            if (!Result.IsSuccessStatusCode)
                container.Content = new Label { Text = "ERROR INTERNO > [Engine GPT-3:DaVinci]", TextColor = Color.Red, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 15 }; ;

            ScpGpt3Summary summary = JsonConvert.DeserializeObject<ScpGpt3Summary>(Result.Content.ReadAsStringAsync().Result);
            currentSummary = summary;
            container.Content = new Label { Text = summary.CombatSummary, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 15 };
        }
    }
}