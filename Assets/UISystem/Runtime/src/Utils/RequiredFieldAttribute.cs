/// Created by: Kirk George
/// Copyright: Kirk George
/// Website: https://github.com/foozlemoozle?tab=repositories
/// See upload date for date created.

/**
Created by Kirk George 05/28/2019.!-- 
Allows you to mark serialized fields as required.!-- 
Adds validation hook for required fields.!--
Uses Reflection--validation during run-time is not effecient.!--  
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

using Attribute = System.Attribute;
using AttributeTargets = System.AttributeTargets;
using Type = System.Type;

[System.AttributeUsage( AttributeTargets.Property | AttributeTargets.Field )]
public class RequiredFieldAttribute : Attribute 
{
}

public static class RequiredFieldValidator
{
	private static readonly BindingFlags _BINDINGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.GetField;

	public static bool PassesRequiredFieldValidation( Object thing, out List<string> _failedFields )
	{
		bool isValid = true;
		int ffIndex = 0;
		_failedFields = new List<string>();

		Type thingType = thing.GetType();
		int count;
		
		MemberInfo[] members = thingType.GetMembers( _BINDINGS );
		count = members.Length;
		for( int i = 0; i < count; ++i )
		{
			MemberInfo member = members[i];
			if( member.GetCustomAttributes( typeof( RequiredFieldAttribute ), false ).Length <= 0 )
			{
				continue;
			}

			bool memberValid = true;

			if( member.MemberType == MemberTypes.Field )
			{
				FieldInfo field = (FieldInfo)member;
				memberValid = field.GetValue( thing ) != null;
			}
			else if( member.MemberType == MemberTypes.Property )
			{
				PropertyInfo property = (PropertyInfo)member;
				memberValid = property.GetValue( thing, new object[0]{} ) != null;
			}

			if( !memberValid )
			{
				_failedFields[ffIndex] = member.Name;
			}
			isValid = isValid && memberValid;
		}

		return isValid;
	}
}
