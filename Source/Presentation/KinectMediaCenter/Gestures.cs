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
using System.Runtime.InteropServices; // win32
using System.Windows.Forms; // sendkeys
using System.Diagnostics; // process stuff
using System.Threading; // a few sleeps
using KinectNUI.Business.Kinect;
using Microsoft.Kinect; // The big tamale

namespace KinectNUI.Presentation.KinectMediaCenter
{
    public static class Gestures
	{
		#region Motion Detection

		/// <summary>Did this joint move in? (Z)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedIn(List<Joint> joints, double Threshhold)
        {
            if (joints.Last().Position.Z - joints.First().Position.Z < -Threshhold && arePointsCloseXY(joints))
                return true;
            return false;
        }
		/// <summary>Did this joint move out? (Z)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedOut(List<Joint> joints, double Threshhold)
        {
            if (joints.Last().Position.Z - joints.First().Position.Z > Threshhold && arePointsCloseXY(joints, 0.2))
                return true;
            return false;
        }
		/// <summary>Did this joint move left? (X)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedLeft(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.X - joints.Last().Position.X > Threshhold && arePointsCloseYZ(joints, 0.2))
                return true;
            return false;
        }
		/// <summary>Did this joint move right? (X)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedRight(List<Joint> joints, double Threshhold)
        {

            if (joints.First().Position.X - joints.Last().Position.X < -Threshhold && arePointsCloseYZ(joints))
                return true;
            return false;
        }
		/// <summary>Did this joint move down? (Y)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedDown(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.Y - joints.Last().Position.Y > Threshhold && arePointsCloseXZ(joints))
                return true;
            return false;
        }
		/// <summary>Did this joint move up? (Y)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedUp(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.Y - joints.Last().Position.Y < -Threshhold && arePointsCloseXZ(joints))
                return true;
            return false;
        }
		/// <summary>Did this joint move up and left? (XY)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedUpLeft(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.Y - joints.Last().Position.Y < -Threshhold && joints.First().Position.X - joints.Last().Position.X > Threshhold && ArePointsCloseZ(joints, Threshhold))
                return true;
            return false;
        }
		/// <summary>Did this joint move up and right? (XY)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedUpRight(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.Y - joints.Last().Position.Y < -Threshhold && joints.First().Position.X - joints.Last().Position.X < -Threshhold && ArePointsCloseZ(joints, Threshhold))
                return true;
            return false;
        }
		/// <summary>Did this joint move down and left? (XY)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedDownLeft(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.Y - joints.Last().Position.Y > Threshhold && joints.First().Position.X - joints.Last().Position.X > Threshhold && ArePointsCloseZ(joints, Threshhold))
                return true;
            return false;
        }
		/// <summary>Did this joint move down and right? (XY)</summary>
		/// <param name="joints">List of joints representing recent history for one specific joint</param>
		/// <param name="Threshhold">Meters needed to detect motion</param>
		/// <returns>True if motion detected; else false</returns>
		public static bool isMovedDownRight(List<Joint> joints, double Threshhold)
        {
            if (joints.First().Position.Y - joints.Last().Position.Y > Threshhold && joints.First().Position.X - joints.Last().Position.X < -Threshhold && ArePointsCloseZ(joints, Threshhold))
                return true;
            return false;
        }

		#endregion Motion Detection
		#region Motion Filtering

		/// <summary>Do the points in this joint's recent history fall along nearby X coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool ArePointsCloseX(List<Joint> joints, double Threshhold = 0.2)
		{
			int numOutOfBounds = 0;
			foreach (Joint j in joints)
			{
				if (j.Position.X - joints[0].Position.X > Threshhold || joints[0].Position.X - j.Position.X > Threshhold) numOutOfBounds++;
				if (numOutOfBounds > 2) return false;
			}
			return true;
		}
		/// <summary>Do the points in this joint's recent history fall along nearby Y coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool ArePointsCloseY(List<Joint> joints, double Threshhold = 0.2)
		{
			int numOutOfBounds = 0;
			foreach (Joint j in joints)
			{
				if (j.Position.Y - joints[0].Position.Y > Threshhold || joints[0].Position.Y - j.Position.Y > Threshhold) numOutOfBounds++;
				if (numOutOfBounds > 2) return false;
			}
			return true;
		}
		/// <summary>Do the points in this joint's recent history fall along nearby Z coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool ArePointsCloseZ(List<Joint> joints, double Threshhold = 0.2)
		{
			int numOutOfBounds = 0;
			foreach (Joint j in joints)
			{
				if (j.Position.Z - joints[0].Position.Z > Threshhold || joints[0].Position.Z - j.Position.Z > Threshhold) numOutOfBounds++;
				if (numOutOfBounds > 2) return false;
			}
			return true;
		}
		/// <summary>Do the points in this joint's recent history fall along nearby XY coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool arePointsCloseXY(List<Joint> joints, double Threshhold = 0.2)
        {
            int numOutOfBounds = 0;
            foreach (Joint j in joints)
            {
                if (j.Position.X - joints[0].Position.X > Threshhold || joints[0].Position.X - j.Position.X > Threshhold) numOutOfBounds++;
                if (j.Position.Y - joints[0].Position.Y > Threshhold || joints[0].Position.Y - j.Position.Y > Threshhold) numOutOfBounds++;
                if (numOutOfBounds > 2) return false;
            }
            return true;
        }
		/// <summary>Do the points in this joint's recent history fall along nearby XZ coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool arePointsCloseXZ(List<Joint> joints, double Threshhold = 0.2)
        {
            int numOutOfBounds = 0;
            foreach (Joint j in joints)
            {
                if (j.Position.X - joints[0].Position.X > Threshhold || joints[0].Position.X - j.Position.X > Threshhold) numOutOfBounds++;
                if (j.Position.Z - joints[0].Position.Z > Threshhold || joints[0].Position.Z - j.Position.Z > Threshhold) numOutOfBounds++;
                if (numOutOfBounds > 2) return false;
            }
            return true;
        }
		/// <summary>Do the points in this joint's recent history fall along nearby YZ coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool arePointsCloseYZ(List<Joint> joints, double Threshhold = 0.2)
        {
            int numOutOfBounds = 0;
            foreach (Joint j in joints)
            {
                if (j.Position.Y - joints[0].Position.Y > Threshhold || joints[0].Position.Y - j.Position.Y > Threshhold) numOutOfBounds++;
                if (j.Position.Z - joints[0].Position.Z > Threshhold || joints[0].Position.Z - j.Position.Z > Threshhold) numOutOfBounds++;
                if (numOutOfBounds > 2) return false;
            }
            return true;
        }
		/// <summary>Do the points in this joint's recent history fall along nearby XYZ coordinates?</summary>
		/// <param name="joints">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool arePointsCloseXYZ(List<Joint> joints, double Threshhold = 0.2)
        {
            int numOutOfBounds = 0;
            foreach (Joint j in joints)
            {
                if (j.Position.X - joints[0].Position.X > Threshhold || joints[0].Position.X - j.Position.X > Threshhold) numOutOfBounds++;
                if (j.Position.Y - joints[0].Position.Y > Threshhold || joints[0].Position.Y - j.Position.Y > Threshhold) numOutOfBounds++;
                if (j.Position.Z - joints[0].Position.Z > Threshhold || joints[0].Position.Z - j.Position.Z > Threshhold) numOutOfBounds++;
                if (numOutOfBounds > 2) return false;
            }
            return true;
        }
		/// <summary>On avearage, do these two joints fall along nearby XYZ coordinates?</summary>
		/// <param name="j1">List of joints representing one joint's recent history</param>
		/// <param name="j2">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool arePointsCloseXYZ(List<Joint> j1, List<Joint> j2, double Threshhold = 0.2)
        {
            if (!arePointsCloseXYZ(j1, Threshhold)) return false;
            if (!arePointsCloseXYZ(j2, Threshhold)) return false;

            if (j1.Average(j => j.Position.X) - j2.Average(j => j.Position.X) > Threshhold) return false;
            if (j2.Average(j => j.Position.X) - j1.Average(j => j.Position.X) > Threshhold) return false;

            if (j1.Average(j => j.Position.Y) - j2.Average(j => j.Position.Y) > Threshhold) return false;
            if (j2.Average(j => j.Position.Y) - j1.Average(j => j.Position.Y) > Threshhold) return false;

            if (j1.Average(j => j.Position.Z) - j2.Average(j => j.Position.Z) > Threshhold) return false;
            if (j2.Average(j => j.Position.Z) - j1.Average(j => j.Position.Z) > Threshhold) return false;
            
            return true;
        }
		/// <summary>On avearage, do these two joints fall along nearby XY coordinates?</summary>
		/// <param name="j1">List of joints representing one joint's recent history</param>
		/// <param name="j2">List of joints representing one joint's recent history</param>
		/// <param name="Threshhold">Maximum standard deviation</param>
		/// <returns>True for close; false for far or excessive out of bounds</returns>
		public static bool arePointsCloseXY(List<Joint> j1, List<Joint> j2, double Threshhold = 0.2)
        {
            if (!arePointsCloseXY(j1, Threshhold)) return false;
            if (!arePointsCloseXY(j2, Threshhold)) return false;

            if (j1.Average(j => j.Position.X) - j2.Average(j => j.Position.X) > Threshhold) return false;
            if (j2.Average(j => j.Position.X) - j1.Average(j => j.Position.X) > Threshhold) return false;

            if (j1.Average(j => j.Position.Y) - j2.Average(j => j.Position.Y) > Threshhold) return false;
            if (j2.Average(j => j.Position.Y) - j1.Average(j => j.Position.Y) > Threshhold) return false;

            return true;
        }

		#endregion Motion Filtering
	}
	public class GestureHandler
	{
		public const int ScrollsPer10cmH = 2, ScrollsPer10cmV = 3; // Full screen magnifier sensitivity. Decrease for smaller viewports. Sends X ctrl-alt-arrowkeys for every 10cm you move.
		public List<List<Joint>> JointHistory { get; set; } // Buffer containing recent joint positions, used for gesture detection
		public bool isGesturesEnabled { get; set; }	// Are we in Gesture Mode?
		public bool isFlip3Drunning { get; set; }			// Are we in Flip3D Mode?
		public bool isMagnified { get; set; }				// Are we in Magnifier mode?
		public bool isPieMenuShowing { get; set; }			// Are we showing pie menu(s)?
		public IntPtr SelectedWindow = new IntPtr(-1);
		public int JointBufferSize;	// JointHistory will be maintained at this many frames (30 is good)

		/// <summary>GestureHandler Constructor with pointless XML Summary</summary>
		/// <param name="_KinectMediaCenter">KinectMediaCenter object</param>
		/// <param name="jointBufferSize">How many frames should be in the jointBuffer? 30 is good.</param>
		public GestureHandler(int jointBufferSize)
		{
			JointBufferSize = jointBufferSize;
			JointHistory = new List<List<Joint>>();
			try { // Build initial joint history buffer
				for (int i = 0; i < (int)JointID.Count; i++)
					JointHistory.Add(new List<Joint>()); }
			catch {  }
		}
		/// <summary>Adds a Joint to the joint history buffer</summary>
		/// <param name="joint">Joint to add</param>
		public void AppendJointHistory(Joint joint)
		{
			if (JointHistory.Count() > 0)
				for (int i = 0; i < JointHistory.Count; i++)
					if (JointHistory[i].Count > JointBufferSize)
						JointHistory[i].RemoveRange(0, JointHistory[i].Count - JointBufferSize);
			JointHistory[(int)joint.ID].Add(joint);
		}

		#region Gesture Recognition

		#region Multi-Gesture Recognition

		/// <summary>Detects multi-point gestures.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesMultipoint()
		{
			if (DetectGestureMulti_DisableGestures()) return true;		// Disable Gestures gesture
			else if (DetectGestureMulti_InvokeFlip3D()) return true;	// Invoke Flip3D gesture
			else if (DetectGestureMulti_ZoomIn()) return true;			// Zoom In Flip3D gesture
			else if (DetectGestureMulti_ZoomOut()) return true;			// Zoom Out Flip3D gesture

			else return false; // No gestures detected
		}
		/// <summary>Detects the "Enable Gestures" gesture.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGestureMulti_EnableGestures()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < JointBufferSize / 2)
				return false;
			if (Gestures.isMovedUp(JointHistory[(int)JointID.HandRight], util.InchesToMeters(18))				// right hand moved up
					&& Gestures.isMovedUp(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(18))			// left hand moved up
					&& Gestures.arePointsCloseXZ(JointHistory[(int)JointID.HandRight], util.InchesToMeters(18))	// right hand did not deviate much from Y path along X and Z axes
					&& Gestures.arePointsCloseXZ(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(18)))
			{// left  hand did not deviate much from Y path along X and Z axes
				isGesturesEnabled = true;
				JointHistory[(int)JointID.HandLeft].Clear();
				JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			return false;
		}
		/// <summary>Detects the "Disable Gestures" gesture.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGestureMulti_DisableGestures()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < JointBufferSize - 5)
				return false;
			if (Gestures.isMovedDown(JointHistory[(int)JointID.HandRight], util.InchesToMeters(24))				// right hand moved down
					&& Gestures.isMovedDown(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(24))		// left hand moved down
					&& Gestures.arePointsCloseXZ(JointHistory[(int)JointID.HandRight], util.InchesToMeters(12))	// right hand did not deviate much from Y path along X and Z axes
					&& Gestures.arePointsCloseXZ(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(12)))
			{// left hand did not deviate much from Y path along X and Z axes
				isGesturesEnabled = false;
				JointHistory[(int)JointID.HandLeft].Clear();
				JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			return false;
		}
		/// <summary>Detects the "Invoke Flip3D" gesture.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGestureMulti_InvokeFlip3D()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < JointBufferSize - 5)
				return false;

			if (Gestures.isMovedLeft(JointHistory[(int)JointID.HandRight], util.InchesToMeters(12))				// Right hand moved left
					&& Gestures.isMovedRight(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(12))		// Left hand moved right
					&& Gestures.arePointsCloseYZ(JointHistory[(int)JointID.HandRight], util.InchesToMeters(12))	// Right hand did not deviate much along X path based on YZ locations
					&& Gestures.arePointsCloseYZ(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(12)))
			{// Left hand did not deviate much along X path based on YZ locations
				isFlip3Drunning = true;
				InvokeFlip3D(); // Invokes Flip3D
				JointHistory[(int)JointID.HandLeft].Clear();
				JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			return false;
		}

		// Flip3D gestures
		/// <summary>Detects all gestures valid in Flip3D mode</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesMultipoint_InFlip3DMode()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < JointBufferSize - 5)
				return false;

			if (DetectGesturesMultiCancelFlip3D()) return true;		// Detect "Cancel Flip3D" gesture
			else if (DetectGesturesMultiFlip3DFwd()) return true;	// Detect "Flip forward" gesture
			else if (DetectGesturesMultiFlip3DBack()) return true;	// Detect "Flip backwards" gesture
			else return false; // No gesture detected
		}
		/// <summary>Detects the "Cancel Flip3D" gesture.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesMultiCancelFlip3D()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < JointBufferSize - 5)
				return false;
			if (Gestures.isMovedRight(JointHistory[(int)JointID.HandRight], util.InchesToMeters(12))					// Left hand moved left
					&& Gestures.isMovedLeft(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(12))			// Right hand moved right
					&& Gestures.arePointsCloseYZ(JointHistory[(int)JointID.HandRight], util.InchesToMeters(12))		// Right hand didn't deviate much from the X axis
					&& Gestures.arePointsCloseYZ(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(12)))
			{	// Left hand didn't deviate much from the X axis
				isFlip3Drunning = false;
				System.Windows.Forms.SendKeys.SendWait("{ENTER}"); // "Enter" closes Flip3D and selects the window
				JointHistory[(int)JointID.HandLeft].Clear();
				JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			return false;
		}
		/// <summary>Detects the Flip3D "Flip Forward" gesture</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesMultiFlip3DFwd()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < 5)
				return false;

			if (Gestures.arePointsCloseXY(JointHistory[(int)JointID.HandLeft], JointHistory[(int)JointID.HandRight], 0.3) // Hands do not deviate much from XY axes, and close to each other.
					&& Gestures.isMovedOut(JointHistory[(int)JointID.HandLeft], 0.02)	// Left hand moves back away from sensor
					&& Gestures.isMovedIn(JointHistory[(int)JointID.HandRight], 0.02))
			{// Right hand moves in toward sensor
				System.Windows.Forms.SendKeys.SendWait("{UP}"); // The Up arrow scrolls Flip3D back. We're following the RIGHT hand here.
				JointHistory[(int)JointID.HandLeft].Clear();
				JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			return false;
		}
		/// <summary>Detects the Flip3D "Flip Backwards" gesture</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesMultiFlip3DBack()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < 5)
				return false;

			if (Gestures.arePointsCloseXY(JointHistory[(int)JointID.HandLeft], JointHistory[(int)JointID.HandRight], 0.3) // Hands do not deviate much from XY axes, and close to each other.
					&& Gestures.isMovedIn(JointHistory[(int)JointID.HandLeft], 0.02)		// Left hand moves in toward sensor
					&& Gestures.isMovedOut(JointHistory[(int)JointID.HandRight], 0.02))
			{	// Right hand moves back away from sensor
				System.Windows.Forms.SendKeys.SendWait("{DOWN}"); // The Down arrow scrolls Flip3D forward. We're following the RIGHT hand here.
				JointHistory[(int)JointID.HandLeft].Clear();
				JointHistory[(int)JointID.HandRight].Clear();
				return true;
			}
			return false;
		}

		#endregion Multi-Gesture Recognition
		#region Magnified Gesture Recognition

		/// <summary>Detects gestures while in Magnified mode.</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGesturesMagnified()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < 5)
				return false;

			else if (Magnified_HipMoved()) return true;				// Hip moved. Scroll sideways.
			else if (DetectGestureMulti_ZoomOut()) return true;		// Zoom out gesture
			else if (Magnified_LH()) return true;					// Left hand gestures (i.e. up/down to scroll vertically)
			else if (DetectGestureMulti_InvokeFlip3D()) return true;// Invoke Flip3D gesture

			else return false;
		}
		/// <summary>Detects the "Magnifier Zoom In" gesture</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGestureMulti_ZoomIn()
		{
			if (JointHistory.Count == 0 || JointHistory[0].Count < JointBufferSize - 5)
				return false;

			if (Gestures.isMovedUpLeft(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(18))			// Left hand moved up and left
				&& Gestures.isMovedUpRight(JointHistory[(int)JointID.HandRight], util.InchesToMeters(18)))
			{	// Right hand moved up and right
				ZoomIn();
				return true;
			}
			return false;
		}
		/// <summary>Detects the "Magnifier Zoom Out" gesture</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool DetectGestureMulti_ZoomOut()
		{
			if (Gestures.isMovedDownRight(JointHistory[(int)JointID.HandLeft], util.InchesToMeters(18))		// Left hand moved down and right
				&& Gestures.isMovedDownLeft(JointHistory[(int)JointID.HandRight], util.InchesToMeters(18)))
			{// Right hand moved down and left
				ZoomOut();
				return true;
			}
			return false;
		}
		/// <summary>Detects "Magnifier scroll sideways" gesture (hip moved while magnified)</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool Magnified_HipMoved()
		{
			List<Joint> joints = JointHistory[(int)JointID.HipCenter];
			if (joints.Count < 5)
				return false;

			// Get distance moved
			double delta = joints.Last().Position.X - joints.First().Position.X;
			if (delta > -.1 && delta < .1)
				return false;

			// SendKeys acts weird.  You have to send it strings. In this case, we want to hold down ctrl and alt while pressing arrowkeys a bunch of times.
			string keys = "%^("; // ctrl-alt-
			if (delta < 0)
			{
				for (int i = 0; i < ScrollsPer10cmH * 100; i++)
					keys += "{LEFT}";
			}
			else
			{
				for (int i = 0; i < ScrollsPer10cmH * 100; i++)
					keys += "{RIGHT}";
			}
			SendKeys.SendWait(keys + ")");

			JointHistory[(int)JointID.HipCenter].Clear();
			return true;
		}
		/// <summary>Detects "Magnifier scroll vertically" gesture (left hand)</summary>
		/// <returns>true if a gesture was recognized and executed; else false</returns>
		public bool Magnified_LH()
		{
			List<Joint> joints = JointHistory[(int)JointID.HandLeft];
			if (joints.Count < JointBufferSize)
				return false;

			if (Gestures.isMovedUp(joints, util.InchesToMeters(12)) && Gestures.arePointsCloseXZ(joints))
			{
				string keys = "%^(";
				for (int i = 0; i < ScrollsPer10cmV * 100; i++)
					keys += "{UP}";
				SendKeys.SendWait(keys + ")");
				JointHistory[(int)JointID.HandLeft].Clear();
				return true;
			}
			else if (Gestures.isMovedDown(joints, util.InchesToMeters(12)) && Gestures.arePointsCloseXZ(joints))
			{
				string keys = "%^(";
				for (int i = 0; i < ScrollsPer10cmV * 100; i++)
					keys += "{DOWN}";
				SendKeys.SendWait(keys + ")");
				JointHistory[(int)JointID.HandLeft].Clear();
				return true;
			}
			else return false;
		}

		#endregion Magnified Gesture Recognition

		#endregion Gesture Recognition
		#region Gesture Handling

		/// <summary>Sends a mousewheel scroll to whatever window is active.</summary>
		/// <param name="direction">True for up, false for down</param>
		/// <param name="distance">Number of (pixels?) to scroll</param>
		public void ScrollMouse(bool direction, int distance)
		{
			util.INPUT mi = new util.INPUT() { type = 0 };
			mi.union.mouseInput.dx = 0;
			mi.union.mouseInput.dy = 0;
			mi.union.mouseInput.mouseData = direction ? distance : -distance;
			mi.union.mouseInput.dwFlags = HardwareEvents.MouseWheel;
			mi.union.mouseInput.time = 0;
			mi.union.mouseInput.dwExtraInfo = new IntPtr(0);

			try { SendInput(1, ref mi, Marshal.SizeOf(mi)); }
			catch { }
		}
		/// <summary>Maximizes or Minimizes the active window.</summary>
		/// <param name="direction">True for maximized, false for minimized, null for normal</param>
		public void ResizeWindow(bool? direction)
		{
			// Get the handle and current state of the active window
			IntPtr active = GetForegroundWindow();

			WindowState mode = (WindowState)GetWindowState(active);

			// Making it normal is easy:
			if (direction == null)
				ShowWindowAsync(active, 1);

			switch (mode)
			{
				case WindowState.Maximized:
				case WindowState.Normal:
					ShowWindowAsync(active, (int)WindowState.Normal); break;
				case WindowState.Minimized: ShowWindowAsync(active, (bool)direction ? (int)WindowState.Maximized : (int)WindowState.Minimized); break;
			}
		}
		/// <summary>Gets the WindowState of a target window</summary>
		/// <param name="handle">Handle to the target window</param>
		/// <returns>WindowState of the window</returns>
		public WindowState GetWindowState(IntPtr handle)
		{
			util.WINDOWPLACEMENT placement = new util.WINDOWPLACEMENT();
			placement.length = Marshal.SizeOf(placement);
			GetWindowPlacement(handle, ref placement);
			return (WindowState)placement.flags;
		}
		/// <summary>Invokes Flip3D.</summary>
		public void InvokeFlip3D()
		{	// Yep. Hackish, but small and effective.
			Process.Start("RunDll32", "DwmApi #105");
		}
		/// <summary>Handles right hand push gesture ("start moving window")</summary>
		public void HandleRHPush()
		{
			SelectedWindow = GetForegroundWindow();
		}
		/// <summary>Moves a target window to a location represented by a target joint.</summary>
		/// <param name="handle">Handle to target window</param>
		/// <param name="joints">List of Joints from the right hand. Math will be done to convert it into pixels.</param>
		/// <returns></returns>
		public void MoveWindow(IntPtr handle, List<Joint> joints)
		{
			if (joints.Count < 1 || (int)SelectedWindow == -1) return;

			int x = 2000 + (int)(joints.Last().Position.X * 4000D), y = -(int)(joints.Last().Position.Y * 3000D);
			MoveWindow(handle, new Point(x, y));
		}
		/// <summary>Moves a target window to a target location.</summary>
		/// <param name="handle">Handle to target window</param>
		/// <param name="point">Point to move window to</param>
		public void MoveWindow(IntPtr handle, Point point)
		{
			// Find the current window size
			if ((int)handle == -1) return;
			util.RECT rect = new util.RECT();
			GetWindowRect(SelectedWindow, out rect);

			SetWindowPos(handle, // which window to set
				new IntPtr((int)util.SpecialWindowHandles.HWND_TOP), // ?
				 (int)point.X, // x coordinate / left
				 (int)point.Y,  // y coordinate / top
				 rect._Right - rect._Left, // x size / width
				 rect._Bottom - rect._Top, // y size / height
				 util.SetWindowPosFlags.ShowWindow); // some Win32 flag thing
		}
		/// <summary>Handles right hand pull gesture ("stop moving window")</summary>
		public void HandleRHPull()
		{
			// Get a handle to the active window
			IntPtr handle = SelectedWindow;
			if ((int)handle == -1)
				return;

			// Stop tracking that window
			SelectedWindow = new IntPtr(-1);
			JointHistory[(int)JointID.HandRight].Clear();
			// Don't maximize anymore. Hell with it; just keep it its old size. For some reason this only works ONCE, and even then, only if Magnifier isn't running. Wait, what?
			//ShowWindowAsync(handle, (int)WindowState.Maximized);
		}
		/// <summary>Performs Magnifier zoom-in</summary>
		public void ZoomIn()
		{
			JointHistory[(int)JointID.HandLeft].Clear();
			JointHistory[(int)JointID.HandRight].Clear();

			// Doesn't work if Magnifier is already running. Make sure it's not.
			foreach (Process process in Process.GetProcessesByName("magnify"))
				process.Kill();

			// Send keys Win Plus Plus
			keybd_event(0x5B, 0x45, 0, new UIntPtr(0)); // lwin
			keybd_event(0xBB, 0x45, 0, new UIntPtr(0)); // +
			keybd_event(0xBB, 0x45, 0, new UIntPtr(0)); // +
			isMagnified = true;
		}
		/// <summary>Performs Magnifier zoom-out</summary>
		public void ZoomOut()
		{
			JointHistory[(int)JointID.HandLeft].Clear();
			JointHistory[(int)JointID.HandRight].Clear();

			// Send keys Win Minus Minus
			keybd_event(0x5B, 0x45, 0, new UIntPtr(0)); // lwin
			keybd_event(0xBD, 0x45, 0, new UIntPtr(0)); // -
			keybd_event(0xBD, 0x45, 0, new UIntPtr(0)); // -

			// We've no further use for you.
			foreach (Process process in Process.GetProcessesByName("magnify"))
				process.Kill();
			isMagnified = false;
		}

		#endregion
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
	
	}
}
