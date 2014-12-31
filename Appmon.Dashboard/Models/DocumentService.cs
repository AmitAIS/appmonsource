using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace BuildManager.Models
{
    public class DocumentService
    {
        private static readonly string EndpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];

        public static string GetSignature(string masterKey, string resourceId, string resourceType, string xDate = null, string date = null)
        {


            if (string.IsNullOrEmpty(xDate) && string.IsNullOrEmpty(date))
            {

                throw new ArgumentException("Either xDate or date must be provided.");

            }



            const string AuthorizationFormat = "type={0}&ver={1}&sig={2}";

            const string MasterToken = "master";

            const string TokenVersion = "1.0";



            var masterKeyBytes = Convert.FromBase64String(masterKey);

            var hmacSha256 = new HMACSHA256(masterKeyBytes);

            var resourceIdInput = resourceId ?? string.Empty;

            var resourceTypeInput = resourceType ?? string.Empty;



            if (!string.IsNullOrEmpty(date))
            {
                date = date.ToLowerInvariant();
            }
            else
            {
                date = string.Empty;
            }



            var payLoad = string.Format(CultureInfo.InvariantCulture,

            "{0}\n{1}\n{2}\n{3}\n{4}\n",

            "GET".ToLowerInvariant(),

            resourceTypeInput.ToLowerInvariant(),

            resourceIdInput.ToLowerInvariant(),

            (xDate ?? string.Empty).ToLowerInvariant(),

            date);

            var hashPayLoad = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(payLoad));

            var authorizationToken = Convert.ToBase64String(hashPayLoad);



            return System.Web.HttpUtility.UrlEncode(string.Format(

            CultureInfo.InvariantCulture,

            AuthorizationFormat,

            MasterToken,

            TokenVersion,

            authorizationToken));

        }

        public static BuildResult GetBuildResult()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions
            // Create a new instance of the DocumentClient
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            // Check to verify a database with the id=FamilyRegistry does not exist
            Database database =
                client.CreateDatabaseQuery().Where(db => db.Id == "aisdevops").AsEnumerable().First();

            // Check to verify a document collection with the id=FamilyCollection does not exist
            DocumentCollection documentCollection =
                client.CreateDocumentCollectionQuery(database.CollectionsLink)
                    .Where(c => c.Id == "ErmsBuildsCollection")
                    .AsEnumerable()
                    .First();

            var families = client.CreateDocumentQuery(documentCollection.DocumentsLink,
                "SELECT * " +
                "FROM ErmsBuildsCollection f " +
                "WHERE f.id = \"ermsBuilds\"");
            string val = string.Empty;
            foreach (var family in families)
            {
                val += family;
            }

            return (BuildResult)JsonConvert.DeserializeObject(val, typeof(BuildResult));

        }

        public static async Task<string> GetBuilds()
        {
            string responseBody = null;

            try
            {
                var username = "helloworld";
                var password = "QwerAsdD123";


                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));

                    using (HttpResponseMessage response = client.GetAsync(
                                "https://ais-dcm.visualstudio.com/DefaultCollection/_apis/build/builds").Result)
                    {
                        response.EnsureSuccessStatusCode();
                        responseBody = await response.Content.ReadAsStringAsync();
                        // WriteToFile(responseBody);
                        //Console.WriteLine(responseBody);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return responseBody;

        }

        public static async Task PushBuilds()
        {
            // Make sure to call client.Dispose() once you've finished all DocumentDB interactions
            // Create a new instance of the DocumentClient
            var client = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey);

            // Check to verify a database with the id=FamilyRegistry does not exist
            Database database =
                client.CreateDatabaseQuery().Where(db => db.Id == "aisdevops").AsEnumerable().First();


            // Check to verify a document collection with the id=FamilyCollection does not exist
            DocumentCollection documentCollection =
                client.CreateDocumentCollectionQuery(database.CollectionsLink)
                    .Where(c => c.Id == "ErmsBuildsCollection")
                    .AsEnumerable()
                    .FirstOrDefault();

            if (documentCollection == null)
            {
                // Create a document collection
                documentCollection = await client.CreateDocumentCollectionAsync(database.CollectionsLink,
                    new DocumentCollection
                    {
                        Id = "ErmsBuildsCollection"
                    });
            }

            BuildResult account = JsonConvert.DeserializeObject<BuildResult>(await GetBuilds());

            account.Id = "ermsBuilds";

            dynamic gameDocument =
        client.CreateDocumentQuery<Document>(documentCollection.SelfLink).Where(
        d => d.Id == "ermsBuilds").AsEnumerable().FirstOrDefault();

            if (gameDocument != null)
            {
                await client.ReplaceDocumentAsync(gameDocument.SelfLink, account);
            }

            else
            {
                await client.CreateDocumentAsync(documentCollection.DocumentsLink, account);
            }

        }

    }
}