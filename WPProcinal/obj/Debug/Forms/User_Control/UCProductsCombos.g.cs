﻿#pragma checksum "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "582B3816056C66A6AD9A091215EABAACF3255A807842B513BE9821A82F13F149"
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
    /// UCProductsCombos
    /// </summary>
    public partial class UCProductsCombos : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 21 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTimer;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnCombos;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lv_Products;
        
        #line default
        #line hidden
        
        
        #line 126 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnSalir;
        
        #line default
        #line hidden
        
        
        #line 136 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnComprar;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/user_control/ucproductscombos.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
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
            
            #line 8 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
            ((WPProcinal.Forms.User_Control.UCProductsCombos)(target)).Loaded += new System.Windows.RoutedEventHandler(this.UserControl_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.tbTimer = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.BtnCombos = ((System.Windows.Controls.Image)(target));
            
            #line 36 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
            this.BtnCombos.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnCombos_TouchDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.lv_Products = ((System.Windows.Controls.ListView)(target));
            return;
            case 7:
            this.BtnSalir = ((System.Windows.Controls.Image)(target));
            
            #line 132 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
            this.BtnSalir.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnSalir_TouchDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BtnComprar = ((System.Windows.Controls.Image)(target));
            
            #line 142 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
            this.BtnComprar.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnComprar_TouchDown);
            
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
            case 5:
            
            #line 96 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
            ((System.Windows.Controls.Image)(target)).TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnPlus_TouchDown);
            
            #line default
            #line hidden
            break;
            case 6:
            
            #line 117 "..\..\..\..\Forms\User_Control\UCProductsCombos.xaml"
            ((System.Windows.Controls.Image)(target)).TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnLess_TouchDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

