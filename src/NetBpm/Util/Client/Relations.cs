using System;
using System.Collections;
using log4net;

namespace NetBpm.Util.Client
{
	
	/// <summary> specifies which related objects should be resolved for data-transfer-objects,
	/// returned from a session-facade.
	/// 
	/// <p>A session facade is supposed to collect data from the persistency-layer
	/// and return data-transfer-objects to the client.  If the datamodel has a lot
	/// of relations, a problem arises : Which related objects should be resolved in
	/// the object that is returned to the client.  A Relations-object is a convenient way of specifying
	/// which related objects you want to be resolve on the objects returned by the session facade.
	/// You can specify these Relations with dot-separated property-names.
	/// E.g. if the returned object is a Flow, "attributeInstance.attribute" specifies that
	/// for every returned Flow-object the methods Collection getAttributeInstance() must
	/// be resolved and that these AttributeInstances have the Attribute getAttribute()
	/// resolved. 
	/// </p>
	/// 
	/// </summary>
	public class Relations
	{
		private IDictionary _relationsMap = new Hashtable();
		private static readonly ILog log = LogManager.GetLogger( typeof(Relations) );

		virtual public IDictionary RelationsMap
		{
			get
			{
				return _relationsMap;
			}
			
		}
		
		/// <summary> creates a Relation from one dot-separated property descriptor.</summary>
		/// <param name="property">is a dot-separated property descriptor.
		/// </param>
		public Relations(String property)
		{
			if ((Object) property != null)
			{
				Add(property);
			}
		}
		
		/// <summary> creates a Relation from multiple dot-separated property descriptor.</summary>
		/// <param name="properties">is a dot-separated property descriptor.
		/// </param>
		public Relations(String[] properties)
		{
			if (properties != null)
			{
				for (int i = 0; i < properties.Length; i++)
				{
					Add(properties[i]);
				}
			}
		}
		
		private void  Add(String fullyQualifiedProperty)
		{
			String atomicProperty = null;
			String subProperty = null;
			
			int dotIndex = fullyQualifiedProperty.IndexOf('.');
			
			if (dotIndex != - 1)
			{
				atomicProperty = fullyQualifiedProperty.Substring(0, dotIndex);
				subProperty = fullyQualifiedProperty.Substring(dotIndex + 1);
			}
			else
			{
				atomicProperty = fullyQualifiedProperty;
			}
			
			Relations subRelation = (Relations) _relationsMap[atomicProperty];
			if (subRelation != null)
			{
				subRelation.Add(subProperty);
			}
			else
			{
				if ((Object) subProperty != null)
				{
					subRelation = new Relations(subProperty);
				}
			}
			
			_relationsMap[atomicProperty] = subRelation;
		}
		
		public void Resolve(Object object_Renamed)
		{
			Resolve(object_Renamed, _relationsMap);
		}
		
		private static void  Resolve(Object object_Renamed, IDictionary relationsMap)
		{
			if (object_Renamed == null)
				return ;
			if (object_Renamed is ICollection)
			{
				IEnumerator iter = ((ICollection) object_Renamed).GetEnumerator();
				while (iter.MoveNext())
				{
					Object element = iter.Current;
					ResolveObject(element, relationsMap);
				}
			}
			else if (object_Renamed is Object[])
			{
				Object[] objectArray = object_Renamed as Object[];
				for (int i = 0; i < objectArray.Length; i++)
				{
					ResolveObject(objectArray[i], relationsMap);
				}
			}
			else
			{
				ResolveObject(object_Renamed, relationsMap);
			}
		}
		
		private static void  ResolveObject(Object persistentObject, IDictionary relationsMap)
		{
			if (relationsMap == null)
				return ;
			
			IEnumerator iter = relationsMap.GetEnumerator();
			while (iter.MoveNext())
			{
				DictionaryEntry entry = (DictionaryEntry) iter.Current;
				String propertyName = (String) entry.Key;
				
				IDictionary rest = null;
				Relations relations = (Relations) entry.Value;
				if (relations != null)
				{
					rest = relations._relationsMap;
				}
				
				try
				{
					String getterName = propertyName.Substring(0, 1).ToUpper() + propertyName.Substring(1);
					System.Reflection.PropertyInfo prop = persistentObject.GetType().GetProperty(getterName);
//					Type type=persistentObject.GetType();
					Object agreggatedObject = prop.GetValue(persistentObject,null);
					// log.Debug( "agreggatedObject: " + agreggatedObject );
					if (agreggatedObject != null)
					{
						Resolve(agreggatedObject, rest);
					}
				}
				catch (System.Exception e)
				{
					log.Error("can't resolve property " + propertyName + " for object " + persistentObject + " : " + e.Message, e);
				}
			}
		}
	}
}