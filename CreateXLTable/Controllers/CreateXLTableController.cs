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
#if BASELINE
using MySql.Data.MySqlClient;
#else
using MySqlConnector;
#endif
using System.Data;
using System.Text.RegularExpressions;
using System.Text;
using System.Data.OleDb;
using  System.Data.Common;
using System.Data.SqlClient;

namespace CreateXLTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateXLTableController : ControllerBase
    {

[Route("api/dataload")]
[HttpGet]
public async Task<JsonResult> DataLoad( )
{
DataTable dt = new System.Data.DataTable();
            try
            {
                string filename = @"B:/Source.xlsx";
                string sWorkbook = string.Empty;
                string ExcelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0 xml;HDR=Yes;IMEX=1'";
                object json=string.Empty;
                OleDbConnection OleDbConn = new OleDbConnection(ExcelConnectionString);
                OleDbConn.Open();
                DataTable dtExcelSchema;
                    await Task.Delay(1);
                dtExcelSchema = OleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                for (int i = 0; i <= dtExcelSchema.Rows.Count - 1; i++)
                {
                    sWorkbook = string.Empty;
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
                    sWorkbook = sWorkbook.Remove(sWorkbook.LastIndexOf("$"));
                    sWorkbook = sWorkbook + "tbl";
                    ds.Tables[0].TableName = sWorkbook.ToString();
                   CreateTablefromDataTable(ds.Tables[0]);
                   // Console.Write(sWorkbook);
                    json = DataTableToJSONWithStringBuilder(ds.Tables[0]);
                   // AutoSqlBulkCopy(ds.Tables[0]);
                   await MySqlBlkCopyAsync(ds.Tables[0],sWorkbook );

                }
                OleDbConn.Close();
                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.ToString());
            }
           
        }
public async Task<bool> MySqlBlkCopyAsync(DataTable dataTable,string TableName)
    {
        try
        {
            bool result = true;
            using (MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection("server=localhost;port=3306;database=ExcelCompare;username=root;password=Honda#333;AllowLoadLocalInfile=true;"))
            {
                await connection.OpenAsync();
                
                var bulkCopy = new MySqlBulkCopy(connection);
                bulkCopy.DestinationTableName = TableName;
                await bulkCopy.WriteToServerAsync(dataTable);
                // the column mapping is required if you have a identity column in the table
               // bulkCopy.ColumnMappings.AddRange(GetMySqlColumnMapping(dataTable));
               // await bulkCopy.(dataTable);
                return result;
            }
        }
        catch (Exception)
        {
            throw;
        }
    }
public static void CreateTablefromDataTable(DataTable dataTable)
    {
        string conn_string = "server=localhost;port=3306;database=ExcelCompare;username=root;password=Honda#333;AllowLoadLocalInfile=True";
        MySqlConnector.MySqlConnection conn = new MySqlConnector.MySqlConnection(conn_string);
        string col = string.Empty;
         string createTableBuilder = string.Format("CREATE TABLE " +  dataTable.TableName + "");
        createTableBuilder = createTableBuilder + "(" ;
        foreach (DataColumn dc in dataTable.Columns)
                {
                    col =dc.ToString();
                    createTableBuilder = createTableBuilder +"`" + col.ToString() + "` Char(255),";
                }
                createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length-1);
                createTableBuilder = createTableBuilder +");";
            conn.Open();
            //var cmd = new MySql.Data.MySqlClient.MySqlCommand(createTableBuilder.ToString(), conn);
            var cmd = new MySqlConnector.MySqlCommand(createTableBuilder.ToString(), conn);
            cmd.ExecuteNonQuery();    
  
    }

[Route("api/readandreturnjson")]
[HttpPost]
//public object ReadAndReturnJsonAsync()
public async Task<JsonResult> CreateSQLTable([FromBody] Object obj)
{
    // object to return through the API (it'll be serialized by WebAPI)
       string allText = System.IO.File.ReadAllText(@"b:\data1.json");

        object jsonObject = JsonConvert.DeserializeObject(allText);
        await Task.Delay(1);
        var col = JsonConvert.DeserializeObject(allText);
        ConvertJsonToDatatable(col.ToString());
        //CreateTablefromJSON();    
        return new JsonResult(col);
}
protected void ConvertJsonToDatatable(string jsonString)
        {
            DataTable dt = new DataTable();
            //strip out bad characters
            string[] jsonParts = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            string strTableName = string.Empty;
            //hold column names
            List<string> dtColumns = new List<string>();

            //get columns
            foreach (string jp in jsonParts)
            {
                //only loop thru once to get column names
                string[] propData = Regex.Split(jp.Replace("{", "").Replace("}", ""), ",");
                
                foreach (string rowData in propData)
                {
                   // Console.Write(rowData);
                }
                foreach (string rowData in propData)
                {
                    try
                    {
                        int idx = rowData.IndexOf(":");
                        string n = rowData.Substring(0, idx - 1);
                        string v = rowData.Substring(idx + 1);
                       // Console.Write("idx :"+ idx +" , n: " +n+ ",v: "+v);
                        if(v.Contains("Tablename"))
                        {
                            strTableName=n.ToString();
                            strTableName=strTableName.Replace("\"", "");
                           // Console.Write("TableName : " +strTableName);
                        }
                        else
                        {
                        if (!dtColumns.Contains(n))
                        {
                            dtColumns.Add(n.Replace("\"", ""));//'
                        }
                        }
                        foreach(string c in dtColumns)
                        {
                            //Console.Write(c);
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Error Parsing Column Name : {0},{1}", rowData,ex));
                    }

                }
                break; // TODO: might not be correct. Was : Exit For
                //build dt
            }
            CreateTablefromJSON(strTableName.ToString(),dtColumns);
            
            }

        
public static void CreateTablefromJSON(string TableName,List<string> dtColumns)
    {
        string conn_string = "server=localhost;port=3306;database=ExcelCompare;username=root;password=Honda#333;";
        MySqlConnector.MySqlConnection conn = new MySqlConnector.MySqlConnection(conn_string);
         string createTableBuilder = string.Format("CREATE TABLE " + TableName + "");
        createTableBuilder = createTableBuilder + "(" ;
        foreach (var dc in dtColumns)
                {
                    createTableBuilder = createTableBuilder +"" + dc.ToString() + " NVARCHAR(5000),";
                }
                //Console.Write(createTableBuilder.LastIndexOf(","));
                createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length-1);
                createTableBuilder = createTableBuilder +");";
                Console.Write(createTableBuilder);
//         string createTableQuery = string.Format(@"CREATE TABLE `{0}` (
//    `sid` smallint(5) unsigned NOT NULL AUTO_INCREMENT,
//    `name` varchar(120) NOT NULL DEFAULT '',
//    `title` varchar(120) NOT NULL DEFAULT '',
//    `description` text NOT NULL,
//    `optionscode` text NOT NULL,
//    `value` text NOT NULL,
//    `disporder` smallint(5) unsigned NOT NULL DEFAULT '0',
//    `gid` smallint(5) unsigned NOT NULL DEFAULT '0',
//    `isdefault` tinyint(1) NOT NULL DEFAULT '0',
//    PRIMARY KEY (`sid`),
//    KEY `gid` (`gid`)) 
//    ENGINE = MyISAM AUTO_INCREMENT = 1 DEFAULT CHARSET = utf8;", "tbl1");
   conn.Open();
 
    //var cmd = new MySql.Data.MySqlClient.MySqlCommand(createTableBuilder.ToString(), conn);
    var cmd = new MySqlConnector.MySqlCommand(createTableBuilder.ToString(), conn);
    cmd.ExecuteNonQuery();
        
    }
    

            public string DataTableToJSONWithStringBuilder(DataTable table)   
    {  
    var JSONString = new StringBuilder();  
    if (table.Rows.Count > 0)   
    {  
        JSONString.Append("[");  
        for (int i = 0; i < table.Rows.Count; i++)   
        {  
            JSONString.Append("{");  
            for (int j = 0; j < table.Columns.Count; j++)   
            {  
                if (j < table.Columns.Count - 1)   
                {  
                    JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");  
                }   
                else if (j == table.Columns.Count - 1)   
                {  
                    JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");  
                }  
            }  
            if (i == table.Rows.Count - 1)   
            {  
                JSONString.Append("}");  
            }   
            else   
            {  
                JSONString.Append("},");  
            }  
        }  
        JSONString.Append("]");  
    }  
    return JSONString.ToString();  
}  

    }
    
}
