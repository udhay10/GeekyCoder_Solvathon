using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.OleDb;
using System.Data.SqlClient;

namespace CreateXLTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataLoadController : ControllerBase
    {     
[Route("api/dataload")]
[HttpGet]
public void DataLoad()
{
DataTable dt = new System.Data.DataTable();
            try
            {
                string filename = @"C:\Users\sani singh\Documents\Excel03.xls";
                string sWorkbook = string.Empty;
                string ExcelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0 xml;HDR=Yes;IMEX=1'";

                OleDbConnection OleDbConn = new OleDbConnection(ExcelConnectionString);
                OleDbConn.Open();
                DataTable dtExcelSchema;

                dtExcelSchema = OleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                for (int i = 0; i <= dtExcelSchema.Rows.Count - 1; i++)
                {
                    sWorkbook = dtExcelSchema.Rows[i]["TABLE_NAME"].ToString();
                    //sWorkbook = sWorkbook.Replace('$', ' ');
                    OleDbCommand OleDbCmd = new OleDbCommand();
                    OleDbCmd.Connection = OleDbConn;
                    OleDbCmd.CommandText = "SELECT * FROM [" + sWorkbook + "]";

                    DataSet ds = new DataSet();
                    OleDbDataAdapter sda = new OleDbDataAdapter();
                    sda.SelectCommand = OleDbCmd;
                    sda.Fill(ds);
                    dt = ds.Tables[0];
                    ds.Tables[0].TableName = sWorkbook.ToString();
                    
                   // AutoSqlBulkCopy(ds.Tables[0]);

                }
                OleDbConn.Close();
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.ToString());
            }
           
        }
}
}
