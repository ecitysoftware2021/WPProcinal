﻿#pragma checksum "..\..\..\Forms\Opciones.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4EF24760924E1EFCFCB2E1710562C99F7AD8FE99D2193304E05B8D453C625460"
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
    /// Opciones
    /// </summary>
    public partial class Opciones : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridOperaciones;
        
        #line default
        #line hidden
        
        
        #line 37 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtOpcion;
        
        #line default
        #line hidden
        
        
        #line 50 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gridTeclado;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtultimosDigitos;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ShowHide;
        
        #line default
        #line hidden
        
        
        #line 187 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock botonAceptar;
        
        #line default
        #line hidden
        
        
        #line 225 "..\..\..\Forms\Opciones.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image GifLoad2;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/opciones.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\Opciones.xaml"
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
            
            #line 19 "..\..\..\Forms\Opciones.xaml"
            ((WPProcinal.Forms.Opciones)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.gridOperaciones = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.txtOpcion = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.gridTeclado = ((System.Windows.Controls.Grid)(target));
            return;
            case 5:
            this.txtultimosDigitos = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            this.ShowHide = ((System.Windows.Controls.Image)(target));
            
            #line 86 "..\..\..\Forms\Opciones.xaml"
            this.ShowHide.PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.ShowHide_PreviewStylusDown);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 97 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 98 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 108 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 109 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 119 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 120 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 130 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 131 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 141 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 142 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 152 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 153 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 13:
            
            #line 163 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 164 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 174 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 175 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 185 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 186 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 16:
            this.botonAceptar = ((System.Windows.Controls.TextBlock)(target));
            
            #line 196 "..\..\..\Forms\Opciones.xaml"
            this.botonAceptar.PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.BotonAceptar_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 197 "..\..\..\Forms\Opciones.xaml"
            this.botonAceptar.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.BotonAceptar_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 17:
            
            #line 207 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 208 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 218 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.TextBlock_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 219 "..\..\..\Forms\Opciones.xaml"
            ((System.Windows.Controls.TextBlock)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.TextBlock_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 19:
            this.GifLoad2 = ((System.Windows.Controls.Image)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

