using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPProcinal.Classes
{
    public class MoviesViewModel
    {
        private string _Title;
        public string Title
        {
            get { return this._Title; }
            set { this._Title = value; }
        }

        private BitmapImage _ImageData;
        public BitmapImage ImageData
        {
            get { return this._ImageData; }
            set { this._ImageData = value; }
        }

        private BitmapImage _ImageTag;
        public BitmapImage ImageTag
        {
            get { return this._ImageTag; }
            set { this._ImageTag = value; }
        }

        private string _Tag;
        public string Tag
        {
            get { return this._Tag; }
            set { this._Tag = value; }
        }

        public string Id { get; set; }


    }
}
