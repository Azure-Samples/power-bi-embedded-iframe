using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace PBIEmbedded
{
    /// <summary>
    /// The Power BI app token used to authenticate with Power BI Embedded services
    /// </summary>
    public class PowerBIToken
    {
        private const int DefaultExpirationSeconds = 3600;
        private const string DefaultIssuer = "PowerBISDK";
        private const string DefaultAudience = "https://analysis.windows.net/powerbi/api";

        /// <summary>
        /// The token issuer
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// The audience this token is valid for
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets a collection of claims associated with this token
        /// </summary>
        public ICollection<Claim> Claims { get; private set; }

        /// <summary>
        /// Gets or set the access key used to sign the app token
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Gets or sets the token expiration.  Default expiration is 1 hour
        /// </summary>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// The Power BI supported claim types
        /// </summary>
        public static class ClaimTypes
        {
            /// <summary>
            /// The version claim
            /// </summary>
            public const string Version = "ver";
            /// <summary>
            /// The token type claim
            /// </summary>
            public const string Type = "type";
            /// <summary>
            /// The workspace collection claim
            /// </summary>
            public const string WorkspaceCollectionName = "wcn";
            /// <summary>
            /// The workspace id claim
            /// </summary>
            public const string WorkspaceId = "wid";
            /// <summary>
            /// The report id claim
            /// </summary>
            public const string ReportId = "rid";
        }


        /// <summary>
        /// Represents an access token used to authenticate and authorize against Power BI Platform services
        /// </summary>
        public PowerBIToken()
        {
            this.Claims = new List<Claim>();
            this.Claims.Add(new Claim(ClaimTypes.Version, "0.1.0"));
            this.Issuer = DefaultIssuer;
            this.Audience = DefaultAudience;
            this.Expiration = DateTime.UtcNow.AddSeconds(DefaultExpirationSeconds);
        }

        /// <summary>
        /// Create a developer token with default expiration used to access Power BI platform services
        /// </summary>
        /// <param name="workspaceCollectionName">The workspace collection name</param>
        /// <param name="workspaceId">The workspace id</param>
        /// <param name="expiration">The token expiration date/time</param>
        /// <returns>The Power BI access token</returns>
        public static PowerBIToken CreateDevToken(string workspaceCollectionName, string workspaceId)
        {

            var token = new PowerBIToken
            {
                Expiration = DateTime.UtcNow.Add(TimeSpan.FromSeconds(DefaultExpirationSeconds))
            };

            token.Claims.Add(new Claim(ClaimTypes.Type, "dev"));
            token.Claims.Add(new Claim(ClaimTypes.WorkspaceCollectionName, workspaceCollectionName));
            token.Claims.Add(new Claim(ClaimTypes.WorkspaceId, workspaceId));

            return token;
        }

        /// <summary>
        /// Create a embed token with default expiration used to embed Power BI components into your own applications
        /// </summary>
        /// <param name="workspaceCollectionName">The workspace collection name</param>
        /// <param name="workspaceId">The workspace id</param>
        /// <param name="reportId">The report id</param>
        /// <param name="expiration">The token expiration date/time</param>
        /// <returns>The Power BI access token</returns>
        public static PowerBIToken CreateReportEmbedToken(string workspaceCollectionName, string workspaceId, string reportId)
        {
            var token = new PowerBIToken
            {
                Expiration = DateTime.UtcNow.Add(TimeSpan.FromSeconds(DefaultExpirationSeconds))
            };

            token.Claims.Add(new Claim(ClaimTypes.Type, "embed"));
            token.Claims.Add(new Claim(ClaimTypes.WorkspaceCollectionName, workspaceCollectionName));
            token.Claims.Add(new Claim(ClaimTypes.WorkspaceId, workspaceId));
            token.Claims.Add(new Claim(ClaimTypes.ReportId, reportId));

            return token;
        }

        /// <summary>
        /// Generates an app token with the specified claims and signs it the the configured signing key.
        /// </summary>
        /// <param name="accessKey">The access key used to sign the app token</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Generate(string accessKey = null)
        {
            if (string.IsNullOrWhiteSpace(this.AccessKey) && accessKey == null)
            {
                throw new ArgumentNullException(nameof(accessKey));
            }

            accessKey = accessKey ?? this.AccessKey;

            var key = Encoding.UTF8.GetBytes(accessKey);
            var signingCredentials = new SigningCredentials(new InMemorySymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);
            var token = new JwtSecurityToken(this.Issuer, this.Audience, this.Claims, DateTime.UtcNow, this.Expiration, signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
