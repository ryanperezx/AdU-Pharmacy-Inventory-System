using System;
using System.Data.SqlServerCe;


namespace AdU_Pharmacy_Inventory_System
{
    class DBUtils
    {
        public static SqlCeConnection GetDBConnection()
        {
            string user = Environment.UserName;
            string datasource = @"C:\Users\" + user + @"\source\repos\ryanperezx\AdU-Pharmacy-Inventory-System\AdU Pharmacy Inventory System\bin\Debug\inventSystemDB.sdf";
            //string datasource = @"C:\Users\" + user + @"\Desktop\aduph-is\inventSystemDB.sdf";
            return DBSQLServerUtils.GetDBConnection(datasource);
        }
    }
}
