using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace FirebaseNet.Database
{
    /// <summary>
    /// Firebase manager.
    /// </summary>
    public class FirebaseManager
    {
        public static void SendNotification()
        {
            var firebaseDB = new FirebaseDB(FirebaseConfig.FIREBASE_DATABASE_URL);

            var firebaseDBEmergencia = firebaseDB.Node(FirebaseConfig.EMERGENCIA_TABLE_NAME);
            var data = @"{ 'nueva_emergencia': 1 }";

            var postResponse = firebaseDBEmergencia.Post(data);
            if(postResponse.Success)
            {
                FirebasePushNotificationSenderManager.SendNotificationToChannelAsync("Nueva Emergencia");
            }
            Console.WriteLine(postResponse);
        }
    }

    static class FirebasePushNotificationSenderManager
    {
        public static async void SendNotificationToChannelAsync(string message)
        {
            try
            {
                var jGcmData = new JObject
                {
                    { "to", "/topics/"+FirebaseConfig.TOPIC_CHANNEL_NAME },
                    { "data", new JObject { { "message", message } } }
                };

                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler, true))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, FirebaseConfig.NOTIFICATIONS_URL))
                        {
                            request.Headers.Add("Accept", "application/json"); //Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            request.Headers.TryAddWithoutValidation("Authorization", "key=" + FirebaseConfig.SERVER_KEY);

                            request.Content = new StringContent(jGcmData.ToString(), Encoding.UTF8, "application/json");

                            using (var response = await client.SendAsync(request))
                            {
                                if (response.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("Message sent: check client device notification tray");
                                }
                                else
                                {
                                    Console.WriteLine("Message sent response: StatusCode = " + response.StatusCode.ToString() + " message = " + response.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to send GCM message");
                Console.Error.WriteLine(ex.StackTrace);
            }
        }
    }


    /// <summary>
    /// Firebase db.
    /// </summary>
    public class FirebaseDB
    {
        public FirebaseDB()
        {
        }

        /// <summary>  
        /// Initializes a new instance of the <see cref="FirebaseDB"/> class with base url of Firebase Database  
        /// </summary>  
        /// <param name="baseURL">Firebase Database URL</param>  
        public FirebaseDB(string baseURL)
        {
            this.RootNode = baseURL;
        }

        /// <summary>  
        /// Gets or sets Represents current full path of a Firebase Database resource  
        /// </summary>  
        private string RootNode { get; set; }

        /// <summary>  
        /// Adds more node to base URL  
        /// </summary>  
        /// <param name="node">Single node of Firebase DB</param>  
        /// <returns>Instance of FirebaseDB</returns>  
        public FirebaseDB Node(string node)
        {
            if (node.Contains("/"))
            {
                throw new FormatException("Node must not contain '/', use NodePath instead.");
            }

            return new FirebaseDB(this.RootNode + '/' + node);
        }

        /// <summary>  
        /// Adds more nodes to base URL  
        /// </summary>  
        /// <param name="nodePath">Nodepath of Firebase DB</param>  
        /// <returns>Instance of FirebaseDB</returns>  
        public FirebaseDB NodePath(string nodePath)
        {
            return new FirebaseDB(this.RootNode + '/' + nodePath);
        }

        /// <summary>  
        /// Make Get request  
        /// </summary>  
        /// <returns>Firebase Response</returns>  
        public FirebaseResponse Get()
        {
            return new FirebaseRequest(HttpMethod.Get, this.RootNode).Execute();
        }

        /// <summary>  
        /// Make Put request  
        /// </summary>  
        /// <param name="jsonData">JSON string to PUT</param>  
        /// <returns>Firebase Response</returns>  
        public FirebaseResponse Put(string jsonData)
        {
            return new FirebaseRequest(HttpMethod.Put, this.RootNode, jsonData).Execute();
        }

        /// <summary>  
        /// Make Post request  
        /// </summary>  
        /// <param name="jsonData">JSON string to POST</param>  
        /// <returns>Firebase Response</returns>  
        public FirebaseResponse Post(string jsonData)
        {
            return new FirebaseRequest(HttpMethod.Post, this.RootNode, jsonData).Execute();
        }

        /// <summary>  
        /// Make Patch request  
        /// </summary>  
        /// <param name="jsonData">JSON sting to PATCH</param>  
        /// <returns>Firebase Response</returns>  
        public FirebaseResponse Patch(string jsonData)
        {
            return new FirebaseRequest(new HttpMethod("PATCH"), this.RootNode, jsonData).Execute();
        }

        /// <summary>  
        /// Make Delete request  
        /// </summary>  
        /// <returns>Firebase Response</returns>  
        public FirebaseResponse Delete()
        {
            return new FirebaseRequest(HttpMethod.Delete, this.RootNode).Execute();
        }

        /// <summary>  
        /// To String  
        /// </summary>  
        /// <returns>Current resource URL as string</returns>  
        public override string ToString()
        {
            return this.RootNode;
        }
    }
}
