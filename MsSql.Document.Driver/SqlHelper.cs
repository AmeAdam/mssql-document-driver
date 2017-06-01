using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.Document.Driver
{
    internal static class SqlHelper
    {
        private static readonly Dictionary<Type, SqlDbType> TypeMap;

        // Create and populate the dictionary in the static constructor
        static SqlHelper()
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

        // Non-generic argument-based method
        public static SqlDbType GetDbType(Type giveType)
        {
            // Allow nullable types to be handled
            giveType = Nullable.GetUnderlyingType(giveType) ?? giveType;

            if (TypeMap.ContainsKey(giveType))
            {
                return TypeMap[giveType];
            }

            throw new ArgumentException($"{giveType.FullName} is not a supported .NET class");
        }

        // Generic version
        public static SqlDbType GetDbType<T>()
        {
            return GetDbType(typeof(T));
        }
    }
}
