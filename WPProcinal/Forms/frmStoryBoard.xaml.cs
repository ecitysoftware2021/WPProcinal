using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPProcinal.Forms
{
    /// <summary>
    /// Interaction logic for frmStoryBoard.xaml
    /// </summary>
    public partial class frmStoryBoard : Window
    {
        public frmStoryBoard(string video)
        {
            InitializeComponent();
            MEVideo.Source = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "StoryBoards", video));
        }

        private void Image_TouchDown(object sender, TouchEventArgs e)
        {
            this.Close();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {

        }

    }
}
