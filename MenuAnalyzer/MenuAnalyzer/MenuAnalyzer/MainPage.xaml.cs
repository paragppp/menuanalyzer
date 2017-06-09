using Plugin.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace MenuAnalyzer
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            
        }

        private void Take_Photo_Clicked(object sender, EventArgs e)
        {
            //TakePhoto();
            var assembly = typeof(MainPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MenuAnalyzer.UWP.sample.json");

            string input = "";

            using (var reader = new System.IO.StreamReader(stream))
            {
                input = reader.ReadToEnd();
            }

            MenuItems mi = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItems>(input);

            MyList.ItemsSource = mi.menus;
            MyList.IsVisible = true;
        }

        private async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();

            if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
            {
                var mediaOptions = new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "MenuAnalyzer",
                    Name = $"{DateTime.UtcNow}.jpg",
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium

                };

                var file = await CrossMedia.Current.TakePhotoAsync(mediaOptions);

                myActivityIndicator.IsRunning = true;
                myActivityIndicator.IsVisible = true;

                using (var client = new HttpClient())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        byte[] filedata = await (new StreamContent(file.GetStream())).ReadAsByteArrayAsync();

                        var fileContent = new ByteArrayContent(filedata);

                        using (var message = await client.PostAsync(SettingsReader.GetKey("url"), fileContent))
                        {
                            try
                            {
                                string input = await message.Content.ReadAsStringAsync();

                                input = input.Substring(1, input.Length - 2);
                                input = input.Replace("\\", "");

                                MenuItems mi = Newtonsoft.Json.JsonConvert.DeserializeObject<MenuItems>(input);

                                MyList.ItemsSource = mi.menus;
                                MyList.IsVisible = true;

                                myActivityIndicator.IsRunning = false;
                                myActivityIndicator.IsVisible = false;
                            }
                            catch
                            {
                                myActivityIndicator.IsRunning = false;
                                myActivityIndicator.IsVisible = false;
                                await DisplayAlert("Error", "No text was detected or there was a parsing error, please try again!", "Ok");
                            }
                        }
                    }
                }
            }
        }

        private void Clear_Clicked(object sender, EventArgs e)
        {
            MyList.IsVisible = false;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //NavigationPage page = new NavigationPage(new FullImage(((Image)sender).Source));

            Navigation.PushAsync(new FullImage(((Image)sender).Source));
            //DisplayAlert($"image clicked: {sender}","clicked","Ok");
        }
    }
}
