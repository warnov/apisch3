using System;
using System.Net.Http;


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


        //Insert in db
        public void SaveRating()
        {

        }
    }
}