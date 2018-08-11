using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace ImageCrypt
{
    class LanguageManager
    {
        public static Lang Language;

        public static string PiText { get; set; }
        public static string PiImage { get; set; }
        public static string BtnEncrypt { get; set; }
        public static string BtnDecrypt { get; set; }
        public static string BtnSave { get; set; }
        public static string BtnLoad { get; set; }
        public static string BtnSettings { get; set; }

        public static ContentDialog GetExitDialog()
        {
            ContentDialog dialog = null;
            if(Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "Exit", Content = "Are you sure you want to exit the application?", PrimaryButtonText = "Yes", SecondaryButtonText = "No",
                };
            } else if(Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Beenden", Content = "Wollen Sie die App wirklich beenden?", PrimaryButtonText = "Ja", SecondaryButtonText = "Nein",
                };
            }
            return dialog;
        }

        public static ContentDialog GetMaxTextSizeDialog()
        {
            ContentDialog dialog = null;
            if (Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "Max Text Size Reached",
                    Content = "The input text can not be bigger than " + Int32.MaxValue + " characters",
                    PrimaryButtonText = "Ok"
                };
            }
            else if (Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Maximale Textgröße erreicht",
                    Content = "Der eingegebene Text darf nicht mehr als " + Int32.MaxValue + " Zeichen haben",
                    PrimaryButtonText = "Ok"
                };
            }
            return dialog;
        }

        public static ContentDialog GetNoTextDialog()
        {
            ContentDialog dialog = null;
            if (Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "Textbox is empty",
                    Content = "Insert text into the textbox to continue with this action",
                    PrimaryButtonText = "Ok"
                };
            }
            else if (Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Textbox ist leer",
                    Content = "Trage Text in die Textbox ein, um mit dieser Aktion fortzufahren",
                    PrimaryButtonText = "Ok"
                };
            }
            return dialog;
        }

        public static ContentDialog GetNoImageDialog()
        {
            ContentDialog dialog = null;
            if (Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "No image loaded",
                    Content = "Load an image via 'Load File' to continue with this action",
                    PrimaryButtonText = "Ok"
                };
            }
            else if (Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Kein Bild geladen",
                    Content = "Lade ein Bild mit 'Datei laden' um mit dieser Aktion fortzufahren",
                    PrimaryButtonText = "Ok"
                };
            }
            return dialog;
        }

        public static ContentDialog GetKeyInputDialog(TextBox tb)
        {
            ContentDialog dialog = null;
            if (Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "Secret Key",
                    Content = tb,
                    PrimaryButtonText = "Ok",
                    SecondaryButtonText = "Cancel",
                };
            } else if(Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Geheimer Schlüssel",
                    Content = tb,
                    PrimaryButtonText = "Ok",
                    SecondaryButtonText = "Abbrechen",
                };
            }
            return dialog;
        }

        public static ContentDialog KeyInvalidDialog()
        {
            ContentDialog dialog = null;
            if (Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "Error during decryption",
                    Content = "Invalid key given",
                    PrimaryButtonText = "Ok"
                };
            }
            else if (Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Fehler bei der Entschlüsslung",
                    Content = "Fahlscher Schlüssel gegeben",
                    PrimaryButtonText = "Ok"
                };
            }
            return dialog;
        }

        public static ContentDialog GetEncryptionErrorDialog()
        {
            ContentDialog dialog = null;
            if (Language == Lang.English)
            {
                dialog = new ContentDialog()
                {
                    Title = "Error during encryption",
                    Content = "An unknown error occured",
                    PrimaryButtonText = "Ok"
                };
            }
            else if (Language == Lang.German)
            {
                dialog = new ContentDialog()
                {
                    Title = "Fehler bei der Verschlüsslung",
                    Content = "Ein unbekannter Fehler ist aufgetreten",
                    PrimaryButtonText = "Ok"
                };
            }
            return dialog;
        }

        public static void SetText()
        {
            PiText = (Language == Lang.English) ? "Text" : (Language == Lang.German) ? "Text" : "";
            PiImage = (Language == Lang.English) ? "Image" : (Language == Lang.German) ? "Bild" : "";
            BtnEncrypt = (Language == Lang.English) ? "Encrypt" : (Language == Lang.German) ? "Verschlüsseln" : "";
            BtnDecrypt = (Language == Lang.English) ? "Decrypt" : (Language == Lang.German) ? "Entschlüsseln" : "";
            BtnSave = (Language == Lang.English) ? "Save" : (Language == Lang.German) ? "Speichern" : "";
            BtnLoad = (Language == Lang.English) ? "Load File" : (Language == Lang.German) ? "Datei laden" : "";
            BtnSettings = (Language == Lang.English) ? "Settings" : (Language == Lang.German) ? "Einstellungen" : "";
        }

    }

    public enum Lang
    {
        English, German
    }

}
