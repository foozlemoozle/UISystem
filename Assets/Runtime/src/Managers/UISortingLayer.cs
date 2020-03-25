/**
Created by Kirk George 05/27/2019.!--
Definitions for UI layers and some helper functions.!--
All values in Layers should match a sorting layer in unity's Tags and Layers.!--
 */

using System.Collections.Generic;

public static class UISortingLayer
{
	//all values here should be yield returned in GetEnumerator().
	//Keep in render order back to front.
	public enum Layers
	{
		UIManager = -2,//this should always be the lowest value, this should not be in the layers settings in Unity

		Background = -1,
		Default = 0,
		Modal = 1,
		Tutorial = 2,
		Notification = 3,
		Error = 4,
		Debug = 5
	}

	public class LayerComparer : IEqualityComparer<Layers>
	{
		private static LayerComparer _INSTANCE;
		public static LayerComparer Get()
		{
			if( _INSTANCE == null )
			{
				_INSTANCE = new LayerComparer();
			}

			return _INSTANCE;
		}

		private LayerComparer(){}

		public bool Equals( Layers a, Layers b )
		{
			return a == b;
		}

		public int GetHashCode( Layers layer )
		{
			return (int)layer;
		}
	}

	//update this when adding a new layer
	public static IEnumerator<Layers> GetEnumerator()
	{
		//we do not yield return for the UIManager layer
		yield return Layers.Background;
		yield return Layers.Default;
		yield return Layers.Modal;
		yield return Layers.Tutorial;
		yield return Layers.Notification;
		yield return Layers.Error;
		yield return Layers.Debug;
	}

	//update this when adding a new layer
	public static int count { get { return 7; } }
}
