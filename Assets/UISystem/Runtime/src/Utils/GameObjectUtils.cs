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
