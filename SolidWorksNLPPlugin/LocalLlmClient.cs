using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SolidWorksNLPPlugin
{
    public class LocalLlmClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _modelName;
        private bool _disposed;

        public LocalLlmClient()
        {
            _httpClient = new HttpClient();
            _apiUrl = "http://localhost:11434/api/chat"; 
            _modelName = "llama2";
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<ParsedCommand> ParseCommandAsync(string userInput)
        {
            try
            {
                var prompt = BuildPrompt(userInput);
                var response = await SendChatRequestAsync(prompt);
                
                if (string.IsNullOrEmpty(response))
                {
                    return ParseWithFallback(userInput);
                }

                return ParseResponse(response);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LLM error: {ex.Message}");
                return ParseWithFallback(userInput);
            }
        }

        private string BuildPrompt(string userInput)
        {
            return $@"You are a SolidWorks CAD modeling assistant. Convert the user's natural language request into a structured JSON command.

Available commands:
- create_primitive: creates basic shapes (cube, box, sphere, cylinder, cone, torus, pyramid)
- create_sketch: creates a 2D sketch
- extrude: extrudes a sketch
- revolve: revolves a sketch around an axis
- assemble: adds assembly constraints
- modify: modifies existing features

Respond ONLY with a JSON object in this format:
{{
  ""command"": ""command_name"",
  ""parameters"": {{
    ""param1"": ""value1"",
    ""param2"": ""value2""
  }}
}}

Examples:
User: create a cube 100mm on each side
Response: {{ ""command"": ""create_primitive"", ""parameters"": {{ ""shape"": ""cube"", ""size"": ""100"" }} }}

User: create a cylinder with diameter 50mm and height 200mm
Response: {{ ""command"": ""create_primitive"", ""parameters"": {{ ""shape"": ""cylinder"", ""diameter"": ""50"", ""height"": ""200"" }} }}

User: extrude the sketch by 50mm
Response: {{ ""command"": ""extrude"", ""parameters"": {{ ""distance"": ""50"" }} }}

Now parse this request: {userInput}";
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
                        new { role = "system", content = "You are a SolidWorks CAD modeling assistant." },
                        new { role = "user", content = prompt }
                    },
                    stream = false,
                    temperature = 0.3,
                    max_tokens = 500
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var jsonResponse = JObject.Parse(responseString);
                
                return jsonResponse["message"]?["content"]?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private ParsedCommand ParseResponse(string response)
        {
            try
            {
                var jsonStart = response.IndexOf('{');
                var jsonEnd = response.LastIndexOf('}') + 1;
                
                if (jsonStart >= 0 && jsonEnd > jsonStart)
                {
                    var json = response.Substring(jsonStart, jsonEnd - jsonStart);
                    var parsed = JsonConvert.DeserializeObject<LlmResponse>(json);
                    
                    if (parsed != null && !string.IsNullOrEmpty(parsed.Command))
                    {
                        return MapToParsedCommand(parsed);
                    }
                }
            }
            catch
            {
            }
            
            return null;
        }

        private ParsedCommand MapToParsedCommand(LlmResponse llmResponse)
        {
            var command = new ParsedCommand
            {
                Parameters = llmResponse.Parameters ?? new Dictionary<string, string>()
            };

            switch (llmResponse.Command.ToLower())
            {
                case "create_primitive":
                    command.Type = CommandType.CreatePrimitive;
                    break;
                case "create_sketch":
                    command.Type = CommandType.CreateSketch;
                    break;
                case "extrude":
                    command.Type = CommandType.Extrude;
                    break;
                case "revolve":
                    command.Type = CommandType.Revolve;
                    break;
                case "assemble":
                    command.Type = CommandType.Assemble;
                    break;
                case "modify":
                    command.Type = CommandType.Modify;
                    break;
                default:
                    return null;
            }

            return command;
        }

        private ParsedCommand ParseWithFallback(string userInput)
        {
            var input = userInput.ToLower();
            
            if (input.Contains("cube") || input.Contains("box"))
            {
                return new ParsedCommand
                {
                    Type = CommandType.CreatePrimitive,
                    Parameters = new Dictionary<string, string>
                    {
                        { "shape", "cube" },
                        { "size", ExtractNumber(input, 100).ToString() }
                    }
                };
            }
            else if (input.Contains("sphere"))
            {
                return new ParsedCommand
                {
                    Type = CommandType.CreatePrimitive,
                    Parameters = new Dictionary<string, string>
                    {
                        { "shape", "sphere" },
                        { "diameter", ExtractNumber(input, 100).ToString() }
                    }
                };
            }
            else if (input.Contains("cylinder"))
            {
                return new ParsedCommand
                {
                    Type = CommandType.CreatePrimitive,
                    Parameters = new Dictionary<string, string>
                    {
                        { "shape", "cylinder" },
                        { "diameter", ExtractNumber(input, 50).ToString() },
                        { "height", ExtractNumber(input, 100).ToString() }
                    }
                };
            }
            else if (input.Contains("extrude"))
            {
                return new ParsedCommand
                {
                    Type = CommandType.Extrude,
                    Parameters = new Dictionary<string, string>
                    {
                        { "distance", ExtractNumber(input, 50).ToString() }
                    }
                };
            }
            
            return null;
        }

        private double ExtractNumber(string input, double defaultValue)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(input, @"\d+");
            if (matches.Count > 0 && double.TryParse(matches[0].Value, out var result))
            {
                return result;
            }
            return defaultValue;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        private class LlmResponse
        {
            [JsonProperty("command")]
            public string Command { get; set; }
            
            [JsonProperty("parameters")]
            public Dictionary<string, string> Parameters { get; set; }
        }
    }
}
