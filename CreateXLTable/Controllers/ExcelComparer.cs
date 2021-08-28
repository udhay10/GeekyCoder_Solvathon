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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using ExcelComparer;

namespace ExcelComparer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExcelComparerController : ControllerBase
    {
        static IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
        public static string connectionString= conf["ConnectionString:Value"].ToString();

[Route("api/dataload")]
[HttpPost]
public async Task<JsonResult> DataLoad([FromBody] ExcelComparer1.GetXLObjClass objClass )
{
        ExcelComparer1.GetXLObjClass objClass1= new ExcelComparer1.GetXLObjClass();
            try
            {   object json = string.Empty;
                objClass.SourceFile =@"C:\Users\UDHAYASANKAR.R\Documents\GitHub\Source.xlsx";
                objClass.SourceSheetName = "Source_SQL";
                objClass.DestFile= @"C:\Users\UDHAYASANKAR.R\Documents\GitHub\Destination.xlsx";
                objClass.DestSheetName ="Destnation_SQL";
                List<string> uniqueKey = new List<string>();
                List<string> boolFields= new List<string>();
                List<string> srcColList = new List<string>();
                List<string> dstColList= new List<string>();
                uniqueKey.Add("PAYMENT_ID");
                uniqueKey.Add("ContractType");
                uniqueKey.Add("PaymentType");
                uniqueKey.Add("Is_Payment_Done?");
                uniqueKey.Add("Payment_due_date");
                boolFields.Add("Is_Payment_Done?");
                srcColList.Add("PAYMENT_ID");	
srcColList.Add("CONTRACT_NO")	;
srcColList.Add("ContractType");
srcColList.Add("PaymentType");
srcColList.Add("CURRENCY_CODE");
srcColList.Add("FX_Rate");
srcColList.Add("Cost (USD)");
srcColList.Add("Cost (Local)");
srcColList.Add("Payment_due_date");
srcColList.Add("Approval_date");
srcColList.Add("Approver_Name");
srcColList.Add("Is_Payment_Done?");
srcColList.Add("Cash_localAmt");
srcColList.Add("Cash_deducted_amt");
srcColList.Add("Cash_USDAmt");
srcColList.Add("Balance_Payment_Local")	;
srcColList.Add("Balance_Payment_USD");
srcColList.Add("Payment_Received_Date");	
srcColList.Add("Issue_Date");
srcColList.Add("Payment_Status");
srcColList.Add("Last_UpdatedDate");
srcColList.Add("Payment_Comments");
srcColList.Add("Products");
srcColList.Add("~");
dstColList.Add("PaymentID");
dstColList.Add("ContractNo");
dstColList.Add("ContractType");
dstColList.Add("PaymentType");
dstColList.Add("CURRENCY_CODE");
dstColList.Add("FX_Rate");
dstColList.Add("Cost in USD");
dstColList.Add("Cost in Local");
dstColList.Add("Payment_due_date");
dstColList.Add("Approval_date");
dstColList.Add("Approver_Name");
dstColList.Add("IS_PAYMENT_COMPLETE?");
dstColList.Add("Cash_localAmt");
dstColList.Add("Cash_deducted_amt");
dstColList.Add("Cash_USDAmt");
dstColList.Add("Balance_Payment_Local");
dstColList.Add("Balance_Payment_USD");
dstColList.Add("Payment_Received_Date");
dstColList.Add("Issue_Date");
dstColList.Add("Payment_Status");
dstColList.Add("Last_UpdatedDate");
dstColList.Add("Payment_Comments");
dstColList.Add("Products");
dstColList.Add("ID");
                await Task.Delay(1);
                if(objClass.SourceFile != null)
                {
                    string TableName = string.Empty;
                    TableName="source";
                    Console.Write("src file\n");
                  //  await CreateTablefromFile(objClass.SourceFile,objClass.SourceSheetName+"$", TableName);/** To create Source Table in MYSQL and insert filedata into the tabel**/
                }
                if(objClass.DestFile != null)
                {
                    string TableName = string.Empty;
                    TableName="destination";
                    Console.Write("destination file\n");
                   // await CreateTablefromFile(objClass.DestFile,objClass.DestSheetName+"$",TableName);/** To create Destination Table in MYSQL and insert filedata into the tabel**/
                }
               // Console.Write(conn_string);
                //InsertMapppedColumns(srcColList,dstColList,uniqueKey,boolFields);
               // UniqueKeyMissingRecords(srcColList,dstColList,uniqueKey,boolFields);
                ColumnMismatch(srcColList,dstColList);//Implementation pending
                // RecordToRecordCount(); //Implementation pending

                return new JsonResult(json);
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.ToString());
            }
           
        }
public async Task<bool> CreateTablefromFile(string filename,string sheetname, string Tablename)
{
    try
            {
                
                //string filename = @"B:/source.xlsx";
                string sWorkbook = string.Empty;
                /** Read Excel**/
                string ExcelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filename + ";Extended Properties='Excel 12.0 xml;HDR=No;IMEX=1'";
                object json=string.Empty;
                OleDbConnection OleDbConn = new OleDbConnection(ExcelConnectionString);
                OleDbConn.Open();
                DataTable dtExcelSchema;
                 //   await Task.Delay(1);
                dtExcelSchema = OleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                for (int i = 0; i <= dtExcelSchema.Rows.Count - 1; i++)
                {

                    sWorkbook = string.Empty;
                    sWorkbook = dtExcelSchema.Rows[i]["TABLE_NAME"].ToString();
                   // Console.Write("\n sWorkBook Name : {0}, Sheetname : {1}\n",sWorkbook,sheetname);
                    if(sWorkbook==sheetname)
                    {
                    //sWorkbook = sWorkbook.Replace('$', ' ');
                    OleDbCommand OleDbCmd = new OleDbCommand();
                    OleDbCmd.Connection = OleDbConn;
                    OleDbCmd.CommandText = "SELECT * FROM [" + sWorkbook + "]";
                    DataSet ds = new DataSet();
                    OleDbDataAdapter sda = new OleDbDataAdapter();
                    sda.SelectCommand = OleDbCmd;
                    sda.Fill(ds);
                    sWorkbook = sWorkbook.Remove(sWorkbook.LastIndexOf("$"));
                    sWorkbook = sWorkbook + "tbl";
                    ds.Tables[0].TableName = Tablename.ToString();
                    DataRow rowDel = ds.Tables[0].Rows[0];
                    CreateTablefromDataTable(ds.Tables[0]); /** Create Table in MYSQL**/
                    // json = DataTableToJSONWithStringBuilder(ds.Tables[0]);
                    ds.Tables[0].Rows.Remove(rowDel);
                    await MySqlBlkCopyAsync(ds.Tables[0],ds.Tables[0].TableName ); /** Insert data into MYSQL table **/
                    }

                }
                OleDbConn.Close();
              return true;
            }
            catch (Exception ex)
            {
                 throw new Exception(ex.ToString());
            }
}
public static void CreateTablefromDataTable(DataTable dataTable)
    {
        //string conn_string = "server=localhost;port=3306;database=excelcomparer;username=root;password=Root@123456;AllowLoadLocalInfile=True";
        string conn_string =connectionString;
        MySqlConnector.MySqlConnection conn = new MySqlConnector.MySqlConnection(conn_string);
        string col = string.Empty;
         string createTableBuilder = string.Format("DROP TABLE IF EXISTS " +  dataTable.TableName + ";");
         createTableBuilder = createTableBuilder + "CREATE TABLE " +  dataTable.TableName + "";
        createTableBuilder = createTableBuilder + "(" ;
        
                for (int i=0;i<1;i++)
                {
                    for(int j=0;j<dataTable.Columns.Count;j++)
                    {
                    col =dataTable.Rows[i][j].ToString();
                    createTableBuilder = createTableBuilder +"`" + col.ToString() + "` varchar(255),";
                    }
                }
                createTableBuilder = createTableBuilder.Remove(createTableBuilder.Length-1);
                createTableBuilder = createTableBuilder +");";
            conn.Open();
            //var cmd = new MySql.Data.MySqlClient.MySqlCommand(createTableBuilder.ToString(), conn);
            var cmd = new MySqlConnector.MySqlCommand(createTableBuilder.ToString(), conn);
            cmd.ExecuteNonQuery();      
    }

public async Task<bool> MySqlBlkCopyAsync(DataTable dataTable,string TableName)
    {
        try
        {
            bool result = true;
            using (MySqlConnector.MySqlConnection connection = new MySqlConnector.MySqlConnection(connectionString))
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
    public  static void UniqueKeyMissingRecords(List<string> srcColList,List<string> dstColList,List<string> UniquekeyList, List<string> boolFields )
    {
        //string conn_string = "server=localhost;port=3306;database=excelcomparer;username=root;password=Root@123456;AllowLoadLocalInfile=True";
        string conn_string = connectionString;
        MySqlConnector.MySqlConnection conn = new MySqlConnector.MySqlConnection(conn_string);
        string col = string.Empty;
        string callMissingRecords = string.Format("CALL IdentifyMissingRecords ('" );
        bool isboolField = false;
        foreach(var Uniquekey in UniquekeyList)
        {
            foreach(var flag in boolFields)
            {

            if(Uniquekey.Equals(flag))
            {
            callMissingRecords = callMissingRecords + "(CASE WHEN source.`"   + Uniquekey + "`=\"0\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"1\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"NO\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"YES\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"False\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"True\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"F\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"T\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"N\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN source.`"   + Uniquekey + "`=\"Y\" then \"YES\"";
            callMissingRecords = callMissingRecords + " Else \"\" END),";

           // boolFields.Remove(flag.ToString());
            isboolField =true;
            Console.Write("Unique Key : Is Bool : Source :"+Uniquekey+"\n");
            break;
            
            }
            else
            {
                isboolField =false;
            }
            }
             if(isboolField== false)
             {
             callMissingRecords = callMissingRecords + "IFNULL(source.`"   + Uniquekey + "`,\"\"),";
             Console.Write("Unique Key : Is not Bool : Source :"+Uniquekey+ "\n");
             }
        }
        callMissingRecords = callMissingRecords.Remove(callMissingRecords.Length-1);
        callMissingRecords = callMissingRecords + "','";
        isboolField= false;
        foreach(var Uniquekey in UniquekeyList)
        {
            foreach(var srcCol in srcColList)
            {
                foreach(var flag in boolFields)
            {
            if(Uniquekey.Equals(flag))
                if(Uniquekey.Equals(srcCol))
                {
                    int idx = srcColList.IndexOf(srcCol);
            callMissingRecords = callMissingRecords + "(CASE WHEN destination.`"   + dstColList[idx] + "`=\"0\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"1\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"NO\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"YES\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"False\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"True\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"F\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"T\" then \"YES\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"N\" then \"NO\"";
            callMissingRecords = callMissingRecords + " WHEN destination.`"   + dstColList[idx] + "`=\"Y\" then \"YES\"";
            callMissingRecords = callMissingRecords + " Else \"\" END),";
            isboolField =true;
            break;
                }
                else
            {
                isboolField=false;
            }
            }
            if(Uniquekey.Equals(srcCol))
                {
                    int idx = srcColList.IndexOf(srcCol);
            if(isboolField== false)
             {
             callMissingRecords = callMissingRecords + "IFNULL(destination.`"   + dstColList[idx] + "`,\"\"),";
             }
                }
            }
        }
        callMissingRecords = callMissingRecords.Remove(callMissingRecords.Length-1);
        callMissingRecords = callMissingRecords + "');";
        conn.Open();
        //var cmd = new MySqlConnector.MySqlCommand(callMissingRecords.ToString(), conn);
       // cmd.ExecuteNonQuery(); 
        DataSet ds= new DataSet();
        var cmd = new MySqlConnector.MySqlCommand(callMissingRecords.ToString(), conn);
        MySqlDataAdapter da = new MySqlDataAdapter();
       da.SelectCommand = cmd;
       da.Fill(ds);
        DataTable dt = new DataTable();
        dt = ds.Tables[0];
        foreach(DataRow dataRow in dt.Rows)
        {
             foreach(var item in dataRow.ItemArray)
             {
             Console.WriteLine(item);
             }
        }
       //json = DataTableToJSONWithStringBuilder(dt);
      // cmd.ExecuteNonQuery(); 
       Console.Write(callMissingRecords);
    }
    public static void InsertMapppedColumns(List<String> srcMappedCol,List<string> destMappedColumns,List<string> UniqueKeys,List<string> flagFields)
    {
   // string conn_string = "server=localhost;port=3306;database=excelcomparer;username=root;password=Root@123456;AllowLoadLocalInfile=True";
        string conn_string = connectionString;
        MySqlConnector.MySqlConnection conn = new MySqlConnector.MySqlConnection(conn_string);
        string col = string.Empty;
        bool isUniquekey = false;
        bool isboolField = false;
         string insertColMapping = string.Format("Truncate TABLE ColumnMapping;");
         insertColMapping = insertColMapping + "INSERT INTO ColumnMapping(Source_Column, Destination_Column, Is_Unique,is_Flag)";
        insertColMapping = insertColMapping + " VALUES " ;
        //Console.Write(destMappedColumns.Count + " : " + srcMappedCol.Count);
        for(int i=0;i<destMappedColumns.Count;i++)
        {
            insertColMapping =insertColMapping + "(case when \""+ srcMappedCol[i] + "\"=\"~\" then Null else \""+srcMappedCol[i] + "\" end,"  ;
            insertColMapping =insertColMapping + "case when \""+destMappedColumns[i] + "\"=\"~\" then Null else \"" +destMappedColumns[i] + "\" end,";
            
            foreach(var uniqueFiels in UniqueKeys){
            if(srcMappedCol[i].Equals(uniqueFiels))
            {
                insertColMapping = insertColMapping + "1,";
                isUniquekey=true;
                break;
            }
            else
            {
                isUniquekey=false;
            }
            }
            if(isUniquekey==false)
            {
               insertColMapping = insertColMapping + "Null,"; 
            }
            foreach(var flag in flagFields){
            if(srcMappedCol[i].Equals(flag))
            {
                insertColMapping = insertColMapping + "1),";
                isboolField=true;
                break;
            }
            else
            {
                isboolField=false;
            }
            }
            if(isboolField==false)
            {
               insertColMapping = insertColMapping + "Null),"; 
            }
            }
            
            
        insertColMapping = insertColMapping.Remove(insertColMapping.Length-1);
        insertColMapping = insertColMapping + ";";
        
        conn.Open();
        var cmd = new MySqlConnector.MySqlCommand(insertColMapping.ToString(), conn);
        cmd.ExecuteNonQuery(); 

    }
    public static void ColumnMismatch(List<string> sourceColmn, List<string> destinationColmn)
    {
            StringBuilder result = new StringBuilder();
            DataTable dt = new DataTable();
            dt.Columns.Add("SourceColumnName", typeof(string));
            dt.Columns.Add("Match", typeof(bool));
            dt.Columns.Add("DestinationColumnName", typeof(string));
            dt.Columns.Add("Comments", typeof(string));
            
            if (sourceColmn.Count == destinationColmn.Count)
            {
                result.Append("Count match no extra column.");
            }
            else
            {
                result.Append("Count mismatch.");
                result.Append(Environment.NewLine);

                int Diff = sourceColmn.Count - destinationColmn.Count;

                if (Diff > 0)
                {
                    result.Append("Source has " + Diff.ToString() + " more columns then Destination");
                    result.Append(Environment.NewLine);

                    for (int i = 1; i <= Diff; i++)
                    {
                        destinationColmn.Add("~");
                    }
                }
                else
                {
                    result.Append("Destination has " + Diff.ToString() + " more columns then Source");

                    for (int i = 1; i <= System.Math.Abs(Diff); i++)
                    {
                        sourceColmn.Add("~");
                    }
                }



                for (int i = 0; i < sourceColmn.Count; i++)
                {
                    if (sourceColmn[i].Equals(destinationColmn[i]))
                    {
                        dt.Rows.Add(sourceColmn[i], true, destinationColmn[i], "");
                        Console.Write(dt.Rows);
                    }
                    else
                    {
                        string comments = string.Empty;
                        if (sourceColmn[i] == "~" || destinationColmn[i] == "~")
                        {
                            comments = (sourceColmn[i] == "~") ? "Additional column in Source" : "Additional column in Destination";
                        }
                        else
                        {
                            comments = "As per mapping the naming is different but it’s the same column";
                        }

                        dt.Rows.Add(sourceColmn[i], false, destinationColmn[i], comments);
                        
                    }
                }
            }
        Console.Write(dt.Rows.Count);
    }
 private List<MySqlBulkCopyColumnMapping> GetMySqlColumnMapping(DataTable dataTable)
    {
        List<MySqlBulkCopyColumnMapping> colMappings = new List<MySqlBulkCopyColumnMapping>();
        int i = 0;
        foreach (DataColumn col in dataTable.Columns)
        {
            colMappings.Add(new MySqlBulkCopyColumnMapping(i, col.ColumnName));
            i++;
        }
        return colMappings;
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
        //string conn_string = "server=localhost;port=3306;database=ExcelComparer;username=root;password=Root@123456;";
        string conn_string = connectionString;
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
