﻿#pragma checksum "..\..\..\Forms\frmConfirmationModal.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2F56823B43EAD4E8600BAA3EFB1BF90DC43679EF"
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
        
        
        #line 82 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtTitle;
        
        #line default
        #line hidden
        
        
        #line 98 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtRoom;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtDate;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvListSeats;
        
        #line default
        #line hidden
        
        
        #line 157 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock TxtTotal;
        
        #line default
        #line hidden
        
        
        #line 174 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnCash;
        
        #line default
        #line hidden
        
        
        #line 182 "..\..\..\Forms\frmConfirmationModal.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image BtnCard;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\..\Forms\frmConfirmationModal.xaml"
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
            this.BtnCash = ((System.Windows.Controls.Image)(target));
            
            #line 177 "..\..\..\Forms\frmConfirmationModal.xaml"
            this.BtnCash.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnCash_TouchDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.BtnCard = ((System.Windows.Controls.Image)(target));
            
            #line 185 "..\..\..\Forms\frmConfirmationModal.xaml"
            this.BtnCard.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnCard_TouchDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.BtnNo = ((System.Windows.Controls.Image)(target));
            
            #line 196 "..\..\..\Forms\frmConfirmationModal.xaml"
            this.BtnNo.TouchDown += new System.EventHandler<System.Windows.Input.TouchEventArgs>(this.BtnNo_TouchDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

