using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MsSql.Document.Driver
{
    internal class SqlLinqParser : ExpressionVisitor
    {
        private readonly Expression expression;
        private readonly Dictionary<string, string> indexes;
        private int paramIndex;
        private readonly StringBuilder sb = new StringBuilder();
        private readonly SqlCommand cmd;

        public SqlLinqParser(string collectionName, Expression expression, Dictionary<string, string> indexes)
        {
            this.expression = expression;
            this.indexes = indexes;
            cmd = new SqlCommand();
            sb.Append($"SELECT id, json FROM [{ collectionName}] WHERE ");
        }

        public SqlCommand Parse()
        {
            Visit(expression);
            cmd.CommandText = sb.ToString();
            return cmd;
        }


        private void ProcessBinareyPart(Expression node)
        {
            if (node.NodeType != ExpressionType.MemberAccess && node.NodeType != ExpressionType.Constant)
            {
                sb.Append("(");
                Visit(node);
                sb.Append(")");
            }
            else
                Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            ProcessBinareyPart(node.Left);
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    sb.Append(" = ");
                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                case ExpressionType.NotEqual:
                    sb.Append(" <> ");
                    break;
                default: throw new NotSupportedException($"The Expression type {node.NodeType} is not supported.");
            }
            ProcessBinareyPart(node.Right);
            return node;
        }

        protected IEnumerable<string> GetMemberPath(MemberExpression node)
        {
            var current = node;
            while (current != null && current.NodeType != ExpressionType.Parameter)
            {
                yield return current.Member.Name;
                current = current.Expression as MemberExpression;
            }
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var memberPath = string.Join(".", GetMemberPath(node).Reverse());
            string ix;
            if (indexes.TryGetValue(memberPath, out ix))
            {
                sb.Append(ix);
            }
            else
                sb.Append($"JSON_VALUE(json, '$.{memberPath}')");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var paramName = "@param_" + paramIndex++;
            cmd.Parameters.Add(new SqlParameter(paramName, SqlTypeHelper.GetDbType(node.Type)) { Value = node.Value });
            sb.Append(paramName);
            return node;
        }
    }
}
