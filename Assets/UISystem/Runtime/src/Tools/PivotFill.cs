/**
 * Created by Kirk George 03/29/2020
 * Positions a UI element by X, Y position,
 * then fills Left, Right, Top, or Bottom.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PivotFill : UIBehaviour
{
    [SerializeField]
    private bool _fillTop;
    [SerializeField]
    private bool _fillBottom;
    [SerializeField]
    private bool _fillLeft;
    [SerializeField]
    private bool _fillRight;

    private bool _waitingForParentUpdate = false;

    private RectTransform rTransform
    {
        get
        {
            if( _rTransform == null )
            {
                _rTransform = this.transform as RectTransform;
            }
            if( _rTransform == null )
            {
                throw new System.NullReferenceException( string.Format( "Couln't convert GO {0} transform to rectTransform--invalid UI type.", this.name ) );
            }

            return _rTransform;
        }
    }
    private RectTransform _rTransform;

    public void Resize()
    {
        RectTransform parent = ( this.transform.parent as RectTransform );

        Vector3 position = rTransform.localPosition;
        Vector2 pivot = rTransform.pivot;
        Vector2 anchorMax = new Vector2( 0.5f, 0.5f );
        Vector2 anchorMin = new Vector2( 0.5f, 0.5f );
        Vector2 sizeDelta = rTransform.sizeDelta;
        Vector2 parentPivot = parent.pivot;
        Vector2 parentSizeDelta = parent.sizeDelta;

        float parentWidth = parent.rect.width;
        float parentHeight = parent.rect.height;

        pivot.x = 0.5f - ( _fillRight ? 0.5f : 0 ) + ( _fillLeft ? 0.5f : 0 );
        pivot.y = 0.5f - ( _fillTop ? 0.5f : 0 ) + ( _fillBottom ? 0.5f : 0 );

        Vector2 rPivot = new Vector2(
            ( position.x + 0.5f * parentWidth ) / parentWidth,
            ( position.y + 0.5f * parentHeight ) / parentHeight
            );

        if( parentHeight == 0 )
        {
            _waitingForParentUpdate = true;
            return;
        }
        else
        {
            _waitingForParentUpdate = false;
        }

        //calculate the appropriate height and width
        if( _fillLeft || _fillRight )
        {
            sizeDelta.x = ( _fillLeft ? rPivot.x * parentWidth : 0 ) + ( _fillRight ? ( 1 - rPivot.x ) * parentWidth : 0 );
        }
        if( _fillTop || _fillBottom )
        {
            sizeDelta.y = ( _fillTop ? ( 1 - rPivot.y ) * parentHeight : 0 ) + ( _fillBottom ? rPivot.y * parentHeight : 0 );
        }

        //modify the position if both values in each axis are the same
        if( _fillLeft && _fillRight )
        {
            position.x = 0;
        }
        if( _fillTop && _fillBottom )
        {
            position.y = 0;
        }

        rTransform.pivot = pivot;
        rTransform.anchorMax = anchorMax;
        rTransform.anchorMin = anchorMin;
        rTransform.sizeDelta = sizeDelta;
        rTransform.localPosition = position;
    }

    private void LateUpdate()
    {
        if( _waitingForParentUpdate )
        {
            Resize();
        }
    }

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        Resize();
    }

    protected override void OnTransformParentChanged()
    {
        base.OnTransformParentChanged();
        Resize();
    }
}

#if UNITY_EDITOR
[CustomEditor( typeof( PivotFill ) )]
[CanEditMultipleObjects]
public class PivotFillEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if( GUILayout.Button( "Refresh" ) )
        {
            ( target as PivotFill ).Resize();
        }

        base.OnInspectorGUI();
    }
}
#endif
