﻿#pragma checksum "..\..\..\Forms\frmMovies.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "55967C25760243BB6B1B2280D60C1F31D772CE3B103A15A256FF1CF8BED04055"
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
    /// frmMovies
    /// </summary>
    public partial class frmMovies : System.Windows.Window, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 16 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid TVGrid;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTimer;
        
        #line default
        #line hidden
        
        
        #line 47 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnPrev;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image btnNext;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lvMovies;
        
        #line default
        #line hidden
        
        
        #line 108 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblCinema1;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image GifLoadder;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbCurrentPage;
        
        #line default
        #line hidden
        
        
        #line 151 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock textBlock3;
        
        #line default
        #line hidden
        
        
        #line 161 "..\..\..\Forms\frmMovies.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock tbTotalPage;
        
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
            System.Uri resourceLocater = new System.Uri("/WPProcinal;component/forms/frmmovies.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\frmMovies.xaml"
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
            
            #line 12 "..\..\..\Forms\frmMovies.xaml"
            ((WPProcinal.Forms.frmMovies)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.TVGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 3:
            this.tbTimer = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 4:
            
            #line 40 "..\..\..\Forms\frmMovies.xaml"
            ((System.Windows.Controls.Image)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Image_PreviewMouseDown);
            
            #line default
            #line hidden
            return;
            case 5:
            this.btnPrev = ((System.Windows.Controls.Image)(target));
            
            #line 53 "..\..\..\Forms\frmMovies.xaml"
            this.btnPrev.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.btnPrev_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.btnNext = ((System.Windows.Controls.Image)(target));
            
            #line 61 "..\..\..\Forms\frmMovies.xaml"
            this.btnNext.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.btnNext_Click);
            
            #line default
            #line hidden
            return;
            case 7:
            this.lvMovies = ((System.Windows.Controls.ListView)(target));
            return;
            case 9:
            this.lblCinema1 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 10:
            this.GifLoadder = ((System.Windows.Controls.Image)(target));
            return;
            case 11:
            this.tbCurrentPage = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 12:
            this.textBlock3 = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.tbTotalPage = ((System.Windows.Controls.TextBlock)(target));
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
            case 8:
            
            #line 88 "..\..\..\Forms\frmMovies.xaml"
            ((System.Windows.Controls.Grid)(target)).PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Movie_MouseDown);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

