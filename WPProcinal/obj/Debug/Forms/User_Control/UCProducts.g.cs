﻿#pragma checksum "..\..\..\..\Forms\User_Control\UCProducts.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "31DC1B7D6D05C0E672E16068424948767770F55B9E3D019AF7E883A8A1BD21C6"
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
    /// UCProducts
    /// </summary>
    public partial class UCProducts : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 24 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTimer;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock typeSelected;
        
        #line default
        #line hidden
        
        
        #line 42 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lv_Products;
        
        #line default
        #line hidden
        
        
        #line 214 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnSalir;
        
        #line default
        #line hidden
        
        
        #line 224 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnComprar;
        
        #line default
        #line hidden
        
        
        #line 234 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnMore;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/user_control/ucproducts.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
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
            this.typeSelected = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.lv_Products = ((System.Windows.Controls.ListView)(target));
            return;
            case 6:
            this.BtnSalir = ((System.Windows.Controls.Image)(target));
            
            #line 220 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
            this.BtnSalir.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnSalir_TouchDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnComprar = ((System.Windows.Controls.Image)(target));
            
            #line 230 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
            this.BtnComprar.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnComprar_TouchDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BtnMore = ((System.Windows.Controls.Image)(target));
            
            #line 241 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
            this.BtnMore.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnMore_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 4:
            
            #line 174 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
            ((System.Windows.Controls.Image)(target)).TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnPlus_TouchDown);
            
            #line default
            #line hidden
            break;
            case 5:
            
            #line 200 "..\..\..\..\Forms\User_Control\UCProducts.xaml"
            ((System.Windows.Controls.Image)(target)).TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnLess_TouchDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

