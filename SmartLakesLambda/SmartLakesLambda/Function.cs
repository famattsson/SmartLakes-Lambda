using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Text;
using System.IO;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SmartLakesLambda
{   
    public class Function
    {
        struct TempRecord
        {   
            public float temperature;
            public string location;
            public string time;
        }

        public string FunctionHandler(Stream input,ILambdaContext context)
        {
            StreamReader streamReader = new StreamReader(input, Encoding.UTF8);
            string inputString = streamReader.ReadToEnd();
            JObject inputJson = JObject.Parse(inputString);
            Console.Write(inputJson);
            return "Rows affected: " + AddTempRecord(inputJson);
            
        }

        int AddTempRecord(JObject input)
        {
            TempRecord tempRecord = new TempRecord();
            var tempHexBytes = input.SelectToken("payload_hex").ToString().Substring(12);
            tempRecord.temperature = Convert.ToInt32(tempHexBytes, 16)/10;
            tempRecord.location = "Källbergsgatan 3";
            DateTime date = DateTime.UtcNow;
            date = date.AddHours(2);
            tempRecord.time = date.ToString("dd/MM/yyyy HH:mm:ss");

            SqlConnection sqlConnection = new SqlConnection("Data Source=smartlakes.cnhjmr9oh4re.us-east-2.rds.amazonaws.com;Initial Catalog=SmartLakes;Integrated Security=False;User ID=SmartLakesNFK;Password=NFKsmartlakes;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            string query = "INSERT INTO TempRecords VALUES(@temperature, @location, @time)";
            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.Add("@temperature", SqlDbType.Float).Value = tempRecord.temperature;
                command.Parameters.Add("@location", SqlDbType.VarChar).Value = tempRecord.location;
                command.Parameters.Add("@time", SqlDbType.VarChar).Value = tempRecord.time;
                command.Connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                command.Connection.Close();
                return rowsAffected;
            }

        }
    }
}
