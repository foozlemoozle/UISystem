﻿/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George 05/27/2019.!--
Exceptions for the UI system.!--
 */

namespace com.keg.uisystem
{
	public class UIException : System.Exception
	{
		public UIException( string message ) : base( message ) { }
		public UIException( string message, System.Exception inner ) : base( message, inner ) { }
	}
	public class UILayerNotFoundException : UIException
	{
		public UILayerNotFoundException( string message ) : base( message ) { }
	}

	public class UIManagerSetupException : UIException
	{
		public UIManagerSetupException() : base( "setup called after initialization already complete" ) { }
	}
}
