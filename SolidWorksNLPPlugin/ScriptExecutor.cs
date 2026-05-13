using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksNLPPlugin
{
    public class ScriptExecutor
    {
        private readonly SldWorks _swApp;
        private readonly List<string> _referencedAssemblies;

        public ScriptExecutor(SldWorks swApp)
        {
            _swApp = swApp;
            _referencedAssemblies = new List<string>
            {
                "System.dll",
                "System.Core.dll",
                "System.Data.dll",
                "System.Xml.dll"
            };
        }

        public ExecutionResult ExecuteScript(string scriptCode, ModelDoc2 activeModel = null)
        {
            try
            {
                var wrappedCode = WrapScript(scriptCode);
                var compiled = CompileScript(wrappedCode);
                
                if (!compiled.Success)
                {
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = $"Compilation failed:\n{string.Join("\n", compiled.Errors)}"
                    };
                }

                return ExecuteCompiledScript(compiled.Assembly, activeModel);
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ErrorMessage = $"Execution failed: {ex.Message}"
                };
            }
        }

        private string WrapScript(string scriptCode)
        {
            return $@"
using System;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksNLPPlugin.Generated
{{
    public class ScriptRunner
    {{
        public static object Run(SldWorks swApp, ModelDoc2 modelDoc)
        {{
            {scriptCode}
            return true;
        }}
    }}
}}";
        }

        private CompilationResult CompileScript(string code)
        {
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                GenerateExecutable = false,
                TreatWarningsAsErrors = false
            };

            foreach (var assembly in _referencedAssemblies)
            {
                parameters.ReferencedAssemblies.Add(assembly);
            }

            AddSolidWorksReferences(parameters);

            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                var errors = new List<string>();
                foreach (CompilerError error in results.Errors)
                {
                    errors.Add($"Line {error.Line}: {error.ErrorText}");
                }
                
                return new CompilationResult
                {
                    Success = false,
                    Errors = errors
                };
            }

            return new CompilationResult
            {
                Success = true,
                Assembly = results.CompiledAssembly
            };
        }

        private void AddSolidWorksReferences(CompilerParameters parameters)
        {
            try
            {
                var swAssembly = typeof(SldWorks).Assembly;
                parameters.ReferencedAssemblies.Add(swAssembly.Location);

                var swconstAssembly = typeof(swCommands_e).Assembly;
                parameters.ReferencedAssemblies.Add(swconstAssembly.Location);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to add SolidWorks references: {ex.Message}");
            }
        }

        private ExecutionResult ExecuteCompiledScript(Assembly assembly, ModelDoc2 activeModel)
        {
            try
            {
                var type = assembly.GetType("SolidWorksNLPPlugin.Generated.ScriptRunner");
                if (type == null)
                {
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = "Could not find ScriptRunner type"
                    };
                }

                var method = type.GetMethod("Run", BindingFlags.Public | BindingFlags.Static);
                if (method == null)
                {
                    return new ExecutionResult
                    {
                        Success = false,
                        ErrorMessage = "Could not find Run method"
                    };
                }

                var model = activeModel ?? _swApp.ActiveDoc;
                var result = method.Invoke(null, new object[] { _swApp, model });

                return new ExecutionResult
                {
                    Success = true,
                    Result = result
                };
            }
            catch (TargetInvocationException ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ErrorMessage = $"Script error: {ex.InnerException?.Message ?? ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new ExecutionResult
                {
                    Success = false,
                    ErrorMessage = $"Execution error: {ex.Message}"
                };
            }
        }

        public void AddReference(string assemblyPath)
        {
            if (!_referencedAssemblies.Contains(assemblyPath))
            {
                _referencedAssemblies.Add(assemblyPath);
            }
        }
    }

    public class ExecutionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public object Result { get; set; }
    }

    public class CompilationResult
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public Assembly Assembly { get; set; }
    }
}
