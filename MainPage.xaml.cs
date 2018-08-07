using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

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

        public UIElement ImageControl { get; private set; }

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
            string text = "";
            eText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);

            if (text.Length > 1 && text.Length < 2147483647)
            {
                ToImage(text);
            }
            else if (text.Length < 2147483647)
            {
                ContentDialog d = new ContentDialog()
                {
                    Title = "Text Größe",
                    Content = "Der Text darf eine maximale Größe von " + text.Length + " Zeichen haben",
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
            dImage.Source = bmp;
            cbmp = bmp;
            mpivot.SelectedIndex = 1;
        }

        public async void ToText(WriteableBitmap bmp)
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
            eText.Document.SetText(TextSetOptions.None, sb.ToString());
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
            string text = "";
            eText.Document.GetText(Windows.UI.Text.TextGetOptions.None, out text);

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
