# mssql-document-driver
Access mssql 2016 database as document database similar like MongoDB.

var client = new DatabaseClient(".\\SqlExpress");
var database = client.GetDatabase("TestDatabase"); //creates database if not exist
var collection = database.GetCollection<DocumentClass>("collection1"); //create table if not exist
collection.InsertOne(new DocumentClass {Id = "123", Name="Abc", Array=new int[]{1,2,3}}); //insert new document/record to database as JSON
var doc = collection.Find(doc => doc.Name == "Abc"); //Linq query expressions