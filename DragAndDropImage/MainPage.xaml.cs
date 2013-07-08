using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DragAndDropImage
{
    public partial class MainPage : UserControl
    {
        #region Fields

        private bool _isDrag;
        private Image _image;
        private Point _dragAnchor;

        #endregion

        #region Constructors

        public MainPage()
        {
            InitializeComponent();
        }
        
        #endregion

        #region EventHandlers
        
        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            var panel = sender as Canvas;
            var dropFormatArray = e.Data.GetFormats();
           
            if (dropFormatArray.Contains("FileDrop"))
            {
                var dropObjectArray = e.Data.GetData("FileDrop");
                var fileObj = dropObjectArray as FileInfo[];
                
                foreach (var fi in fileObj)
                {
                    var dropFile = fi.OpenRead();
                    
                    try
                    {
                        var image = new Image();
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(dropFile);
                        image.Source = bitmapImage;

                        image.Width = bitmapImage.PixelWidth;
                        image.Height = bitmapImage.PixelHeight;

                        image.Cursor = Cursors.Hand;
                        image.MouseLeftButtonDown += new MouseButtonEventHandler(image_MouseLeftButtonDown);
                        image.MouseLeftButtonUp += new MouseButtonEventHandler(image_MouseLeftButtonUp);

                        panel.Children.Add(image);
                        
                        Canvas.SetLeft(image, e.GetPosition(panel).X - image.Width / 2);
                        Canvas.SetTop(image, e.GetPosition(panel).Y - image.Height / 2);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        //stream errors
                        //might also want to handle ImageFailed on added images
                        //so can prompt user about format errors etc.
                    }
                }
            }
        }

        private void ImagePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrag)
            {
                Canvas.SetLeft(_image, e.GetPosition(ImagePanel).X - _dragAnchor.X);
                Canvas.SetTop(_image, e.GetPosition(ImagePanel).Y - _dragAnchor.Y);
            }
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _image = sender as Image;
            _dragAnchor = e.GetPosition(_image);
            _isDrag = true;
        }

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDrag = false;
        }

        #endregion
    }
}
