﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerater
{
    class CsharpEnumGenerare
    {
        private string mNameSpace;
        private string connstr;
        private string mDbType;
        private string tab = "\t ";
        private string comma = ",";
        private List<MyHelper.DbSchema> mDbSchemas;
        private List<MyHelper.MysqlTabeSchema> mMysqlTabeSchemas;
        private List<MyHelper.SqliteTableSchema> mSqliteTableSchemas;
        public CsharpEnumGenerare(string nsp, string conn = null,string dbtype = null)
        {
            mNameSpace = nsp;
            connstr = conn;
            if (dbtype == null)
            {
                mDbType = DbType.mysql.ToString();
            }
            else {
                mDbType = dbtype;
            }
        }

        public string dbEnumGenerater()
        {
            if (mDbSchemas.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using System.Linq;");
                sb.AppendLine("using System.Text;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine();
                sb.AppendLine(string.Format("namespace {0}", mNameSpace));
                sb.AppendLine("{");
                sb.AppendLine();
                sb.AppendLine(getcomment("数据库中的所有表名称"));
                sb.AppendLine(tab + " public enum DataTabeName{");
                for (int i = 0; i < mDbSchemas.Count; i++)
                {
                    MyHelper.DbSchema sc = mDbSchemas[i];
                    sb.AppendLine(tab + tab + sc.TableName + comma);
                }
                sb.AppendLine(tab + "}");

                for (int i = 0; i < mDbSchemas.Count; i++)
                {
                    MyHelper.DbSchema sc = mDbSchemas[i];
                    sb.AppendLine(tableEnumGenerater(sc.TableName));
                }

                sb.AppendLine("}");
                return sb.ToString();
            }
            return null;
        }

        public string tableEnumGenerater(string tableName, string commend = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (!string.IsNullOrEmpty(commend))
            {
                sb.AppendLine(getcomment(commend));
            }
            string name = MyHelper.StringHelper.upperCaseFirstLetter(MyHelper.StringHelper.DBNamingToCamelCase(tableName));
            sb.AppendLine(tab + $"public enum {name}" + "{");
            if (mDbType == DbType.mysql.ToString()) {
                mMysqlTabeSchemas = new MyHelper.MySqlHelper(connstr).getTableSchema(tableName);
                for (int i = 0; i < mMysqlTabeSchemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + mMysqlTabeSchemas[i].Field + comma);
                }
            } else {
                mSqliteTableSchemas = new MyHelper.SQLiteHelper(connstr).getTableSchema(tableName);
                for (int i = 0; i < mSqliteTableSchemas.Count; i++)
                {
                    sb.AppendLine(tab + tab + mSqliteTableSchemas[i].name + comma);
                }
            }           
            sb.AppendLine(tab + "}");
            return sb.ToString();
        }

        private void getDbSchema()
        {
            if (mDbType == DbType.mysql.ToString())
            {
                mDbSchemas = new MyHelper.MySqlHelper(connstr).getAllTableSchema();
            }
            else {
                mDbSchemas = new MyHelper.SQLiteHelper(connstr).getAllTableSchema();

            }
        }

        private void getTableSchema()
        {
            if (mDbType == DbType.mysql.ToString())
            {
                mDbSchemas = new MyHelper.MySqlHelper(connstr).getAllTableSchema();
            }
            else
            {
                mDbSchemas = new MyHelper.SQLiteHelper(connstr).getAllTableSchema();
            }
        }
        public string getcomment(string comment)
        {           
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(tab+"/// <summary>");
            sb.AppendLine(tab + "///" + comment);
            sb.AppendLine(tab + "/// </summary>");
            return sb.ToString();
        }
    }
}