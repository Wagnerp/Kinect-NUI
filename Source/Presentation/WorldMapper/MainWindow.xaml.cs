using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using KinectNUI.Business.Kinect;
using Microsoft.Kinect;

namespace WorldMapper
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{ InitializeComponent(); }

		private KinectSensor nui = new KinectSensor(); // All Kinect communication goes through this object
		private DateTime lastTime = DateTime.MinValue; // Used to calculate FPS
		private byte[] depthFrame32 = new byte[320 * 240 * 4]; // How we get 3D spatial information
		private int totalFrames = 0, lastFrames = 0; // Used to calculate FPS
		private bool[, ,] world = new bool[100, 100, 100];


		#region Event Handlers

		/// <summary>Fires when the window loads.</summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Initialize the Kinect connection
            try
            {
                nui.ColorStream.Enable();
                nui.DepthStream.Enable();
                nui.SkeletonStream.Enable();
                nui.Start();
            }
			catch (Exception ex) { MessageBox.Show(ex.ToString()); }
			nui.ElevationAngle = 0;
		}
		/// <summary>Fires when a depth frame is ready.</summary>
		/*protected void nui_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
		{
			// Convert depth frame to video frame to render it
			PlanarImage Image = e.ImageFrame.Image;
			byte[] convertedDepthFrame = util.convertDepthFrame2(Image.Bits, ref depthFrame32);
			image2.Source = BitmapSource.Create(Image.Width, Image.Height, 96, 96, PixelFormats.Bgr32, null, convertedDepthFrame, Image.Width * 4);
			MapFrame(Image.Bits);

			// Calculate FPS
			totalFrames++;
			if (lastTime < DateTime.Now.AddSeconds(-1))
			{
				int frameDiff = totalFrames - lastFrames;
				lastFrames = totalFrames;
				lastTime = DateTime.Now;
				Title = "WorldMapper - " + frameDiff.ToString() + " FPS";
			}
		}*/
		/// <summary>Fires when a video frame is ready.</summary>
		protected void nui_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
		{
			// Convert video from a useless planar format to a useful bitmap format we can actually display
			ColorImageFrame image = e.OpenColorImageFrame();
			image1.Source = BitmapSource.Create(image.Width, image.Height, 96, 96, PixelFormats.Bgr32, null, image.PixelData, image.Width * image.BytesPerPixel);
		}
		/// <summary>When we close the window, make sure we close the Kinect stream. Be a good code citizen.</summary>
		/// <remarks>One would expect this to turn off the depth sensor. Alas, it does not.</remarks>
		private void Window_Closed(object sender, EventArgs e)
		{
			nui.Stop();
			System.Windows.Application.Current.Shutdown();
		}

		private void MapFrame(byte[] frame)
		{
			for (int i16 = 0, i32 = 0; i16 < frame.Length && i32 < depthFrame32.Length; i16 += 2, i32 += 4)
			{
				int depth = (frame[i16 + 1] << 5) | (frame[i16] >> 3);

			}
		}

		#endregion Event Handlers
	}
}
