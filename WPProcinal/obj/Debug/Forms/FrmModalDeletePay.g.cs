﻿#pragma checksum "..\..\..\Forms\FrmModalDeletePay.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "5A2286A41341344CF8E358261113C12C4B06A8C6CB6AB0332185DF1FFFFD9F66"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
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
using WPProcinal.Forms;


namespace WPProcinal.Forms {
    
    
    /// <summary>
    /// FrmModalDeletePay
    /// </summary>
    public partial class FrmModalDeletePay : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 42 "..\..\..\Forms\FrmModalDeletePay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtSecuencia;
        
        #line default
        #line hidden
        
        
        #line 44 "..\..\..\Forms\FrmModalDeletePay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtMs;
        
        #line default
        #line hidden
        
        
        #line 69 "..\..\..\Forms\FrmModalDeletePay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btIngresar;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\..\Forms\FrmModalDeletePay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btSalir;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/frmmodaldeletepay.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\FrmModalDeletePay.xaml"
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
            this.txtSecuencia = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.txtMs = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.btIngresar = ((System.Windows.Controls.Button)(target));
            
            #line 68 "..\..\..\Forms\FrmModalDeletePay.xaml"
            this.btIngresar.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtIngresar_TouchDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btSalir = ((System.Windows.Controls.Button)(target));
            
            #line 80 "..\..\..\Forms\FrmModalDeletePay.xaml"
            this.btSalir.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtSalir_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
