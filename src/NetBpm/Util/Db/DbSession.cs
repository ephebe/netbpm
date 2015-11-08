using System;
using System.Collections;
using System.Runtime.Serialization;
using log4net;
using NetBpm.Util.Net;
using NHibernate;
using NHibernate.Type;

namespace NetBpm.Util.DB
{
	/// <summary> This class wraps the Hibernate Session.
	/// It adds the guarantee that if a method throws an exception, the session will already be closed.
	/// Furthermore, this wrapper adds 2 convenience-methods findOne(...) and iterateOne(...) that checks if exactly one object is returned and extracts that single object from the collection. 
	/// </summary>
	public class DbSession
	{
		private ISession _session = null;
		private static readonly ILog log = LogManager.GetLogger(typeof (DbSession));

		public FlushMode FlushMode
		{
			get
			{
				try
				{
					FlushMode f = _session.FlushMode;
				}
				catch (Exception t)
				{
					HandleDatabaseException(t, "getFlushMode()");
				}
				return _session.FlushMode;
			}

			set
			{
				try
				{
					_session.FlushMode = value;
				}
				catch (Exception t)
				{
					HandleDatabaseException(t, "setFlushMode(" + value + ")");
				}
			}

		}

		public bool Connected
		{
			get
			{
				bool b = false;
				try
				{
					b = _session.IsConnected;
				}
				catch (Exception t)
				{
					HandleDatabaseException(t, "isConnected()");
				}
				return b;
			}

		}

		public bool Open
		{
			get
			{
				bool b = false;
				try
				{
					b = _session.IsOpen;
				}
				catch (Exception t)
				{
					HandleDatabaseException(t, "isOpen()");
				}
				return b;
			}

		}

		public ITransaction Transaction
		{
			get { return _session.Transaction; }
		}

		/* package private */

		internal DbSession(ISession session)
		{
			this._session = session;
		}

		public ICollection Filter(Object collection, String filter)
		{
			ICollection c = null;
			try
			{
				//c = _session.Filter(collection, filter);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "filter(collection," + filter + ")");
			}
			return c;
		}

		public ICollection Filter(Object collection, String filter, Object[] values, IType[] types)
		{
			ICollection c = null;
			try
			{
				//c = _session.Filter(collection, filter, values, types);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "filter(collection," + filter + "," + ArrayUtil.ToString(values) + ",types)");
			}
			return c;
		}

		public ICollection Filter(Object collection, String filter, Object valueObject, IType type)
		{
			ICollection c = null;
			try
			{
				//c = _session.Filter(collection, filter, valueObject, type);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "filter(collection," + filter + ",value,type)");
			}
			return c;
		}

		public IQuery GetNamedQuery(String queryName)
		{
			IQuery q = null;
			try
			{
				q = _session.GetNamedQuery(queryName);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "getNamedQuery(" + queryName + ")");
			}
			return q;
		}

		public IQuery CreateFilter(Object collection, String queryString)
		{
			IQuery q = null;
			try
			{
				q = _session.CreateFilter(collection, queryString);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "createFilter(collection," + queryString + ")");
			}
			return q;
		}

		public IQuery CreateQuery(String queryString)
		{
			IQuery q = null;
			try
			{
				q = _session.CreateQuery(queryString);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "createQuery(" + queryString + ")");
			}
			return q;
		}

		public Object Load(Type theClass, Int64 id)
		{
			Object o = null;
			try
			{
				o = _session.Load(theClass, id);
			}
			catch (NHibernate.ObjectNotFoundException e)
			{
				throw new ObjectNotFoundException(theClass, "id=" + id.ToString() + ". Excepton: " + e.Message);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "load(" + theClass.FullName + "," + id.ToString() + ")");
			}
			return o;
		}

		public Object Load(Type theClass, Int64 id, LockMode lockMode)
		{
			Object o = null;
			try
			{
				o = _session.Load(theClass, id, lockMode);
			}
			catch (ObjectNotFoundException e)
			{
				throw new ObjectNotFoundException(theClass, "id=" + id.ToString() + " Exception:" + e.Message);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "load(" + theClass.FullName + "," + id.ToString() + "," + lockMode + ")");
			}
			return o;
		}

		public void Load(Object object_Renamed, Int64 id)
		{
			try
			{
				_session.Load(object_Renamed, id);
			}
			catch (NHibernate.ObjectNotFoundException e)
			{
				throw new ObjectNotFoundException(object_Renamed.GetType(), "id=" + id.ToString() + " Exception: " + e.Message);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "load(" + object_Renamed + "," + id + ")");
			}
		}

		public IList Find(String query)
		{
			IList l = null;
			try
			{
				// log.Debug( "find-query(" + query + ")" );
				l = _session.CreateQuery(query).List();
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "find(" + query + ")");
			}
			return l;
		}

		public IList Find(String query, Object[] values, IType[] types)
		{
			IList l = null;
			try
			{
				// log.Debug( "find-query(" + query + "), parameters(" + ArrayUtil.toString( values ) + ")" );
				//l = _session.Find(query, values, types);
			    var qry =  _session.CreateQuery(query);
                for (int i = 0; i < values.Length; i++)
                {
                    qry = qry.SetParameter(i, values[i], types[i]);
                }

			    l = qry.List();

			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "find(" + query + "," + ArrayUtil.ToString(values) + ",types)");
			}
			return l;
		}

		public IList Find(String query, Object valueObject, IType type)
		{
			IList l = null;
			try
			{
				// log.Debug( "find-query(" + query + "), parameter(" + value + ")" );
				l = _session.CreateQuery(query).SetParameter(0,valueObject,type).List();
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "find(" + query + ",value,type)");
			}
			return l;
		}

		public Object FindOne(String query)
		{
			ICollection l = Find(query);
			if (l.Count == 0)
			{
				throw new ObjectNotFoundException(query);
			}
			else if (l.Count > 1)
			{
				throw new NotUniqueException(query, l.Count);
			}
			IEnumerator e = l.GetEnumerator();
			e.MoveNext();
			return e.Current;
		}

		public Object FindOne(String query, Object[] values, IType[] types)
		{
			ICollection l = Find(query, values, types);
			if (l.Count == 0)
			{
				throw new ObjectNotFoundException(query, values);
			}
			else if (l.Count > 1)
			{
				throw new NotUniqueException(query, values, l.Count);
			}
			IEnumerator e = l.GetEnumerator();
			e.MoveNext();
			return e.Current;
		}

		public Object FindOne(String query, Object valueObject, IType type)
		{
			ICollection l = Find(query, valueObject, type);
			if (l.Count == 0)
			{
				throw new ObjectNotFoundException(query, valueObject);
			}
			else if (l.Count > 1)
			{
				throw new NotUniqueException(query, valueObject, l.Count);
			}
			IEnumerator e = l.GetEnumerator();
			e.MoveNext();
			return e.Current;
		}

		public IEnumerable Iterate(String query)
		{
			IEnumerable i = null;
			try
			{
				// log.Debug( "iterate-query(" + query + ")" );
				i = _session.CreateQuery(query).Enumerable();
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "iterate(" + query + ")");
			}
			return i;
		}

		public IEnumerable Iterate(String query, Object[] values, IType[] types)
		{
			IEnumerable i = null;
			try
			{
				// log.Debug( "iterate-query(" + query + "), parameters(" + ArrayUtil.toString( values ) + ")" );
			    IQuery q = _session.CreateQuery(query);
			    for (int j = 0; j < values.Length; j++)
			    {
			        q = q.SetParameter(j, values[j], types[j]);
			    }

			    i = q.Enumerable();
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "iterate(" + query + "," + ArrayUtil.ToString(values) + ",types)");
			}
			return i;
		}

		public IEnumerable Iterate(String query, Object valueObject, IType type)
		{
			IEnumerable i = null;
			try
			{
				// log.Debug( "iterate-query(" + query + "), parameters(" + value + ")" );
				//i = _session.Enumerable(query, valueObject, type);
			    i = _session.CreateQuery(query).SetParameter(0, valueObject).List();
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "iterate(" + query + "," + valueObject + ",type)");
			}
			return i;
		}

		public Object IterateOne(String query)
		{
			IEnumerable e = Iterate(query);
			IEnumerator i = e.GetEnumerator();
			if (!i.MoveNext())
			{
				throw new ObjectNotFoundException(query);
			}
			Object o = i.Current;
			if (i.MoveNext())
			{
				int nbrOfObjectsFound = 1;
				while (i.MoveNext())
				{
					nbrOfObjectsFound++;
				}
				throw new NotUniqueException(query, nbrOfObjectsFound);
			}
			return o;
		}

		public Object IterateOne(String query, Object[] values, IType[] types)
		{
			IEnumerable e = Iterate(query, values, types);
			IEnumerator i = e.GetEnumerator();
			if (!i.MoveNext())
			{
				throw new ObjectNotFoundException(query, values);
			}
			Object o = i.Current;
			if (i.MoveNext())
			{
				int nbrOfObjectsFound = 1;
				while (i.MoveNext())
				{
					nbrOfObjectsFound++;
				}
				throw new NotUniqueException(query, values, nbrOfObjectsFound);
			}
			return o;
		}

		public Object IterateOne(String query, Object valueObject, IType type)
		{
			IEnumerable e = Iterate(query, valueObject, type);
			IEnumerator i = e.GetEnumerator();
			if (!i.MoveNext())
			{
				throw new ObjectNotFoundException(query, valueObject);
			}
			Object o = i.Current;
			if (i.MoveNext())
			{
				int nbrOfObjectsFound = 1;
				while (i.MoveNext())
				{
					nbrOfObjectsFound++;
				}
				throw new NotUniqueException(query, valueObject, nbrOfObjectsFound);
			}
			return o;
		}

		public LockMode GetCurrentLockMode(Object object_Renamed)
		{
			LockMode l = null;
			try
			{
				l = _session.GetCurrentLockMode(object_Renamed);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "getCurrentLockMode(" + object_Renamed + ")");
			}
			return l;
		}

		public void Lock(Object object_Renamed, LockMode lockMode)
		{
			try
			{
				_session.Lock(object_Renamed, lockMode);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "load(" + object_Renamed + "," + lockMode + ")");
			}
		}

		public object GetIdentifier(Object object_Renamed)
		{
			object s = null;
			try
			{
				s = _session.GetIdentifier(object_Renamed);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "getIdentifier(" + object_Renamed + ")");
			}
			return s;
		}

		public void Delete(Object object_Renamed)
		{
			try
			{
				_session.Delete(object_Renamed);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "delete(" + object_Renamed + ")");
			}
		}

		public int Delete(String query)
		{
			int i = - 1;
			try
			{
				i = _session.Delete(query);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "delete(" + query + ")");
			}
			return i;
		}

		public int Delete(String query, Object[] values, IType[] types)
		{
			int i = - 1;
			try
			{
				i = _session.Delete(query, values, types);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "delete(" + query + "," + ArrayUtil.ToString(values) + ",types)");
			}
			return i;
		}

		public int Delete(String query, Object valueObject, IType type)
		{
			int i = - 1;
			try
			{
				i = _session.Delete(query, valueObject, type);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "delete(" + query + ",value,type)");
			}
			return i;
		}

		public object Save(Object object_Renamed)
		{
			object s = null;
			try
			{
				s = _session.Save(object_Renamed);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "save(" + object_Renamed + ")");
			}
			return s;
		}

		public void Save(Object object_Renamed, ISerializable id)
		{
			try
			{
				_session.Save(object_Renamed, id);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "save(" + object_Renamed + "," + id + ")");
			}
		}

		public void SaveOrUpdate(Object object_Renamed)
		{
			try
			{
				_session.SaveOrUpdate(object_Renamed);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "saveOrUpdate(" + object_Renamed + ")");
			}
		}

		public void Update(Object object_Renamed)
		{
			try
			{
				_session.Update(object_Renamed);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "update(" + object_Renamed + ")");
			}
		}

		public void Update(Object object_Renamed, ISerializable id)
		{
			try
			{
				_session.Update(object_Renamed, id);
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "update(" + object_Renamed + "," + id + ")");
			}
		}

		public void Flush()
		{
			try
			{
				_session.Flush();
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "flush()");
			}
		}

		public void Close()
		{
			try
			{
				if (_session != null)
				{
					_session.Close();
				}
			}
			catch (Exception t)
			{
				HandleDatabaseException(t, "close()");
			}
		}

		private void HandleDatabaseException(Exception t, String operation)
		{
			if (_session != null)
			{
				try
				{
					_session.Close();
					_session = null;
				}
				catch (Exception t2)
				{
					log.Error("couldn't close the database session properly. " + t2.Message, t);
				}
			}

			if (t is DbException)
			{
				throw (DbException) t;
			}
			else
			{
				log.Debug("operation: "+operation,t);
				throw new DbException(t.GetType().FullName + " while performing database operation '" + operation + "' : " + t.Message);
			}
		}
	}
}