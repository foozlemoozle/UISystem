/**
Created by Kirk George 05/28/2019.!-- 
Frame that sits within a canvas layer.!-- 
Allows the effective screen to change shape depending on device notches.!--
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent( typeof( Canvas ) )]
public class DeviceFrame : MonoBehaviour
{
	public enum Panels
	{
		Header = 1,
		Footer = 2,
		Left = 4,
		Right = 8
	}

	[SerializeField]
	[RequiredField]
	private RectTransform _dialogContainer;
	[SerializeField]
	[RequiredField]
	private RectTransform _fullScreenContainer;
	
	[SerializeField]
	private RectTransform _header;
	[SerializeField]
	private RectTransform _footer;
	[SerializeField]
	private RectTransform _left;
	[SerializeField]
	private RectTransform _right;

	private Canvas _canvas;
	public Canvas canvas { get { if( _canvas == null ){ _canvas = this.GetComponent<Canvas>(); } return _canvas; } }

	public void ResizeSafeArea( float top, float bottom, float left, float right )
	{
		_dialogContainer.offsetMin = new Vector2( left, top );
		_dialogContainer.offsetMax = new Vector2( right, bottom );

		ResizeHeader( top );
		ResizeFooter( bottom );
		ResizeLeft( left );
		ResizeRight( right );
	}

	public void AttachUI( UIView ui )
	{
		Transform hook = ui.ignoreFrameLayout ? _fullScreenContainer : _dialogContainer;
		ui.transform.SetParent( hook, false );
	}

	public void ShowPanels( Panels toShow )
	{
		_header.gameObject.SetActiveIfNeeded( ( toShow & Panels.Header ) != 0 );
		_footer.gameObject.SetActiveIfNeeded( ( toShow & Panels.Footer ) != 0 );
		_left.gameObject.SetActiveIfNeeded( ( toShow & Panels.Left ) != 0 );
		_right.gameObject.SetActiveIfNeeded( ( toShow & Panels.Right ) != 0 );
	}

	private void ResizeHeader( float top )
	{
		if( _header == null )
		{
			return;
		}

		_header.sizeDelta = new Vector2(
			_header.sizeDelta.x,
			top
		);
	}

	private void ResizeFooter( float bottom )
	{
		if( _footer == null )
		{
			return;
		}

		_footer.sizeDelta = new Vector2( 
			_footer.sizeDelta.x,
			bottom
		);
	}

	private void ResizeLeft( float left )
	{
		if( _left == null )
		{
			return;
		}

		_left.sizeDelta = new Vector2( 
			left,
			_left.sizeDelta.y
		);
	}

	private void ResizeRight( float right )
	{
		if( _right == null )
		{
			return;
		}

		_right.sizeDelta = new Vector2(
			right,
			_right.sizeDelta.y
		);
	}
}
