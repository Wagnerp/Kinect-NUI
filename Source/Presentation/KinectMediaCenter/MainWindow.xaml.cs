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
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using KinectNUI.Business.Kinect;
using Microsoft.Kinect;
using System.IO;

namespace KinectNUI.Presentation.KinectMediaCenter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		#region Properties and Globals

		private KinectSensor nui = new KinectSensor(); // All Kinect communication goes through this object
		private GestureHandler handler; // Handles gestures
		private byte[] depthFrame32 = new byte[320 * 240 * 4]; // How we get 3D spatial information
		private DateTime lastGesture = DateTime.MinValue;

		#endregion Properties and Globals
		#region Event Handlers

		/// <summary>Fires when the window loads.</summary>
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Inits
			handler = new GestureHandler(30);
			handler.isGesturesEnabled = true;
			
			// Initialize the Kinect connection
            nui.ColorStream.Enable();
            nui.DepthStream.Enable();
            nui.SkeletonStream.Enable();
            nui.Start();
			nui.ElevationAngle = 6;

			// Wire up event handlers
			nui.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(nui_SkeletonFrameReady);
			//nui.VideoFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_VideoFrameReady);
			//nui.DepthFrameReady += new EventHandler<ImageFrameReadyEventArgs>(nui_DepthFrameReady);

			// Connect to the streams
			// nui.VideoStream.Open(ImageStreamType.Video, 2, ImageResolution.Resolution640x480, ImageType.Color);
			lbMenuMain.SelectedIndex = 0;
		}
		/// <summary>Fires when a skeleton frame is ready.</summary>
		protected void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
		{
			List<Skeleton> skels = e.OpenSkeletonFrame.Skeletons.ToList();
			for (int i = skels.Count - 1; i >= 0; i--)
				if (skels[i].Joints[0].Position.X == 0 && skels[i].Joints[0].Position.Z == 0)
					skels.RemoveAt(i);
			SkeletonData[] skels2 = skels.ToArray<SkeletonData>();

			if (skels2.Count() < 1) return;
			foreach (Joint joint in skels2[0].Joints)
				handler.AppendJointHistory(joint);
			if (lastGesture < DateTime.Now.AddSeconds(-2))
			{
				if (handler.isGesturesEnabled)
				{
					bool isGestured = DetectGestures();
					if (isGestured) lastGesture = DateTime.Now;
				}
				else handler.DetectGestureMulti_EnableGestures();

			}
		}
		/// <summary>When we close the window, make sure we close the Kinect stream. Be a good code citizen.</summary>
		/// <remarks>One would expect this to turn off the depth sensor. Alas, it does not.</remarks>
		private void Window_Closed(object sender, EventArgs e)
		{
			nui.Stop();
			System.Windows.Application.Current.Shutdown();
		}

		#endregion Event Handlers
		#region DllImports

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
		[DllImport("user32.dll", SetLastError = true)]
		public static extern int SendInput(int nInputs, ref util.INPUT mi, int cbSize);
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int CmdShow);
		[DllImport("user32.dll")]
		static extern bool GetWindowPlacement(IntPtr hWnd, ref util.WINDOWPLACEMENT lpwndl);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, util.SetWindowPosFlags uFlags);
		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(ref Point pt);
		[DllImport("user32.dll")]
		static extern IntPtr WindowFromPoint(util.POINT Point);
		[DllImport("user32.dll")]
		static extern bool SetCursorPos(int X, int Y);
		[DllImport("user32.dll")]
		static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(IntPtr hwnd, out util.RECT lpRect);

		#endregion DllImports
		#region Event declarations

		public delegate void Message(string message);

		#endregion Event declarations
		#region Gesture recognition
		/// <summary>While Gesture recognition is enabled, detects and executes ALL GESTURES.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		/// <remarks>Note there is a prioritization concept as to which gestures get recognized first.
		///	If you do a multipoint gesture, we don't want to recognize a single-point gesture with the same motions.</remarks>
		public bool DetectGestures()
		{

			if (handler.JointHistory.Count == 0 || handler.JointHistory[0].Count < handler.JointBufferSize / 2) return false;	// Do not detect gestures if our buffer is too small
			else if (handler.DetectGesturesMultipoint()) return true;											// Detect multi-point gestures first. 
			else																						// Detect single-point gestures for each interesting point
				foreach (List<Joint> joints in handler.JointHistory)
				{
					if (joints.Count == 0)
						continue;
					switch (joints[0].ID)
					{
						case (JointID.HandRight): if (DetectGesturesRH(joints)) return true; break;
						case (JointID.HandLeft): if (DetectGesturesLH(joints)) return true; break;
					}
				}
			return false; // No gestures recognized
		}
		/// <summary>Detects single-point right hand gestures.</summary>
		/// <param name="joints">Joint history for the right hand</param>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesRH(List<Joint> joints)
		{
			if (joints.Count < handler.JointBufferSize)
				return false;

			else if (Gestures.isMovedRight(joints, util.InchesToMeters(12)))
			{
				if (lbMenuMain.Visibility == System.Windows.Visibility.Visible)
					if (lbMenuMain.SelectedIndex > 0)
					{
						lbMenuMain.SelectedIndex--;
						lbMenuMain.ScrollIntoView(lbMenuMain.SelectedItem);
						handler.JointHistory[(int)JointID.HandRight].Clear();
					}
				return true;
			}
			else if (Gestures.isMovedLeft(joints, util.InchesToMeters(12)))
			{
				if (lbMenuMain.Visibility == System.Windows.Visibility.Visible)
					if (lbMenuMain.SelectedIndex < lbMenuMain.Items.Count)
					{
						lbMenuMain.SelectedIndex++;
						lbMenuMain.ScrollIntoView(lbMenuMain.SelectedItem);
						handler.JointHistory[(int)JointID.HandRight].Clear();
					}
				return true;
			}

			else if (Gestures.isMovedOut(joints, util.InchesToMeters(18)))
			{ // Right hand moved back away from sensor
				handler.JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			else if (Gestures.isMovedIn(joints, util.InchesToMeters(18)))
			{ // Right hand moved in toward sensor
				handler.JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}

			else return false;
		}
		/// <summary>Detects single-point left hand gestures.</summary>
		/// <param name="joints">Joint history for the left hand</param>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesLH(List<Joint> joints)
		{
			if (joints.Count < handler.JointBufferSize / 2)
				return false;

			if (Gestures.isMovedOut(joints, util.InchesToMeters(24)))
			{ // Left hand moved back away from sensor
				handler.JointHistory[(int)joints[0].ID].Clear();
				return true;
			}
			else if (Gestures.isMovedIn(joints, util.InchesToMeters(24)))
			{ // Left hand moved in toward sensor
				handler.JointHistory[(int)joints[0].ID].Clear();
				return true;
			}
			else return false;
		}
		#endregion
	}

	public static class ImageLoader
	{
		public static List<BitmapImage> LoadMainMenuImages()
		{
			List<BitmapImage> mainMenuImages = new List<BitmapImage>();
			foreach (FileInfo fi in new DirectoryInfo(@".").GetFiles("*.png"))
			{
				Uri uri = new Uri(fi.FullName);
				mainMenuImages.Add(new BitmapImage(uri));
			}
			return mainMenuImages;
		}
	}

}
