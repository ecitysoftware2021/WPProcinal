﻿#pragma checksum "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "33219ECE970ED586F7A9D56E3DFAC5B82CB62C90294057E24B31E088B61F772E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using WPProcinal.Forms.User_Control;


namespace WPProcinal.Forms.User_Control {
    
    
    /// <summary>
    /// UCSelectProducts
    /// </summary>
    public partial class UCSelectProducts : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 25 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTimer;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BrdCombos;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BrdDrinks;
        
        #line default
        #line hidden
        
        
        #line 116 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BrdPackages;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border BrdOthers;
        
        #line default
        #line hidden
        
        
        #line 190 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnSalir;
        
        #line default
        #line hidden
        
        
        #line 200 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnOmitir;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/user_control/ucselectproducts.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.tbTimer = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.BrdCombos = ((System.Windows.Controls.Border)(target));
            
            #line 52 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            this.BrdCombos.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BrdCombos_TouchDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.BrdDrinks = ((System.Windows.Controls.Border)(target));
            
            #line 88 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            this.BrdDrinks.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BrdDrinks_TouchDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.BrdPackages = ((System.Windows.Controls.Border)(target));
            
            #line 124 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            this.BrdPackages.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BrdPackages_TouchDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.BrdOthers = ((System.Windows.Controls.Border)(target));
            
            #line 160 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            this.BrdOthers.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BrdOthers_TouchDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.BtnSalir = ((System.Windows.Controls.Image)(target));
            
            #line 196 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            this.BtnSalir.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnSalir_TouchDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnOmitir = ((System.Windows.Controls.Image)(target));
            
            #line 206 "..\..\..\..\Forms\User_Control\UCSelectProducts.xaml"
            this.BtnOmitir.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnOmitir_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

