using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageCrypt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private WriteableBitmap cbmp;
        private string ctext;

        public MainPage()
        {

            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(700, 500);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ShutdownDialog();
        }

        private void ShutdownDialog()
        {
            ContentDialog exitdialog = new ContentDialog()
            {
                Title = "Beenden",
                Content = "Wollen Sie die App wirklich beenden?",
                PrimaryButtonText = "Ja",
                SecondaryButtonText = "Nein",
            };
            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested +=
            async (sender, args) =>
            {
                args.Handled = true;
                var result = await exitdialog.ShowAsync();
                if (result != ContentDialogResult.Secondary)
                {
                    Application.Current.Exit();
                }
            };
        }

        private async void eExecute_Click(object sender, RoutedEventArgs e)
        {
            string text = eText.Text;

            if (text.Length > 1 && text.Length < Int32.MaxValue)
            {
                ToImage(text);
            }
            else if (text.Length > Int32.MaxValue)
            {
                ContentDialog d = new ContentDialog()
                {
                    Title = "Text Größe",
                    Content = "Der Text darf eine maximale Größe von " + Int32.MaxValue + " Zeichen haben",
                    PrimaryButtonText = "Ok"
                };
                await d.ShowAsync();
            }
        }

        public async void ToImage(string text)
        {
            int[] bounds = getBounds(text.Length - 1);
            int width = bounds[0];
            int height = bounds[1];
            int length = bounds[2];

            WriteableBitmap bmp = new WriteableBitmap(width, height);
            using (bmp.GetBitmapContext())
            {
                int pos = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        int c;
                        char r;
                        char g;
                        char b;
                        if (pos < text.Length-1) { r = text[pos++]; } else { r = '\0'; }
                        if (pos < text.Length-1) { g = text[pos++]; } else { g = '\0'; }
                        if (pos < text.Length-1) { b = text[pos++]; } else { b = '\0'; }
                        c = (r << 16) + (g << 8) + b;
                        bmp.SetPixel(x, y, c);
                    }
                }
            }

            IRandomAccessStream stream = await Convert(bmp);
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);

            cbmp = bmp;
            dImage.Source = bitmap;
            mpivot.SelectedIndex = 1;
        }

        private async Task<IRandomAccessStream> Convert(WriteableBitmap writeableBitmap)
        {
            var stream = new InMemoryRandomAccessStream();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();

            return stream;
        }

        public void ToText(WriteableBitmap bmp)
        {
            StringBuilder sb = new StringBuilder("");
            using (bmp.GetBitmapContext())
            {
                for (int x = 0; x < bmp.PixelWidth; x++)
                {
                    for (int y = 0; y < bmp.PixelHeight; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        char r = (char)c.R;
                        char g = (char)c.G;
                        char b = (char)c.B;
                        sb.Append(r);
                        sb.Append(g);
                        sb.Append(b);
                    }
                }
            }
            string s = sb.ToString();
            eText.Text = s; //Extremly slow --> Find solution
        }

        private int[] getBounds(int length)
        {
            length += 3 - length % 3;
            length /= 3;

            List<int> widthl = new List<int>();
            int height = 0;
            while (widthl.Count() == 0)
            {
                for (int i = 1; i <= length; i++)
                {
                    if (length % i == 0 && (Math.Ceiling((double)i / 3) <= length / i && Math.Ceiling((double)(length / i) / 3) <= i))
                    {
                        widthl.Add(i);
                    }
                }
                if (widthl.Count() == 0) { length += 1; }
            }
            int width = 0;
            int index = (widthl.Count() / 2);
            width = widthl.ElementAt(index);
            height = length / width;
            return new int[] { width, height, length };
        }

        public static async Task<StorageFile> WriteableBitmapToStorageFile(WriteableBitmap bm)
        {
            Guid BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;
            BitmapEncoderGuid = BitmapEncoder.JpegEncoderId;

            var Picker = new FileSavePicker();
            Picker.FileTypeChoices.Add("Image", new List<string>() { ".png" });
            StorageFile file = await Picker.PickSaveFileAsync();

            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoderGuid, stream);
                Stream pixelStream = bm.PixelBuffer.AsStream();
                byte[] pixels = new byte[pixelStream.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)bm.PixelWidth, (uint)bm.PixelHeight, 96, 96, pixels);
                await encoder.FlushAsync();
            }
            return file;
        }

        private void dSave_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmapToStorageFile(cbmp);
        }

        private async void eSave_Click(object sender, RoutedEventArgs e)
        {
            string text = eText.Text;

            var Picker = new FileSavePicker();
            Picker.FileTypeChoices.Add("Text", new List<string>() { ".txt" });
            StorageFile file = await Picker.PickSaveFileAsync();
            if(file == null) { return; }
            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            {
                DataWriter dw = new DataWriter(stream);
                dw.WriteString(text);
                await dw.StoreAsync();
                await dw.FlushAsync();
                dw.DetachStream();
            }
        }

        private void dExecute_Click(object sender, RoutedEventArgs e)
        {
            if(cbmp != null)
            {
                ToText(cbmp);
            }
        }
    }
}
