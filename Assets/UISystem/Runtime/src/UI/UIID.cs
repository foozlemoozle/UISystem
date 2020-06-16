/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George 05/23/2019.!--
Unique identifier for a UI.!--
*/

public struct UIID
{
	public class UIIDComparer : System.Collections.Generic.IEqualityComparer<UIID>
	{
		private static UIIDComparer _INSTANCE;
		public static UIIDComparer Get()
		{
			if( _INSTANCE == null )
			{
				_INSTANCE = new UIIDComparer();
			}

			return _INSTANCE;
		}

		private UIIDComparer()
		{
		}

		public bool Equals( UIID a, UIID b )
		{
			return a._id == b._id;
		}

		public int GetHashCode( UIID a )
		{
			return (int)a._id;
		}
	}

	public static UIID Generate( CullSettings cullSettings = CullSettings.NoCullNoClear )
	{
		UIID created = new UIID( NEXT, cullSettings );
		++NEXT;

		return created;
	}

	private static long NEXT = 0;

	private long _id;
	public long id { get { return _id; } }
	private CullSettings _cullSettings;
	public CullSettings cullSettings { get { return _cullSettings; } }

	private UIID( long id, CullSettings cullSettings )
	{
		_id = id;
		_cullSettings = cullSettings;
	}
}
