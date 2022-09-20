using SqlSugar;

namespace ExcelTest.Env
{
    public class SqlSugerConfig
    {
        private static string dbConnectionStr = SystemConfig.GetSettingset("ConnectionStrings:ProcDB");

        public static SqlSugarClient GetConnectOption()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = dbConnectionStr,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true, // 自动释放链接
                // InitKeyType = InitKeyType.Attribute
            });
        }
    }
}