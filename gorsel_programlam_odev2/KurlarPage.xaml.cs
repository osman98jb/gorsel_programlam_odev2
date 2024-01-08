using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace gorsel_programlam_odev2
{
    public partial class KurlarPage : ContentPage
    {
        ObservableCollection<CurrencyInfo> currencies;

        public KurlarPage()
        {
            InitializeComponent();
            currencies = new ObservableCollection<CurrencyInfo>();
            currencyListView.ItemsSource = currencies;
            LoadData();
        }

        private void OnDarkModeToggled(object sender, ToggledEventArgs e)
        {
            App.ToggleTheme(e.Value);
        }

        private async void LoadData()
        {
            try
            {
                HttpClient client = new HttpClient();
                string json = await client.GetStringAsync("https://api.genelpara.com/embed/altin.json");

                if (!string.IsNullOrWhiteSpace(json))
                {
                    JObject jsonRoot = JsonConvert.DeserializeObject<JObject>(json);

                    foreach (var currency in jsonRoot)
                    {
                        JObject currencyData = (JObject)currency.Value;

                        double degisim = Convert.ToDouble(currencyData["degisim"]);
                        string changeLabel;
                        string changeImageSource;

                        if (degisim > 0)
                        {
                            changeLabel = "Pozitif Degisim";
                            changeImageSource = "up.png";
                        }
                        else if (degisim < 0)
                        {
                            changeLabel = "Negatif Degisim";
                            changeImageSource = "down.png";
                        }
                        else
                        {
                            changeLabel = "Degisiklik yok";
                            changeImageSource = "orta.jpeg";
                        }

                        currencies.Add(new CurrencyInfo
                        {
                            Type = currency.Key,
                            Buying = $"Alis: {currencyData["alis"]}",
                            Selling = $"Satis: {currencyData["satis"]}",
                            Situation = $"Degisim: {currencyData["degisim"]} ({currencyData["d_oran"]}%)",
                            ChangeLabel = changeLabel,
                            ChangeImageSource = changeImageSource
                        });
                    }
                }
                else
                {
                    Console.WriteLine("Empty or invalid JSON response.");
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void Refresh_Clicked(object sender, EventArgs e)
        {
            currencies.Clear();
            LoadData();

            DisplayAlert("basarili", "Doviz kurlari basariyla yenilendi.", "tamam");

        }

    }

}
