using System.IO;
using System.Net;
using System.Net.Http;

namespace TeddyBlazor.Services
{
    public class DadJokeService
    {

        public string GetDadJoke()
        {
            var request = (HttpWebRequest)WebRequest.Create("https://icanhazdadjoke.com/");
            request.Headers.Add("Accept", "text/plain");
            
            using (var response = (HttpWebResponse)request.GetResponse())
             using (var stream = response.GetResponseStream())
             using (var reader = new StreamReader(stream))
            {
                //return response.ToString();
                return reader.ReadToEnd();
            }
        }
    }
}