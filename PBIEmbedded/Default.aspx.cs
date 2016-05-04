
using System;
using System.Web.UI;
using Newtonsoft.Json;
using PBIWebApp.Properties;

namespace PBIEmbedded
{
     /* NOTE: This sample illustrates how to authenticate a Power BI web app using the app token flow. 
     * In a production application, you should provide appropriate exception handling and refactor authentication settings into 
     * a configuration. Authentication settings are hard-coded in the sample to make it easier to follow the flow of authentication. */
    public partial class _Default : Page
    {
        //In a production application you might store these values in secure storage.
        private string accessKey = Settings.Default.AccessKey;
        private string workspaceName = Settings.Default.WorkspaceName;
        private string workspaceId = Settings.Default.WorkspaceId;

        /// <summary>
        /// <a name="GetReport"/>
        /// ## Get a report in a workspace
        /// </summary>
        protected void getReportsButton_Click(object sender, EventArgs e)
        {

            //Get an app token to generate a JSON Web Token (JWT). An app token flow is a claims-based design pattern.
            //To learn how you can code an app token flow to generate a JWT, see the PowerBIToken class.
            var appToken = PowerBIToken.CreateDevToken(workspaceName, workspaceId);

            //After you get a PowerBIToken which has Claims including your WorkspaceName and WorkspaceID,
            //you generate JSON Web Token (JWT) . The Generate() method uses classes from System.IdentityModel.Tokens: SigningCredentials, 
            //JwtSecurityToken, and JwtSecurityTokenHandler. 
            string jwt = appToken.Generate(accessKey);

            //Set app token textbox to JWT string to show that the JWT was generated
            appTokenTextbox.Text = jwt;

            //Construct reports uri resource string
            var uri = String.Format("https://api.powerbi.com/beta/collections/{0}/workspaces/{1}/reports", workspaceName, workspaceId);

            //Configure reports request
            System.Net.WebRequest request = System.Net.WebRequest.Create(uri) as System.Net.HttpWebRequest;
            request.Method = "GET";
            request.ContentLength = 0;

            //Set the WebRequest header to AppToken, and jwt
            //Note the use of AppToken instead of Bearer
            request.Headers.Add("Authorization", String.Format("AppToken {0}", jwt));

            //Get reports response from request.GetResponse()
            using (var response = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get reader from response stream
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    string json = reader.ReadToEnd();
                    //Deserialize JSON string into PBIReports
                    PBIReports PBIReports = JsonConvert.DeserializeObject<PBIReports>(json);

                    //Clear Textbox
                    tb_reportsResult.Text = string.Empty;

                    //Get each report 
                    foreach (PBIReport rpt in PBIReports.value)
                    {
                        tb_reportsResult.Text += String.Format("{0}\t{1}\t{2}\n", rpt.id, rpt.name, rpt.embedUrl);
                    }
                }
            }
        }

        /// <summary>
        /// <a name="EmbedReportToken"/>
        /// ### Get an app token for a report
        /// </summary>
        protected void getReportAppTokenButton_Click(object sender, EventArgs e)
        {
            //Get an embed token for a report. 
            var token = PowerBIToken.CreateReportEmbedToken(workspaceName, workspaceId, getReportAppTokenTextBox.Text);

            //After you get a PowerBIToken which has Claims including your WorkspaceName and WorkspaceID,
            //you generate JSON Web Token (JWT) . The Generate() method uses classes from System.IdentityModel.Tokens: SigningCredentials, 
            //JwtSecurityToken, and JwtSecurityTokenHandler. 
            string jwt = token.Generate(accessKey);

            //Set textbox to JWT string. The JavaScript embed code uses the embed jwt for a report. 
            reportAppTokenTextbox.Text = jwt;
        }
    }

    public class PBIReports
    {
        public PBIReport[] value { get; set; }
    }
    public class PBIReport
    {
        public string id { get; set; }
        public string name { get; set; }
        public string webUrl { get; set; }     
        public string embedUrl { get; set; } 
    }
}