﻿#pragma checksum "C:\Users\skowa\source\repos\ImageCrypt\ImageCrypt\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8D44642A1B3DA230FA497ADD246772FA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImageCrypt
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.16.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1: // MainPage.xaml line 10
                {
                    this.grid = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 2: // MainPage.xaml line 11
                {
                    this.mpivot = (global::Windows.UI.Xaml.Controls.Pivot)(target);
                }
                break;
            case 3: // MainPage.xaml line 44
                {
                    this.dImage = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 4: // MainPage.xaml line 33
                {
                    this.dExecute = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                }
                break;
            case 5: // MainPage.xaml line 38
                {
                    this.dSave = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.dSave).Click += this.dSave_Click;
                }
                break;
            case 6: // MainPage.xaml line 39
                {
                    this.dOpen = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                }
                break;
            case 7: // MainPage.xaml line 26
                {
                    this.eText = (global::Windows.UI.Xaml.Controls.RichEditBox)(target);
                }
                break;
            case 8: // MainPage.xaml line 15
                {
                    this.eExecute = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.eExecute).Click += this.eExecute_Click;
                }
                break;
            case 9: // MainPage.xaml line 20
                {
                    this.eSave = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.eSave).Click += this.eSave_Click;
                }
                break;
            case 10: // MainPage.xaml line 21
                {
                    this.eOpen = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        /// <summary>
        /// GetBindingConnector(int connectionId, object target)
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 10.0.16.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

