using System;
using System.Collections.Generic;
using System.Web.Mvc;
using imdbmovierecommender.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace imdbmovierecommender.Controllers
{
    public class MoviesController : Controller
    {
        public static string recommendeditems;
        // GET: Movies  
        public ActionResult Choose()
        {
            Movies movies = new Movies();

            ViewData["movies"] = movies; 
            return View();
        }

        static async Task InvokeRequestResponseService(int id)
        {
            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {
                    Inputs = new Dictionary<string, List<Dictionary<string, string>>>() {
                        {
                            "input1",
                            new List<Dictionary<string, string>>(){new Dictionary<string, string>(){
                                            {
                                                "UserId", "1"
                                            },
                                            {
                                                "MovieId", id.ToString()
                                            },
                                            {
                                                "Rating", "10"
                                            },
                                }
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };

                const string apiKey = "a0d2hnuGgjgVYYnUSP72QVcbvAcRrEGYHonelgYeW3wSYndTse1INOF2EOzzTByoPK4istw2F0TZstOPSqVQfg=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/3e97e535695a4dffb13d62ef0d77c2de/services/ce4aa637cb544d3480705fa8c56504bf/execute?api-version=2.0&format=swagger");

                // WARNING: The 'await' statement below can result in a deadlock
                // if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false)
                // so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:b
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)
                
                HttpResponseMessage response = client.PostAsJsonAsync("", scoreRequest).Result;  


                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    MoviesController.recommendeditems = result;
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp,
                    // which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
        }
}
        public ActionResult Recommend(int id)
        {
            InvokeRequestResponseService(id).Wait();
            Movies movies = new Movies();
            int[] movieselector = new int[Movies.moviestorecommend];
            
            string parsestr = MoviesController.recommendeditems;
            var obj = JObject.Parse(parsestr);
            String text; 
            for (int i = 0; i < Movies.moviestorecommend; i++)
            {
                text = (string)obj["Results"]["output1"][0]["Related Item " + (i + 1)];
                movieselector[i] = Int32.Parse(text);
            }
            ViewData["movies"] = movies; 
            ViewData["movieselector"] = movieselector;
            return View();
        }

        public ActionResult Watch()
        {
            return View();
        }
    }
}