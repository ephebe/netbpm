using NHibernate;
using NHibernate.Type;

namespace NetBpm.Util.DB
{
	/// <summary> This class wraps all the hibernate types so that 
	/// users of this package don't have to include the net.sf.hibernate packages.
	/// </summary>
	public class DbType
	{
		//@portme
//		public static readonly Type BIG_DECIMAL = Hibernate.BIG_DECIMAL;
		public static readonly IType BINARY = NHibernateUtil.Binary;
		public static readonly IType BLOB = NHibernateUtil.BinaryBlob;
		public static readonly IType BOOLEAN = NHibernateUtil.Boolean;
		public static readonly IType BYTE = NHibernateUtil.Byte;
//		public static readonly IType CALENDAR = Hibernate.CALENDAR;
//		public static readonly IType CALENDAR_DATE = Hibernate.CALENDAR_DATE;
//		public static readonly IType CHARACTER = Hibernate.CHARACTER;
		public static readonly IType CLASS = NHibernateUtil.Class;
//		public static readonly IType CLOB = Hibernate.CLOB;
//		public static readonly IType CURRENCY = Hibernate.CURRENCY;
		public static readonly IType DATE = NHibernateUtil.Date;
		public static readonly IType DOUBLE = NHibernateUtil.Double;
//		public static readonly IType FLOAT = Hibernate.FLOAT;
		public static readonly IType INTEGER = NHibernateUtil.Int32;
//		public static readonly IType LOCALE = Hibernate.LOCALE;
		public static readonly IType LONG = NHibernateUtil.Int64;
		public static readonly IType OBJECT = NHibernateUtil.Object;
		public static readonly IType SERIALIZABLE = NHibernateUtil.Serializable;
		public static readonly IType SHORT = NHibernateUtil.Int16;
		public static readonly IType STRING = NHibernateUtil.String;
		public static readonly IType TIME = NHibernateUtil.Time;
		public static readonly IType TIMESTAMP = NHibernateUtil.Timestamp;
//		public static readonly IType TIMEZONE = Hibernate.TIMEZONE;
		public static readonly IType TRUE_FALSE = NHibernateUtil.TrueFalse;
		public static readonly IType YES_NO = NHibernateUtil.YesNo;
	}
}