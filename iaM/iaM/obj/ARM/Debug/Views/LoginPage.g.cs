﻿#pragma checksum "\\Mac\Home\Documents\IM166\iaM\iaM\Views\LoginPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6FAB4BDD39D3A889AB439737FDC100DC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iaM.Views
{
    partial class LoginPage : 
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
                    this.contentPanel = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 2:
                {
                    this.Login_Signup = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 165 "\\Mac\Home\Documents\IM166\iaM\iaM\Views\LoginPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.Login_Signup).Click += this.Signup;
                    #line default
                }
                break;
            case 3:
                {
                    this.inputPanel = (global::Windows.UI.Xaml.Controls.StackPanel)(target);
                }
                break;
            case 4:
                {
                    this.Grid_Password = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 5:
                {
                    this.Login_Start = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 156 "\\Mac\Home\Documents\IM166\iaM\iaM\Views\LoginPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.Login_Start).Click += this.ToMainPage;
                    #line default
                }
                break;
            case 6:
                {
                    this.Input_Password = (global::Windows.UI.Xaml.Controls.TextBox)(target);
                }
                break;
            case 7:
                {
                    global::Windows.UI.Xaml.Controls.Button element7 = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 147 "\\Mac\Home\Documents\IM166\iaM\iaM\Views\LoginPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)element7).Click += this.Forgot_Password;
                    #line default
                }
                break;
            case 8:
                {
                    this.Grid_Mail = (global::Windows.UI.Xaml.Controls.Grid)(target);
                }
                break;
            case 9:
                {
                    this.Input_Mail = (global::Windows.UI.Xaml.Controls.TextBox)(target);
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

