/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George.!--
 */

using UnityEngine;

public static class GameObjectUtils 
{
	public static void SetActiveIfNeeded( this GameObject thing, bool active )
	{
		if( thing != null && thing.activeSelf != active )
		{
			thing.SetActive( active );
		}
	}
}
