using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Flurl;

namespace Configuration.Vault.Hashicorp.Extensions
{
    public static class VaultExtensions
    {
        const string VAULT_TOKEN_HEADER = "X-Vault-Token";
        const string JSON_CONTENT = "application/json";
        public static IConfiguration AddVault(this IConfiguration configuration, string configurationSectionName)
        {
            return configuration.AddVault(configuration.GetSection(configurationSectionName));
        }

        public static IConfiguration AddVault(this IConfiguration configuration, IConfigurationSection configurationSection)
        {
            var vaultConfiguration = new VaultConfiguration();
            configurationSection.Bind(vaultConfiguration);
            return configuration.AddVault(vaultConfiguration);
        }

        public static IConfiguration AddVault(this IConfiguration configuration, Func<VaultConfiguration> vaultConfiguration)
        {
            return configuration.AddVault(vaultConfiguration.Invoke());
        }

        public static IConfiguration AddVault(this IConfiguration configuration, VaultConfiguration vaultConfiguration)
        {
            using (HttpClient secretHttpClient = new HttpClient())
            {
                string url = vaultConfiguration.Url.AppendPathSegment($"/v1/{vaultConfiguration.Backend}/data/{vaultConfiguration.Secret}");

                #region Authentication
                if (!string.IsNullOrWhiteSpace(vaultConfiguration.Token))
                {
                    secretHttpClient.DefaultRequestHeaders.Add(VAULT_TOKEN_HEADER, vaultConfiguration.Token);
                }
                else
                {
                    using (HttpClient authHttpClient = new HttpClient())
                    {
                        string authUrl = vaultConfiguration.Url.AppendPathSegment($"/v1/auth/userpass/login/{vaultConfiguration.Username}");
                        var authResponse = authHttpClient.PostAsync(authUrl, new StringContent(
                                JsonSerializer.Serialize(new { password = vaultConfiguration.Password }),
                                Encoding.UTF8,
                                JSON_CONTENT)).Result.EnsureSuccessStatusCode();

                        var authResponseDoc = JsonDocument.Parse(authResponse.Content.ReadAsStringAsync().Result);
                        var clientToken = authResponseDoc.RootElement.GetProperty("auth").GetProperty("client_token").GetString();
                        secretHttpClient.DefaultRequestHeaders.Add(VAULT_TOKEN_HEADER, clientToken);
                    }
                }
                #endregion

                secretHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT));
                var response = secretHttpClient.GetStringAsync(url).Result;

                using var doc = JsonDocument.Parse(response);
                var data = doc.RootElement.GetProperty("data").GetProperty("data");

                var keys = data.EnumerateObject();
                foreach (var key in keys)
                {
                    configuration[key.Name] = key.Value.GetString();
                }

                return configuration;
            }
        }

    }
}
