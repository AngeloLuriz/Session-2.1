using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Session2
{
    [TestClass]
    public class UnitTest1
    {
        private static HttpClient httpClient;
        private static readonly string BaseURL = "https://petstore.swagger.io/v2";
        private static readonly string UserEndpoint = "pet";
        private static string GetURL(string endpoint) => $"{BaseURL}{endpoint}";
        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetClass> cleanUpList = new List<PetClass>();

        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            httpClient = new HttpClient();
        }

        [TestMethod]
        public async Task PutMethod() {

            #region create data
            //create json obhject
            PetClass petData = new PetClass()
            {
                Id = 1,
                Name = "SampleName",
                PhotoUrls = new string[1] { "PhotoURL" },
                Category = new Category { Id = 1, Name = "Category" },
                Tags = new Category[1] { new Category { Id = 1, Name = "Tags" } },
                Status = "available"
            };
            // Serialize Content
            var request = JsonConvert.SerializeObject(petData);
            var postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            // send put request
            await httpClient.PostAsync(GetURL(UserEndpoint), postRequest);

            #endregion

            var getResponse = await httpClient.GetAsync(GetURI($"{UserEndpoint}/{petData.Name}"));

            var listPetData = JsonConvert.DeserializeObject<PetClass>(getResponse.Content.ReadAsStringAsync().Result);
            var Petdata = listPetData.Name;

            #region update data
            petData = new PetClass()
            {
                Id = 2,
                Name = "Updated Name",
                PhotoUrls = new string[1] { "Updated PhotoURL" },
                Category = new Category { Id = 1, Name = "Updated Category" },
                Tags = new Category[1] { new Category { Id = 1, Name = "Updated Tags" } },
                Status = "available"
            };

            request = JsonConvert.SerializeObject(petData);
            postRequest = new StringContent(request, Encoding.UTF8, "application/json");

            var httpResponse = await httpClient.PutAsync(GetURL($"{UserEndpoint}"), postRequest);

            httpResponse.EnsureSuccessStatusCode();
            #endregion

            #region get updated data 
            // Get Request
            getResponse = await httpClient.GetAsync(GetURI($"{UserEndpoint}/{petData.Id}"));

            // Deserialize Content
            listPetData = JsonConvert.DeserializeObject<PetClass>(getResponse.Content.ReadAsStringAsync().Result);

            // filter created data
            string createdPetName = listPetData.Name;
            //Get Status code
            var statusCode = httpResponse.StatusCode;

            #endregion

            #region cleanup data

            // Add data to cleanup list
            cleanUpList.Add(listPetData);

            #endregion

            #region assertion

            // Assertion
            Assert.AreEqual(HttpStatusCode.OK, statusCode, "Status code is not equal to 200");
            Assert.AreEqual(petData.Name, createdPetName, "Username not matching");

            #endregion
        }
    }
}