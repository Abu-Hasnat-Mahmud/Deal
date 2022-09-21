using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Deal.Helpers
{

    public static class SQlHelper
    {
        //public IConfiguration Configuration { get; }

        //public SQlHelper(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}


        static IServiceProvider services = null;
        /// <summary>
        /// Provides static access to the framework's services provider
        /// </summary>
        public static IServiceProvider Services
        {
            get { return services; }
            set
            {
                if (services != null)
                {
                    throw new Exception("Can't set once a value has already been set.");
                }
                services = value;
            }
        }

        public static HttpContext Current
        {
            get
            {
                IHttpContextAccessor httpContextAccessor = services.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                return httpContextAccessor?.HttpContext;
            }
        }

       
        static readonly string conString = "Server=36.255.68.118; Database=MeghnaDB; user id=meghna; password=meghna@1234; MultipleActiveResultSets=true; Connection Timeout=300;";


        //helper snippet to exec a procedure and map the result to a generic type of class
        public static List<T> ExecProcMapTList<T>(string commandText, SqlParameter[] sqlParameter = null) where T : class, new()
        {
            try
            {

                // AppData.AppData.globalException = "";
                SqlConnection connection = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                if (sqlParameter != null)
                {
                    cmd.Parameters.AddRange(sqlParameter);
                }
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);


                connection.Close();

                //map datatable to a geniric class type list

                List<T> list = new List<T>();

                if (dataTable.Rows.Count > 0)
                {
                    var serializedMyObjects = JsonConvert.SerializeObject(dataTable);

                    list = (List<T>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<T>));
                }


                return list;

            }
            catch (Exception ex)
            {
                //AppData.AppData.globalException = ex.Message;
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static T ExecProcMapT<T>(string commandText, SqlParameter[] sqlParameter = null) where T : class, new()
        {
            try
            {
                SqlConnection connection = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                if (sqlParameter != null)
                {
                    cmd.Parameters.AddRange(sqlParameter);
                }
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);


                connection.Close();
                T obj = new T();
                //map datatable to a geniric class type list
                foreach (var row in dataTable.AsEnumerable())
                {


                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                return obj;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static DataTable GetDataTable(string commandText, SqlParameter[] sqlParameter = null)
        {
            try
            {
                SqlConnection connection = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                if (sqlParameter != null)
                {
                    cmd.Parameters.AddRange(sqlParameter);
                }
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                var dataTable = new DataTable();
                dataTable.Load(reader);


                connection.Close();

                return dataTable;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static List<T> ExecFnc<T>(string commandText, SqlParameter[] sqlParameter = null) where T : class, new()
        {
            try
            {
                SqlConnection connection = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.Text;
                if (sqlParameter != null)
                {
                    cmd.Parameters.AddRange(sqlParameter);
                }
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                //map datatable to a geniric class type list

                List<T> list = new List<T>();

                foreach (var row in dt.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list.Add(obj);
                }

                return list;

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static DataSet ExecProcMapDS(string commandText, SqlParameter[] sqlParameter = null) /*where T : class, new()*/
        {
            try
            {
                SqlConnection connection = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                if (sqlParameter != null)
                {
                    cmd.Parameters.AddRange(sqlParameter);
                }
                connection.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        public static string GTRDate(string strDate)
        {
            if (strDate.Length == 0)
            {
                return "";
            }
            DateTime dt = DateTime.Parse(strDate);
            return string.Format("{0:dd}", dt) + "-" + string.Format("{0:MMM}", dt) + "-" + string.Format("{0:yyyy}", dt);
        }

        public static Int64 GTRCountingDataLarge(string commandText)
        {
            Int64 Result = 0;
            if (commandText.Length == 0 || commandText == string.Empty)
            {
                return Result;
            }

            try
            {
                SqlConnection connection = new SqlConnection(conString);
                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.Text;
                connection.Open();

                Result = (Int64)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                //throw exception to front caller
                throw (ex);
            }

            return Result;
        }

        public static void ExecProc(string commandText, SqlParameter[] sqlParameter = null)
        {
            //SqlConnection connection = new SqlConnection(conString);
            //connection.ConnectionTimeout("15");

            //SqlConnection connectionString = new SqlConnection(conString);


            try
            {
                //AppData.AppData.globalException = "";
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();

                    SqlCommand cmd = new SqlCommand(commandText, connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (sqlParameter != null)
                    {
                        cmd.Parameters.AddRange(sqlParameter);

                    }
                    // Setting command timeout to 1 second  
                    cmd.CommandTimeout = 1000;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Command Successfull");
                    }

                    catch (SqlException e)
                    {
                        Console.WriteLine("Got expected SqlException due to command timeout ");
                        Console.WriteLine(e);
                    }
                }

                //    SqlCommand cmd = new SqlCommand(commandText, connection);
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    if (sqlParameter != null)
                //    {
                //        cmd.Parameters.AddRange(sqlParameter);

                //    }
                //    connection.Open();

                //    cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //AppData.AppData.globalException = ex.Message;
                Console.WriteLine(ex.Message);
            }
            //finally
            //{
            //    connection.Close();
            //}

        }

        public static void GTRSaveDataWithSQLCommand(ArrayList sqlQuery)
        {
            if (sqlQuery.Count == 0)
            {
                throw (new Exception("Empty SQL Query."));
            }

            SqlConnection connection = new SqlConnection(conString);

            SqlCommand cmd;

            connection.Open();

            //Open Connection
            //GTRConnectionOpen();
            cmd = connection.CreateCommand();
            cmd.CommandTimeout = 1200;

            SqlTransaction tran = connection.BeginTransaction();
            cmd.Connection = connection;
            cmd.Transaction = tran;

            try
            {
                //Passing Query & Connection Object to initialize Command
                for (int i = 0; i < sqlQuery.Count; i++)
                {
                    cmd.CommandText = sqlQuery[i].ToString();
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    tran.Rollback();
                }
                catch (SqlException sqlex)
                {
                    if (tran.Connection != null)
                    {
                        Console.WriteLine("An exception of type " + sqlex.GetType() + " was encountered while attempting to roll back the transaction.");
                    }
                }
                throw (ex);
            }
            finally
            {
                //Close Connection
                connection.Close();
            }
        }

        public static void ExecCommand(string commandText)

        {
            try
            {

                SqlConnection connection = new SqlConnection(@"Server=101.2.165.189\MSSQL2014;Database=GTRERP_DAP_LIVE; user id=sa; password=gTr@$#@456*&%^()gT#7; MultipleActiveResultSets=true");

                SqlCommand cmd = new SqlCommand(commandText, connection);
                cmd.CommandType = CommandType.Text;

                connection.Open();

                cmd.ExecuteNonQuery();

            }
            catch (SqlException ex)
            {

                Console.WriteLine(ex.Message);
            }

        }

        public static void PutTempData<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T GetTempData<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }

    }
}
