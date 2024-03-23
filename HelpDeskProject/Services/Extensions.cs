using System.Text.RegularExpressions;

namespace HelpDeskProject.Services
{
    public class Extensions
    {
        public Extensions()
        {

        }
        public async Task<string> GetIPAddress()
        {
            try
            {
                var externalIp = (await (new HttpClient()).GetAsync("http://checkip.dyndns.org/")).Content.ToString();
                externalIp = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.d{1,3}")).Matches(externalIp)[0].ToString();
                return externalIp;
            }
            catch {
                return null;
            }
        }
    }
}
