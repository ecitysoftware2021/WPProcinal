﻿#pragma checksum "..\..\..\Forms\frmConfirmationModal.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "82F31E120D3359E8673E14B4BE711196E6E6FD72D2568AAF9D6CEABB95928FA9"
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


namespace WPProcinal.Forms {
    
    
    /// <summary>
    /// frmConfirmationModal
    /// </summary>
    public partial class frmConfirmationModal : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 74 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtTitle;
        
        #line default
        #line hidden
        
        
        #line 88 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtRoom;
        
        #line default
        #line hidden
        
        
        #line 106 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtDate;
        
        #line default
        #line hidden
        
        
        #line 119 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvListSeats;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtTotal;
        
        #line default
        #line hidden
        
        
        #line 154 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnYes;
        
        #line default
        #line hidden
        
        
        #line 162 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnNo;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/frmconfirmationmodal.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\frmConfirmationModal.xaml"
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
            this.TxtTitle = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 2:
            this.TxtRoom = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.TxtDate = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            this.lvListSeats = ((System.Windows.Controls.ListView)(target));
            return;
            case 5:
            this.TxtTotal = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 6:
            this.BtnYes = ((System.Windows.Controls.Image)(target));
            
            #line 156 "..\..\..\Forms\frmConfirmationModal.xaml"
            this.BtnYes.PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.BtnYes_PreviewStylusDown);
            
            #line default
            #line hidden
            
            #line 157 "..\..\..\Forms\frmConfirmationModal.xaml"
            this.BtnYes.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.BtnYes_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnNo = ((System.Windows.Controls.Image)(target));
            
            #line 165 "..\..\..\Forms\frmConfirmationModal.xaml"
            this.BtnNo.PreviewStylusDown += new System.Windows.Input.StylusDownEventHandler(this.BtnNo_PreviewStylusDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

