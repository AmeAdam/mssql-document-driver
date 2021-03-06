﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace MsSql.Document.Driver
{
    public class DocumentCollection<T> where T : IIdentifiable
    {
        private readonly DocumentDatabase database;
        private readonly string collectionName;
        private Dictionary<string, string> indexes = new Dictionary<string, string> {{"Id", "Id"}};

        internal DocumentCollection(DocumentDatabase database, string collectionName)
        {
            this.database = database;
            this.collectionName = collectionName;
            database.ExecuteCommand(SqlSchemaHelper.CreateCollectionIfNotExist(collectionName));
        }

        public IEnumerable<T> FindAll()
        {
            var cmd = new SqlCommand { CommandText = $"select id,json from [{collectionName}]" };
            foreach (var row in database.ExecuteReader(cmd))
            {
                yield return JsonConvert.DeserializeObject<T>(row.json);
            }
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            SqlLinqParser parser = new SqlLinqParser(collectionName, predicate, indexes);
            var cmd = parser.Parse();

            //var body = (BinaryExpression)predicate.Body;
            //var field = (MemberExpression) body.Left;
            //var value = (ConstantExpression) body.Right;

            //var cmd = new SqlCommand { CommandText = $"select id,json from [{collectionName}] where JSON_VALUE(json, '$.{field.Member.Name}')=@param" };
            //cmd.Parameters.Add("@param", SqlTypeHelper.GetDbType(value.Type)).Value = value.Value;
            foreach (var row in database.ExecuteReader(cmd))
            {
                yield return JsonConvert.DeserializeObject<T>(row.json);
            }
        }

        public void InsertOne(T document)
        {
            var cmd = new SqlCommand {CommandText = "insert into users (id,json) values (@id, @json)"};
            cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = document.Id;
            var json = JsonConvert.SerializeObject(document, Formatting.Indented);
            cmd.Parameters.Add("@json", SqlDbType.NText).Value = json;
            database.ExecuteCommand(cmd);
        }

        public void Update(T document)
        {
            var cmd = new SqlCommand { CommandText = "update users set json=@json where id=@id" };
            cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = document.Id;
            var json = JsonConvert.SerializeObject(document, Formatting.Indented);
            cmd.Parameters.Add("@json", SqlDbType.NText).Value = json;
            database.ExecuteCommand(cmd);
        }

        public void Delete(string id)
        {
            var cmd = new SqlCommand { CommandText = "delete users where id=@id" };
            cmd.Parameters.Add("@id", SqlDbType.NVarChar).Value = id;
            database.ExecuteCommand(cmd);
        }

        public void Drop()
        {
            database.ExecuteCommand(SqlSchemaHelper.DropCollectionIfExist(collectionName));
        }
    }
}