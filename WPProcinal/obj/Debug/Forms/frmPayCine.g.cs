﻿#pragma checksum "..\..\..\Forms\frmPayCine.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ACF2CF724F225BBCF35075BA3615FDE6DE97AEE2"
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
using WPProcinal.Forms;
using WpfAnimatedGif;


namespace WPProcinal.Forms {
    
    
    /// <summary>
    /// frmPayCine
    /// </summary>
    public partial class frmPayCine : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid PaymentGrid;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblValorPagar;
        
        #line default
        #line hidden
        
        
        #line 55 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblValorIngresado;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblFaltante;
        
        #line default
        #line hidden
        
        
        #line 90 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblSobrante;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnCancelar;
        
        #line default
        #line hidden
        
        
        #line 109 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgLeyendoBillete;
        
        #line default
        #line hidden
        
        
        #line 115 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnCancelar_Copy;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnChange;
        
        #line default
        #line hidden
        
        
        #line 133 "..\..\..\Forms\frmPayCine.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgEspereRecibo;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/frmpaycine.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\frmPayCine.xaml"
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
            
            #line 9 "..\..\..\Forms\frmPayCine.xaml"
            ((WPProcinal.Forms.frmPayCine)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.PaymentGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.lblValorPagar = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblValorIngresado = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 5:
            this.lblFaltante = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.lblSobrante = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.btnCancelar = ((System.Windows.Controls.Image)(target));
            
            #line 106 "..\..\..\Forms\frmPayCine.xaml"
            this.btnCancelar.PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.btnCancelar_PreviewStylusDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.imgLeyendoBillete = ((System.Windows.Controls.Image)(target));
            return;
            case 9:
            this.btnCancelar_Copy = ((System.Windows.Controls.Image)(target));
            return;
            case 10:
            this.btnChange = ((System.Windows.Controls.Image)(target));
            return;
            case 11:
            this.imgEspereRecibo = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

