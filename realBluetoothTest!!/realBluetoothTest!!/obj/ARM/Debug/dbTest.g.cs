﻿#pragma checksum "C:\Users\KMK\Videos\IM16\IM16\realBluetoothTest!!\realBluetoothTest!!\dbTest.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "E76D62B3C95508469E39F3D831700368"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace realBluetoothTest__
{
    partial class dbTest : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.image_show = (global::Windows.UI.Xaml.Controls.Image)(target);
                }
                break;
            case 2:
                {
                    this.test = (global::Windows.UI.Xaml.Controls.TextBlock)(target);
                }
                break;
            case 3:
                {
                    this.photo_pick = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 14 "..\..\..\dbTest.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.photo_pick).Click += this.Click_Photo_Button;
                    #line default
                }
                break;
            case 4:
                {
                    this.textBox = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 5:
                {
                    this.Add = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 22 "..\..\..\dbTest.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.Add).Click += this.Add_Click;
                    #line default
                }
                break;
            case 6:
                {
                    this.Show = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 23 "..\..\..\dbTest.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.Show).Click += this.Show_Click;
                    #line default
                }
                break;
            case 7:
                {
                    this.RemoveAll = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 24 "..\..\..\dbTest.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.RemoveAll).Click += this.Delete_Click;
                    #line default
                }
                break;
            case 8:
                {
                    this.refresh = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 25 "..\..\..\dbTest.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.refresh).Click += this.Contact_Click;
                    #line default
                }
                break;
            case 9:
                {
                    this.image_b = (global::Windows.UI.Xaml.Controls.AppBarButton)(target);
                    #line 26 "..\..\..\dbTest.xaml"
                    ((global::Windows.UI.Xaml.Controls.AppBarButton)this.image_b).Click += this.Image_Click;
                    #line default
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}

