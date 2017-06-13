using System;
using System.Collections.Generic;
using System.Data;

namespace MsSql.Document.Driver
{
    internal static class SqlTypeHelper
    {
        private static readonly Dictionary<Type, SqlDbType> TypeMap;

        static SqlTypeHelper()
        {
            TypeMap = new Dictionary<Type, SqlDbType>
            {
                [typeof(string)] = SqlDbType.NVarChar,
                [typeof(char[])] = SqlDbType.NVarChar,
                [typeof(byte)] = SqlDbType.TinyInt,
                [typeof(short)] = SqlDbType.SmallInt,
                [typeof(int)] = SqlDbType.Int,
                [typeof(long)] = SqlDbType.BigInt,
                [typeof(byte[])] = SqlDbType.Image,
                [typeof(bool)] = SqlDbType.Bit,
                [typeof(DateTime)] = SqlDbType.DateTime2,
                [typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset,
                [typeof(decimal)] = SqlDbType.Money,
                [typeof(float)] = SqlDbType.Real,
                [typeof(double)] = SqlDbType.Float,
                [typeof(TimeSpan)] = SqlDbType.Time
            };
        }

        public static SqlDbType GetDbType(Type giveType)
        {
            giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;
            SqlDbType res;
            if (TypeMap.TryGetValue(giveType, out res))
                return res;
            throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
        }

        public static SqlDbType GetDbType<T>()
        {
            return GetDbType(typeof(T));
        }
    }
}
