using System;
using System.Data.SqlServerCe;

namespace AdU_Pharmacy_Inventory_System
{
    class DBSQLServerUtils
    {
        public static SqlCeConnection
        GetDBConnection(string datasource)
        {
            //Data Source=ADMINRG-S0R6T5U\SQLEXPRESS;Initial Catalog=studentDB;Integrated Security=True
            string connString = @"Data Source=" + datasource;
            SqlCeConnection conn = new SqlCeConnection(connString);

            return conn;
        }
    }
}
