using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;

namespace SolidWorksNLPPlugin
{
    public class NlpCommandHandler : IDisposable
    {
        private readonly SldWorks _swApp;
        private CloudLlmClient _llmClient;
        private readonly SkillManager _skillManager;
        private readonly ScriptExecutor _scriptExecutor;
        private ScriptGenerationResult _lastGeneratedScript;

        public NlpCommandHandler(SldWorks swApp)
        {
            _swApp = swApp;
            _skillManager = new SkillManager();
            _scriptExecutor = new ScriptExecutor(swApp);
        }

        public void InitializeLlmClient(string apiKey, string apiUrl = "https://api.openai.com/v1/chat/completions", string modelName = "gpt-4")
        {
            _llmClient?.Dispose();
            _llmClient = new CloudLlmClient(apiKey, apiUrl, modelName);
        }

        public bool IsLlmInitialized => _llmClient != null;

        public async Task<ProcessResult> ProcessCommandAsync(string userInput)
        {
            if (_llmClient == null)
            {
                return new ProcessResult
                {
                    Success = false,
                    ErrorMessage = "LLM client not initialized. Please configure API key first."
                };
            }

            try
            {
                var scriptResult = await _llmClient.GenerateScriptAsync(userInput);
                _lastGeneratedScript = scriptResult;

                if (!scriptResult.Success)
                {
                    return new ProcessResult
                    {
                        Success = false,
                        ErrorMessage = scriptResult.ErrorMessage
                    };
                }

                return new ProcessResult
                {
                    Success = true,
                    Script = scriptResult.Script,
                    Description = scriptResult.Description,
                    OriginalInput = userInput,
                    Parameters = scriptResult.Parameters
                };
            }
            catch (Exception ex)
            {
                return new ProcessResult
                {
                    Success = false,
                    ErrorMessage = $"Error: {ex.Message}"
                };
            }
        }

        public ExecutionResult ExecuteScript(string script)
        {
            return _scriptExecutor.ExecuteScript(script);
        }

        public ExecutionResult ExecuteLastScript()
        {
            if (_lastGeneratedScript == null)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ErrorMessage = "No script has been generated yet"
                };
            }

            return ExecuteScript(_lastGeneratedScript.Script);
        }

        public Skill SaveCurrentAsSkill(string name, string description = null)
        {
            if (_lastGeneratedScript == null)
            {
                return null;
            }

            return _skillManager.SaveSkill(
                name,
                description ?? _lastGeneratedScript.Description,
                _lastGeneratedScript.Script,
                _lastGeneratedScript.OriginalInput,
                _lastGeneratedScript.Parameters
            );
        }

        public Skill SaveAsSkill(string name, string description, string script, string originalInput, Dictionary<string, object> parameters = null)
        {
            return _skillManager.SaveSkill(name, description, script, originalInput, parameters);
        }

        public List<Skill> GetAllSkills()
        {
            return _skillManager.GetAllSkills();
        }

        public ExecutionResult ExecuteSkill(Skill skill)
        {
            _skillManager.RecordSkillUse(skill);
            return _scriptExecutor.ExecuteScript(skill.Script);
        }

        public bool DeleteSkill(Skill skill)
        {
            return _skillManager.DeleteSkill(skill);
        }

        public ScriptGenerationResult LastGeneratedScript => _lastGeneratedScript;

        public void Dispose()
        {
            _llmClient?.Dispose();
        }
    }

    public class ProcessResult
    {
        public bool Success { get; set; }
        public string Script { get; set; }
        public string Description { get; set; }
        public string OriginalInput { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public string ErrorMessage { get; set; }
    }
}
