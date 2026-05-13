using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolidWorksNLPPlugin
{
    public class CloudLlmClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _modelName;
        private bool _disposed;

        public CloudLlmClient(string apiKey, string apiUrl = "https://api.openai.com/v1/chat/completions", string modelName = "gpt-4")
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _apiUrl = apiUrl;
            _modelName = modelName;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        public async Task<ScriptGenerationResult> GenerateScriptAsync(string userInput)
        {
            try
            {
                var prompt = BuildScriptPrompt(userInput);
                var response = await SendChatRequestAsync(prompt);
                
                if (string.IsNullOrEmpty(response))
                {
                    return new ScriptGenerationResult
                    {
                        Success = false,
                        ErrorMessage = "Failed to get response from API"
                    };
                }

                return ParseScriptResponse(response, userInput);
            }
            catch (Exception ex)
            {
                return new ScriptGenerationResult
                {
                    Success = false,
                    ErrorMessage = $"API error: {ex.Message}"
                };
            }
        }

        private string BuildScriptPrompt(string userInput)
        {
            return $@"You are a SolidWorks API script generator. Generate C# code that uses SolidWorks API to perform the requested modeling operation.

## Requirements:
1. Use SolidWorks.Interop.sldworks and SolidWorks.Interop.swconst
2. The code should be in a method format
3. Assume you have access to an active SldWorks object named 'swApp'
4. Assume you have access to an active ModelDoc2 named 'modelDoc'
5. Only generate the C# code without any explanations
6. Keep the code safe and efficient
7. Include error handling
8. Use meters as length unit

## Output format:
{{
  ""script"": ""// C# code here"",
  ""description"": ""brief description of what the script does"",
  ""parameters"": {{}}
}}

Now generate the script for: {userInput}";
        }

        private async Task<string> SendChatRequestAsync(string prompt)
        {
            try
            {
                var requestBody = new
                {
                    model = _modelName,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a SolidWorks API script generation assistant." },
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.3,
                    max_tokens = 2000
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseString);
                
                return jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API request failed: {ex.Message}");
                return null;
            }
        }

        private ScriptGenerationResult ParseScriptResponse(string response, string originalInput)
        {
            try
            {
                var jsonStart = response.IndexOf('{');
                var jsonEnd = response.LastIndexOf('}') + 1;
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var json = response.Substring(jsonStart, jsonEnd - jsonStart);
                    var result = JsonConvert.DeserializeObject<ScriptGenerationResponse>(json);
                    
                    if (result != null && !string.IsNullOrEmpty(result.Script))
                    {
                        return new ScriptGenerationResult
                        {
                            Success = true,
                            Script = result.Script,
                            Description = result.Description,
                            OriginalInput = originalInput,
                            Parameters = result.Parameters ?? new Dictionary<string, object>()
                        };
                    }
                }
                
                return new ScriptGenerationResult
                {
                    Success = false,
                    ErrorMessage = "Could not parse the API response format"
                };
            }
            catch (Exception ex)
            {
                return new ScriptGenerationResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to parse response: {ex.Message}"
                };
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        private class ScriptGenerationResponse
        {
            [JsonProperty("script")]
            public string Script { get; set; }
            
            [JsonProperty("description")]
            public string Description { get; set; }
            
            [JsonProperty("parameters")]
            public Dictionary<string, object> Parameters { get; set; }
        }
    }

    public class ScriptGenerationResult
    {
        public bool Success { get; set; }
        public string Script { get; set; }
        public string Description { get; set; }
        public string OriginalInput { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public string ErrorMessage { get; set; }
    }
}
