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

namespace CreateXLTable.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateXLTableController : ControllerBase
    {
       
       
[Route("api/readandreturnjson")]
[HttpGet]
//public object ReadAndReturnJsonAsync()
public async Task<JsonResult> Get()
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
        MySqlConnection conn = new MySqlConnection(conn_string);
         string createTableBuilder = string.Format("CREATE TABLE " + TableName + "");
        createTableBuilder = createTableBuilder + "(" ;
        foreach (var dc in dtColumns)
                {
                    createTableBuilder = createTableBuilder +"" + dc.ToString() + " VARCHAR(150),";
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
 
    var cmd = new MySql.Data.MySqlClient.MySqlCommand(createTableBuilder.ToString(), conn);
    cmd.ExecuteNonQuery();
        
    }
    }
}
