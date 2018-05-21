using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace SmartLakesLambda
{
     
    public class Function
    {
        public string FunctionHandler(JObject input,ILambdaContext context)
        {
            return input.SelectToken("payload.payload_hex").ToString();
        }
    }
}
