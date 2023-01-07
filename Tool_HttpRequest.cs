using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;


internal static class Tool_HttpRequest {

    internal static string Get(string route) {
        System.Net.HttpStatusCode code = System.Net.HttpStatusCode.OK;
        return Get(route, ref code);
    }

    internal static string Get(string route, ref System.Net.HttpStatusCode code) {
        using (var httpClient = new HttpClient()) {
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), route)) {
                request.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");

                HttpResponseMessage response = httpClient.SendAsync(request).Result;
                code = response.StatusCode;

                if (!response.IsSuccessStatusCode) return "";
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}

