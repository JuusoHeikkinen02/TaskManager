using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

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
    }
}
