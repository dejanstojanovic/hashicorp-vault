using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Flurl;

namespace Configuration.Vault.Hashicorp.Extensions
{
    public static class VaultExtensions
    {

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
                if (!string.IsNullOrWhiteSpace(vaultConfiguration.Token))
                {
                    secretHttpClient.DefaultRequestHeaders.Add("X-Vault-Token", vaultConfiguration.Token);
                }
                else
                {
                    using (HttpClient authHttpClient = new HttpClient())
                    {

                    }
                }
                secretHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
