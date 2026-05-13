using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksNLPPlugin
{
    public class NlpCommandHandler : IDisposable
    {
        private readonly SldWorks _swApp;
        private readonly LocalLlmClient _llmClient;
        private readonly ModelBuilder _modelBuilder;

        public NlpCommandHandler(SldWorks swApp)
        {
            _swApp = swApp;
            _llmClient = new LocalLlmClient();
            _modelBuilder = new ModelBuilder(swApp);
        }

        public async Task<string> ProcessCommandAsync(string userInput)
        {
            try
            {
                var parsedCommand = await _llmClient.ParseCommandAsync(userInput);
                if (parsedCommand == null)
                {
                    return "Sorry, I couldn't understand your command. Please try again.";
                }

                var result = ExecuteCommand(parsedCommand);
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string ExecuteCommand(ParsedCommand command)
        {
            switch (command.Type)
            {
                case CommandType.CreatePrimitive:
                    return ExecuteCreatePrimitive(command);
                case CommandType.CreateSketch:
                    return ExecuteCreateSketch(command);
                case CommandType.Extrude:
                    return ExecuteExtrude(command);
                case CommandType.Revolve:
                    return ExecuteRevolve(command);
                case CommandType.Assemble:
                    return ExecuteAssemble(command);
                case CommandType.Modify:
                    return ExecuteModify(command);
                default:
                    return $"Unknown command type: {command.Type}";
            }
        }

        private string ExecuteCreatePrimitive(ParsedCommand command)
        {
            var shapeType = command.Parameters.GetValueOrDefault("shape", "");
            var dimensions = command.Parameters.GetValueOrDefault("dimensions", "");

            var (success, message) = _modelBuilder.CreatePrimitive(shapeType, command.Parameters);
            return success ? $"Created {shapeType} successfully." : message;
        }

        private string ExecuteCreateSketch(ParsedCommand command)
        {
            return "Sketch creation command executed.";
        }

        private string ExecuteExtrude(ParsedCommand command)
        {
            var distance = double.TryParse(command.Parameters.GetValueOrDefault("distance", "0"), out var d) ? d : 10;
            var (success, message) = _modelBuilder.Extrude(distance);
            return success ? "Extruded successfully." : message;
        }

        private string ExecuteRevolve(ParsedCommand command)
        {
            var angle = double.TryParse(command.Parameters.GetValueOrDefault("angle", "360"), out var a) ? a : 360;
            var (success, message) = _modelBuilder.Revolve(angle);
            return success ? "Revolved successfully." : message;
        }

        private string ExecuteAssemble(ParsedCommand command)
        {
            return "Assembly command executed.";
        }

        private string ExecuteModify(ParsedCommand command)
        {
            return "Modify command executed.";
        }

        public void Dispose()
        {
            _llmClient?.Dispose();
        }
    }

    public enum CommandType
    {
        CreatePrimitive,
        CreateSketch,
        Extrude,
        Revolve,
        Assemble,
        Modify
    }

    public class ParsedCommand
    {
        public CommandType Type { get; set; }
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
