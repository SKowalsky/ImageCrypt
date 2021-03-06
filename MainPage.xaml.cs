﻿using System;
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
using Windows.Storage.FileProperties;
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
        private LanguageManager manager;

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(700, 500);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            manager = new LanguageManager();
            GetLanguage();
        }

        private void SetLanguage(Lang Language)
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            localSettings.Values["language"] = Language.ToString();
        }

        private void GetLanguage()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            Object value = localSettings.Values["language"];
            Lang lang;
            if(value != null)
            {
                string s = (string)value;
                lang = s.Equals("English") ? Lang.English : s.Equals("German") ? Lang.German : Lang.English;
            } else
            {
                lang = Lang.English;
            }
            LanguageManager.Language = lang;
            LanguageManager.SetText();
            this.DataContext = null;
            this.DataContext = manager;
        }

        private async void eExecute_Click(object sender, RoutedEventArgs e)
        {
            string text = eText.Text;
            if (text.Length > 0 && text.Length < Int32.MaxValue)
            {
                TextBox tb = new TextBox();
                if(await LanguageManager.GetKeyInputDialog(tb).ShowAsync() != ContentDialogResult.Secondary)
                {
                    string key = tb.Text;
                    text = Crypto.Encrypt(text, key);
                    if(text != null)
                    {
                        ToImage(text);
                    } else
                    {
                        await LanguageManager.GetEncryptionErrorDialog().ShowAsync();
                    }
                }
            }
            else if (text.Length > Int32.MaxValue)
            {
                await LanguageManager.GetMaxTextSizeDialog().ShowAsync();
            }
            else if(text.Length == 0)
            {
                await LanguageManager.GetNoTextDialog().ShowAsync();
            }
        }

        private async void dExecute_Click(object sender, RoutedEventArgs e)
        {
            if (cbmp != null)
            {
                ToText(cbmp);
            }
            else
            {
                await LanguageManager.GetNoImageDialog().ShowAsync();
            }
        }

        private async void eSave_Click(object sender, RoutedEventArgs e)
        {
            string text = eText.Text;
            if (text.Length > 0)
            {
                var Picker = new FileSavePicker();
                Picker.FileTypeChoices.Add("Text", new List<string>() { ".txt" });
                StorageFile file = await Picker.PickSaveFileAsync();
                if (file != null) {
                    using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        DataWriter dw = new DataWriter(stream);
                        dw.WriteString(text);
                        await dw.StoreAsync();
                        await dw.FlushAsync();
                        dw.DetachStream();
                    }
                }
            }
            else
            {
                await LanguageManager.GetNoTextDialog().ShowAsync();
            }
        }

        private async void dSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbmp != null)
            {
                await WriteableBitmapToStorageFile(cbmp);
            }
            else
            {
                await LanguageManager.GetNoImageDialog().ShowAsync();
            }
        }

        private async void eOpen_Click(object sender, RoutedEventArgs e)
        {
            var Picker = new FileOpenPicker();
            Picker.FileTypeFilter.Add(".txt");
            StorageFile file = await Picker.PickSingleFileAsync();
            if (file != null) {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    StreamReader reader = new StreamReader(stream.AsStreamForRead());
                    eText.Text = await reader.ReadToEndAsync();
                }
            }
        }

        private async void dOpen_Click(object sender, RoutedEventArgs e)
        {
            WriteableBitmap nbmp = await StorageFileToWritableBitmap();
            if(nbmp != null)
            {
                cbmp = nbmp;
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(await Convert(cbmp));
                dImage.Source = bitmap;
            }
        }

        public async void ToImage(string text)
        {
            int[] bounds = getBounds(text.Length);
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
                        char r;
                        char g;
                        char b;
                        if (pos < text.Length) { r = text[pos++]; } else { r = '\0'; }
                        if (pos < text.Length) { g = text[pos++]; } else { g = '\0'; }
                        if (pos < text.Length) { b = text[pos++]; } else { b = '\0'; }
                        bmp.SetPixel(x, y, (byte)255, (byte)r, (byte)g, (byte)b);
                    }
                }
            }
            
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(await Convert(bmp));

            cbmp = bmp;
            dImage.Source = bitmap;
            mpivot.SelectedIndex = 1;
        }

        private int[] getBounds(int length)
        {
            length = (length + 3 - length % 3) / 3;

            List<int> widthlist = new List<int>();
            do
            {
                for (int i = 1; i <= length; i++)
                {
                    if (length % i == 0 && Math.Ceiling((double)i / 3) <= length / i && Math.Ceiling((double)(length / i) / 3) <= i)
                    {
                        widthlist.Add(i);
                    }
                }
                if (widthlist.Count() == 0) { length += 1; }
            } while (widthlist.Count() == 0);

            int width = widthlist.ElementAt(widthlist.Count() / 2);
            int height = length / width;
            return new int[] { width, height, length };
        }

        private async Task<IRandomAccessStream> Convert(WriteableBitmap writeableBitmap)
        {
            var stream = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, pixels);
            await encoder.FlushAsync();
            return stream;
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
                        if (r != '\0') { sb.Append(r); }
                        if (g != '\0') { sb.Append(g); }
                        if (b != '\0') { sb.Append(b); }
                    }
                }
            }
            string str = sb.ToString();
            TextBox tb = new TextBox();
            if (await LanguageManager.GetKeyInputDialog(tb).ShowAsync() != ContentDialogResult.Secondary)
            {
                string key = tb.Text;
                str = Crypto.Decrypt(str, key);
                if(str != null)
                {
                    eText.Text = str;
                    mpivot.SelectedIndex = 0;
                } else
                {
                    await LanguageManager.KeyInvalidDialog().ShowAsync();
                }
            }
        }

        public static async Task WriteableBitmapToStorageFile(WriteableBitmap bm)
        {
            var Picker = new FileSavePicker();
            Picker.FileTypeChoices.Add("Image", new List<string>() { ".png" });
            StorageFile file = await Picker.PickSaveFileAsync();
            if(file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    Stream pixelStream = bm.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)bm.PixelWidth, (uint)bm.PixelHeight, 96, 96, pixels);
                    await encoder.FlushAsync();
                }
            }
        }

        public static async Task<WriteableBitmap> StorageFileToWritableBitmap()
        {
            var Picker = new FileOpenPicker();
            Picker.FileTypeFilter.Add(".png");
            StorageFile file = await Picker.PickSingleFileAsync();
            if (file == null) { return null; }

            ImageProperties properties = await file.Properties.GetImagePropertiesAsync();
            WriteableBitmap bmp = new WriteableBitmap((int)properties.Width, (int)properties.Height);
            bmp.SetSource(await file.OpenAsync(FileAccessMode.Read));
            return bmp;
        }

        public async void OpenSettings()
        {
            Lang lang = await LanguageManager.ShowSettingsDialog();
            Debug.WriteLine(lang.ToString());
            if(lang != Lang.Null)
            {
                SetLanguage(lang);
                GetLanguage();
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            OpenSettings();
        }
    }
}
