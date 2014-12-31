using System;
using System.Configuration;
using System.Globalization;
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
    //TODO :Clean up code.
    public class DocumentService
    {
        private static readonly string EndpointUrl = ConfigurationManager.AppSettings["EndPointUrl"];
        private static readonly string AuthorizationKey = ConfigurationManager.AppSettings["AuthorizationKey"];
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["DatabaseId"];

        public static string GetSignature(string masterKey, string resourceId, string resourceType, string xDate = null,
            string date = null)
        {
            if (string.IsNullOrEmpty(xDate) && string.IsNullOrEmpty(date))
            {
                throw new ArgumentException("Either xDate or date must be provided.");
            }


            const string AuthorizationFormat = "type={0}&ver={1}&sig={2}";

            const string MasterToken = "master";

            const string TokenVersion = "1.0";


            byte[] masterKeyBytes = Convert.FromBase64String(masterKey);

            var hmacSha256 = new HMACSHA256(masterKeyBytes);

            string resourceIdInput = resourceId ?? string.Empty;

            string resourceTypeInput = resourceType ?? string.Empty;


            if (!string.IsNullOrEmpty(date))
            {
                date = date.ToLowerInvariant();
            }
            else
            {
                date = string.Empty;
            }


            string payLoad = string.Format(CultureInfo.InvariantCulture,
                "{0}\n{1}\n{2}\n{3}\n{4}\n",
                "GET".ToLowerInvariant(),
                resourceTypeInput.ToLowerInvariant(),
                resourceIdInput.ToLowerInvariant(),
                (xDate ?? string.Empty).ToLowerInvariant(),
                date);

            byte[] hashPayLoad = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(payLoad));

            string authorizationToken = Convert.ToBase64String(hashPayLoad);


            return HttpUtility.UrlEncode(string.Format(
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
                client.CreateDatabaseQuery().Where(db => db.Id == DatabaseId).AsEnumerable().First();

            // Check to verify a document collection with the id=FamilyCollection does not exist
            DocumentCollection documentCollection =
                client.CreateDocumentCollectionQuery(database.CollectionsLink)
                    .Where(c => c.Id == "ErmsBuildsCollection")
                    .AsEnumerable()
                    .First();

            IQueryable<dynamic> documetnObjects = client.CreateDocumentQuery(documentCollection.DocumentsLink,
                "SELECT * " +
                "FROM ErmsBuildsCollection f " +
                "WHERE f.id = \"ermsBuilds\"");
            string val = string.Empty;
            foreach (dynamic document in documetnObjects)
            {
                val += document;
            }

            return (BuildResult) JsonConvert.DeserializeObject(val, typeof (BuildResult));
        }

        public static async Task<string> GetBuilds()
        {
            string responseBody = null;

            try
            {
                string username = "";
                string password = "";


                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            Encoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));

                    using (HttpResponseMessage response = client.GetAsync(
                        "https://aiserms.visualstudio.com/DefaultCollection/_apis/build/builds").Result)
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
                client.CreateDatabaseQuery().Where(db => db.Id == DatabaseId).AsEnumerable().First();


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

            var account = JsonConvert.DeserializeObject<BuildResult>(await GetBuilds());

            account.Id = "ermsBuilds";

            dynamic ermsDocument =
                client.CreateDocumentQuery<Document>(documentCollection.SelfLink).Where(
                    d => d.Id == "ermsBuilds").AsEnumerable().FirstOrDefault();

            if (ermsDocument != null)
            {
                await client.ReplaceDocumentAsync(ermsDocument.SelfLink, account);
            }

            else
            {
                await client.CreateDocumentAsync(documentCollection.DocumentsLink, account);
            }
        }
    }
}