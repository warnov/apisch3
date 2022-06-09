using System;
using System.Collections.Generic;
using System.Net.Http;
using Azure.Identity;
using Microsoft.Azure.Cosmos;


namespace Company.Function
{
    public class Rating
    {
        public string userId { get; set; }
        public string productId { get; set; }
        public string locationName { get; set; }
        public int rating { get; set; }
        public string userNotes { get; set; }
        public string id { get; set; }
        public DateTime timestamp { get; set; }


        public bool Validate()
        {
            //Validate Rating
            if (rating < 0 || rating > 5) return false;

            //Validate Product
            var url = $"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={productId}";
            var httpClient = new HttpClient();
            var response = httpClient.GetAsync(url).Result;
            if (response.StatusCode != System.Net.HttpStatusCode.OK) return false;

            //Validate User
            url = $"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={userId}";
            response = httpClient.GetAsync(url).Result;
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public void CompleteInfo()
        {
            id = Guid.NewGuid().ToString();
            timestamp = DateTime.UtcNow;
        }

        private static Container RatingsContainer
        {
            get
            {
                // The Azure Cosmos DB endpoint for running this sample.
                string EndpointUri = "";

                // The primary key for the Azure Cosmos account.
                string PrimaryKey = "";

                // The Cosmos client instance
                CosmosClient cosmosClient = new CosmosClient(EndpointUri, PrimaryKey);
                /* CosmosClient cosmosClient = new CosmosClient(
                     accountEndpoint: Environment.GetEnvironmentVariable("COSMOS_ENDPOINT", EnvironmentVariableTarget.Process),
                     new DefaultAzureCredential()
                 );*/

                // The database
                Database database = cosmosClient.GetDatabase("CDBApisCh3");

                // The container 
                return database.GetContainer("ratings");
            }
        }

        //Insert in db
        public void SaveRating()
        {
            var rating = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            ItemResponse<Rating> ratingResponse =
            RatingsContainer.CreateItemAsync<Rating>(this, new PartitionKey(this.id)).Result;
        }

        public static Rating LoadFromDB(string ratingId)
        {
            ItemResponse<Rating> response =
            RatingsContainer.ReadItemAsync<Rating>(
                partitionKey: new PartitionKey(ratingId),
                id: ratingId
            ).Result;

            return response.Resource;
        }

        public static List<Rating> RatingsFromUser(string userId)
        {
            var sqlQueryText = $"SELECT * FROM c WHERE c.userId = '{userId}'";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            using FeedIterator<Rating> queryResultSetIterator = RatingsContainer.GetItemQueryIterator<Rating>(queryDefinition);
            List<Rating> ratings = new List<Rating>();

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<Rating> currentResultSet = queryResultSetIterator.ReadNextAsync().Result;
                foreach (Rating rating in currentResultSet)
                {
                    ratings.Add(rating);
                }
            }

            return ratings;
        }
    }
}