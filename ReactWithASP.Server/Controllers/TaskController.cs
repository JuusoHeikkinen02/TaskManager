using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ReactWithASP.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : Controller
    {
        private IConfiguration _configuration;

        public TaskController(IConfiguration configuration)
        {
           _configuration = configuration;
        
        }

        [HttpGet]
        [Route("GetTasks")]

        //Funktio jolla saadaan tietokannasta tehtävän tiedot
        public JsonResult GetTasks()
        {
            string query = "select * from dbo.Task";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            SqlDataReader myReader;
            using(SqlConnection myCon = new SqlConnection(sqlDatasource)) 
            {
              myCon.Open();
                using (SqlCommand MyCommand = new SqlCommand(query, myCon))
                {
                    myReader = MyCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetTags")]

        //Funktio jolla saadaan tagin tiedot
        public JsonResult GetTags() 
        {
            string query = "select * from dbo.Tag";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand MyCommand = new SqlCommand(query, myCon))
                {
                    myReader = MyCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);


        }

        [HttpGet]
        [Route("GetStatus")]

        //Funktio jolla saadaan status
        public JsonResult GetStatus()
        {
            string query = "select * from dbo.Status";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand MyCommand = new SqlCommand(query, myCon))
                {
                    myReader = MyCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);


        }

        [HttpGet]
        [Route("GetActivity")]
        //Funktio jolla saadaan aktiviteetin tiedot 
        public JsonResult GetActivity()
        {
            string query = "select * from dbo.Activity";
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand MyCommand = new SqlCommand(query, myCon))
                {
                    myReader = MyCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);
        }

        [HttpPut]
        [Route("AddTask")]

        public JsonResult AddTask(string Name, string Description, DateTime StartDate, DateTime EndDate, string TagName, string TagTheme, string StatusName, string StatusTheme)
        {
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                int TagId = InsertTag(TagName, TagTheme);

                int statusId = InsertStatus(StatusName, StatusTheme);

                string insertQuery = "INSERT INTO Task (Name, Description, StartDate, EndDate, TagId, StatusId) " +
                                "VALUES (@Name, @Description, @StartDate, @EndDate, @TagId, @StatusId)";


                using (SqlCommand MyCommand = new SqlCommand(insertQuery, myCon))
                {
                    MyCommand.Parameters.AddWithValue("@Name", Name);
                    MyCommand.Parameters.AddWithValue("@Description", Description);
                    MyCommand.Parameters.AddWithValue("@StartDate", StartDate);
                    MyCommand.Parameters.AddWithValue("@EndDate", EndDate);
                    MyCommand.Parameters.AddWithValue("@TagId", TagId);
                    MyCommand.Parameters.AddWithValue("@StatusId", statusId);
                    myReader = MyCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Lisäys onnistui");


        }

        //täällä saadaan Tägi luotua samalla kun tehdään uusi Task tai Activity
        private int InsertTag(string TagName, string TagTheme)
        {
          
            string insertTagQuery = "IF NOT EXISTS (SELECT 1 FROM Tag WHERE Name = @TagName) " +
                                    "BEGIN " +
                                    "    INSERT INTO Tag (Name, Theme) VALUES (@TagName, @TagTheme) " +
                                    "END " +
                                    "SELECT Id FROM Tag WHERE Name = @TagName";
            
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(insertTagQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@TagName", TagName);
                    myCommand.Parameters.AddWithValue("@TagTheme", TagTheme);
                    myCommand.ExecuteNonQuery();
                    int TagId = (int)myCommand.ExecuteScalar();
                    myCon.Close();
                    return TagId;
                }
            }
            
        }

        //täällä saadaan status luotua samalla kun tehdään uusi Task tai Activity
        private int InsertStatus(string StatusName, string StatusTheme)
        {
            string insertStatusQuery = "IF NOT EXISTS (SELECT 1 FROM Status WHERE Name = @StatusName) " +
                                       "BEGIN " +
                                       "    INSERT INTO Status (Name, Theme) VALUES (@StatusName, @StatusTheme) " +
                                       "END " +
                                       "SELECT Id FROM Status WHERE Name = @StatusName";
          
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(insertStatusQuery, myCon))
                {
                    myCommand.Parameters.AddWithValue("@StatusName", StatusName);
                    myCommand.Parameters.AddWithValue("@StatusTheme", StatusTheme);

                    myCommand.ExecuteNonQuery();
                    int statusId = (int)myCommand.ExecuteScalar();
                    myCon.Close();
                    return statusId;
                }
            }
            
        }

        //Funktio jolla voidaan poistaa tehtävä ja sen pitäisi samalla poistaa status ja tagi 
        [HttpDelete]
        [Route("DeleteTask")]

        public JsonResult DeleteTask(int Id)
        {
            DataTable table = new DataTable();
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();

                //Hankitaan tehtävän avulla näille arvot jotta ne voidaan myös poistaa
                int TagId;
                int StatusId;

                string getIdsQuery = "SELECT TagId, StatusId FROM Task WHERE Id = @Id";
                using (SqlCommand MyCommand = new SqlCommand(getIdsQuery, myCon))
                {
                    MyCommand.Parameters.AddWithValue("@Id", Id);
                    
                    using (SqlDataReader reader = MyCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TagId = reader.GetInt32(0);
                            StatusId = reader.GetInt32(1);
                        }
                        else
                        {
                            // Task not found
                            return new JsonResult("Tehtävää ei löytynyt");
                        }
                    }
                }
                // Tehtävän poisto
                string deleteTaskQuery = "DELETE FROM Task WHERE Id = @Id";
                using (SqlCommand MyCommand = new SqlCommand(deleteTaskQuery, myCon))
                {
                    MyCommand.Parameters.AddWithValue("@Id", Id);
                    MyCommand.ExecuteNonQuery();
                    
                }
                //Lopuksi poistetaan tag ja status joka oli yhdessä tehtävän kanssa
                DeleteTag(TagId);
                DeleteStatus(StatusId);

                myCon.Close();

            }
            return new JsonResult("Poistettu onnistuneesti");
        }

        //Komento jolla poistetaan tägi samalla kun tehtävä poistuu
        private void DeleteTag(int Id)
        {
            string deleteTagQuery = "DELETE FROM dbo.Tag WHERE Id = @Id";
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand deleteCommand = new SqlCommand(deleteTagQuery, myCon))
                {

                    deleteCommand.Parameters.AddWithValue("@Id", Id);
                    deleteCommand.ExecuteNonQuery();
                    myCon.Close();
                    
                }
            }
            
        }
        //Funktio jolla saadaan poistettua status kun tehtävä tai aktiviteetti poistetaan
        private void DeleteStatus(int Id)
        {
            string deleteStatusQuery = "DELETE FROM dbo.Status WHERE Id = @Id";
            string sqlDatasource = _configuration.GetConnectionString("taskDBCon");
            using (SqlConnection myCon = new SqlConnection(sqlDatasource))
            {
                myCon.Open();
                using (SqlCommand deleteCommand = new SqlCommand(deleteStatusQuery, myCon))
                {

                    deleteCommand.Parameters.AddWithValue("@Id", Id);
                    deleteCommand.ExecuteNonQuery();
                    myCon.Close();

                }
            }

        }


    }
}
