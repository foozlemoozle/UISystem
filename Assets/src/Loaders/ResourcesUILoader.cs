﻿/**
Created by Kirk George 05/23/2019.!--
Handles loading a UI through Resources.Load.!--
THIS SHOULD NOT BE USED SERIOUSLY.!--
Implement a loaders that make sense for your project.!--
 */

using UnityEngine;

public class ResourcesUILoader<T> : Loader<T> where T : UIView
{
	private string _assetPath;

    public ResourcesUILoader( string assetPath )
    {
		_assetPath = assetPath;
    }

	public override void StartLoad()
	{
		T asset = Resources.Load<T>( _assetPath );
		OnLoaded( asset.gameObject );
	}
}
