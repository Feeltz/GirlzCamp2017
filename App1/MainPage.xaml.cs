using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.Media.Capture;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using System.Linq;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Serwis Azurowy, któremu wysyłamy zdjęcie i pobieramy dane o emocjach - wpisujemy swój klucz z azure
        private readonly EmotionServiceClient emotionServiceClient = new EmotionServiceClient("dc67427c656c41899ccd6ce3891cbb02"); // AZURE

        // zmienna zdjęcia przechowująca zrobione zdjęcie
        public StorageFile photo;
        public MainPage()
        {
            // inicjalizacja okienka
            this.InitializeComponent();
        }

        async void DetectEmotions()
        {
            // tablica emocji - zawiera emocje dla każdej osoby na zdjęciu
            Emotion[] emotionResult;
            var storageFile = photo;

            // zamieniamy zdjęcie na ciąg bitów
            var randomAccessStream = await storageFile.OpenReadAsync();

            // wysyłamy zdjęcie do Azure i dostajemy spowrotem emocje
            emotionResult = await emotionServiceClient.RecognizeAsync(randomAccessStream.AsStream());

            // zapisujemy wyniki do zmiennej
            Microsoft.ProjectOxford.Common.Contract.EmotionScores score = emotionResult[0].Scores;


            // wpisujemy do textboxa wynik emocji w procentach zaokrąglone do 4 miejsc po przecinku
            mojTextbox.Text = "Your Emotions are : \n" +

                "Happiness: " + Math.Round((double)(score.Happiness) * 100, 4) + " %" + "\n" +

                "Sadness: " + Math.Round((double)(score.Sadness) * 100, 4) + " %" + "\n" +

                "Surprise: " + Math.Round((double)(score.Surprise) * 100, 4) + " %" + "\n" +

                "Neutral: " + Math.Round((double)(score.Neutral) * 100, 4) + " %" + "\n" +

                "Anger: " + Math.Round((double)(score.Anger) * 100, 4) + " %" + "\n" +

                "Contempt: " + Math.Round((double)(score.Contempt) * 100, 4) + " %" + "\n" +

                "Disgust: " + Math.Round((double)(score.Disgust) * 100, 4) + " %" + "\n" +

                "Fear: " + Math.Round((double)(score.Fear) * 100, 4) + " %" + "\n";

            // Linq które wybiera najbardziej znaczący wynik, czyli emocję która ma najwięcej punktów
            var lista = score.ToRankedList().FirstOrDefault().Key;

            moj2.Text = (lista.ToString());

        }

        private void guzikFoto_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            zrobFoto();
        }

        private async void zrobFoto()
        {
            
            CameraCaptureUI captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(400, 400);
            captureUI.PhotoSettings.AllowCropping = true;

            // nasza apka czeka, na zrobienie zdjęcia prze aplikację CameraCaptureUI
            photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo == null)
            {
                // Gdy klikniemy cancel na aplikacji do robienia zdjęć
                return;
            }

            // ciąg bitów ze zdjęcia
            IRandomAccessStream stream = await photo.OpenAsync(FileAccessMode.Read);
            // dekodujemy na bitmapę
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
            // zmieniamy rodzaj bitmapy na software
            SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

            // konwertujemy bitmape na taką, która nam odpowiada do tego zadania
            SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap,BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);

            // ustawaimy obiekt typu źródło bitmap i wciskamy tam nasze wcześniej skonwertowane zdjęcie
            SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
            await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);

            // na koniec przypisujemy zdjęcie do kontrolki aby wyświetlić
            imageControl.Source = bitmapSource;
        }

        private void emocje_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            DetectEmotions();
        }
    }



}
