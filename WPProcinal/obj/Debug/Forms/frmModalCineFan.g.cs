﻿#pragma checksum "..\..\..\Forms\frmModalCineFan.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9F54FA8DDD780FEAC57FDC76A7750AFA98BE24ED00692863257447998A838C0B"
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
using WpfAnimatedGif;


namespace WPProcinal.Forms {
    
    
    /// <summary>
    /// frmModalCineFan
    /// </summary>
    public partial class frmModalCineFan : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 28 "..\..\..\Forms\frmModalCineFan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid grdScanner;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\Forms\frmModalCineFan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btRegister;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\Forms\frmModalCineFan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image gifCedula;
        
        #line default
        #line hidden
        
        
        #line 105 "..\..\..\Forms\frmModalCineFan.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtError;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/frmmodalcinefan.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\frmModalCineFan.xaml"
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
            
            #line 18 "..\..\..\Forms\frmModalCineFan.xaml"
            ((WPProcinal.Forms.frmModalCineFan)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.grdScanner = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.btRegister = ((System.Windows.Controls.Image)(target));
            
            #line 54 "..\..\..\Forms\frmModalCineFan.xaml"
            this.btRegister.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnRegister_TouchDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.gifCedula = ((System.Windows.Controls.Image)(target));
            return;
            case 5:
            
            #line 88 "..\..\..\Forms\frmModalCineFan.xaml"
            ((System.Windows.Controls.Image)(target)).TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.Image_TouchDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtError = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

