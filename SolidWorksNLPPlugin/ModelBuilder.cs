using System;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksNLPPlugin
{
    public class ModelBuilder
    {
        private readonly SldWorks _swApp;

        public ModelBuilder(SldWorks swApp)
        {
            _swApp = swApp;
        }

        public (bool Success, string Message) CreatePrimitive(string shapeType, Dictionary<string, string> parameters)
        {
            try
            {
                var partDoc = GetActiveOrCreatePart();
                if (partDoc == null)
                {
                    return (false, "Could not create or get active part document.");
                }

                double[] origin = { 0.0, 0.0, 0.0 };

                switch (shapeType.ToLower())
                {
                    case "cube":
                    case "box":
                        return CreateCube(partDoc, parameters, origin);
                    case "sphere":
                        return CreateSphere(partDoc, parameters, origin);
                    case "cylinder":
                        return CreateCylinder(partDoc, parameters, origin);
                    case "cone":
                        return CreateCone(partDoc, parameters, origin);
                    case "torus":
                        return CreateTorus(partDoc, parameters, origin);
                    case "pyramid":
                        return CreatePyramid(partDoc, parameters, origin);
                    default:
                        return (false, $"Unknown shape type: {shapeType}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error creating primitive: {ex.Message}");
            }
        }

        private (bool Success, string Message) CreateCube(PartDoc partDoc, Dictionary<string, string> parameters, double[] origin)
        {
            var size = GetParameterValue(parameters, "size", 100.0);
            
            partDoc.InsertSketch2(true);
            
            var sketchManager = partDoc.SketchManager;
            sketchManager.InsertRectangle(0, 0, 0, size, size, 0);
            
            partDoc.InsertSketch2(true);
            
            var featureManager = partDoc.FeatureManager;
            featureManager.FeatureExtrusion2(true, false, false, 0, 0, size, 0.01, 0.01, false, false, false, false, 0, 0, false, false, true, true, true, 0, 0, false);
            
            return (true, "Cube created successfully.");
        }

        private (bool Success, string Message) CreateSphere(PartDoc partDoc, Dictionary<string, string> parameters, double[] origin)
        {
            var diameter = GetParameterValue(parameters, "diameter", 100.0);
            var radius = diameter / 2.0;
            
            partDoc.InsertSketch2(true);
            
            var sketchManager = partDoc.SketchManager;
            sketchManager.CreateCircle(0, 0, 0, radius, 0, 0);
            sketchManager.CreateLine(0, 0, 0, 0, radius, 0);
            
            partDoc.InsertSketch2(true);
            
            var featureManager = partDoc.FeatureManager;
            featureManager.FeatureRevolve2(true, false, false, false, true, false, 0, 0, 360.0, 0.0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
            
            return (true, "Sphere created successfully.");
        }

        private (bool Success, string Message) CreateCylinder(PartDoc partDoc, Dictionary<string, string> parameters, double[] origin)
        {
            var diameter = GetParameterValue(parameters, "diameter", 50.0);
            var height = GetParameterValue(parameters, "height", 100.0);
            var radius = diameter / 2.0;
            
            partDoc.InsertSketch2(true);
            
            var sketchManager = partDoc.SketchManager;
            sketchManager.CreateCircle(0, 0, 0, radius, 0, 0);
            
            partDoc.InsertSketch2(true);
            
            var featureManager = partDoc.FeatureManager;
            featureManager.FeatureExtrusion2(true, false, false, 0, 0, height, 0.01, 0.01, false, false, false, false, 0, 0, false, false, true, true, true, 0, 0, false);
            
            return (true, "Cylinder created successfully.");
        }

        private (bool Success, string Message) CreateCone(PartDoc partDoc, Dictionary<string, string> parameters, double[] origin)
        {
            var diameter = GetParameterValue(parameters, "diameter", 50.0);
            var height = GetParameterValue(parameters, "height", 100.0);
            var radius = diameter / 2.0;
            
            partDoc.InsertSketch2(true);
            
            var sketchManager = partDoc.SketchManager;
            sketchManager.CreateLine(0, 0, 0, radius, 0, 0);
            sketchManager.CreateLine(radius, 0, 0, 0, height, 0);
            sketchManager.CreateLine(0, height, 0, 0, 0, 0);
            
            partDoc.InsertSketch2(true);
            
            var featureManager = partDoc.FeatureManager;
            featureManager.FeatureRevolve2(true, false, false, false, true, false, 0, 0, 360.0, 0.0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
            
            return (true, "Cone created successfully.");
        }

        private (bool Success, string Message) CreateTorus(PartDoc partDoc, Dictionary<string, string> parameters, double[] origin)
        {
            var majorDiameter = GetParameterValue(parameters, "major_diameter", 100.0);
            var minorDiameter = GetParameterValue(parameters, "minor_diameter", 20.0);
            var majorRadius = majorDiameter / 2.0;
            var minorRadius = minorDiameter / 2.0;
            
            partDoc.InsertSketch2(true);
            
            var sketchManager = partDoc.SketchManager;
            sketchManager.CreateCircle(majorRadius, 0, 0, majorRadius + minorRadius, 0, 0);
            sketchManager.CreateLine(0, 0, 0, majorRadius * 2, 0, 0);
            
            partDoc.InsertSketch2(true);
            
            var featureManager = partDoc.FeatureManager;
            featureManager.FeatureRevolve2(true, false, false, false, true, false, 0, 0, 360.0, 0.0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
            
            return (true, "Torus created successfully.");
        }

        private (bool Success, string Message) CreatePyramid(PartDoc partDoc, Dictionary<string, string> parameters, double[] origin)
        {
            var size = GetParameterValue(parameters, "size", 100.0);
            var height = GetParameterValue(parameters, "height", 100.0);
            
            partDoc.InsertSketch2(true);
            
            var sketchManager = partDoc.SketchManager;
            var halfSize = size / 2.0;
            sketchManager.CreateLine(-halfSize, -halfSize, 0, halfSize, -halfSize, 0);
            sketchManager.CreateLine(halfSize, -halfSize, 0, halfSize, halfSize, 0);
            sketchManager.CreateLine(halfSize, halfSize, 0, -halfSize, halfSize, 0);
            sketchManager.CreateLine(-halfSize, halfSize, 0, -halfSize, -halfSize, 0);
            
            partDoc.InsertSketch2(true);
            
            var featureManager = partDoc.FeatureManager;
            featureManager.FeatureExtrusion2(true, false, false, 0, 0, height, 0.01, 0.01, false, false, true, false, 0, 0, false, false, true, true, true, 0, 0, false);
            
            return (true, "Pyramid created successfully.");
        }

        public (bool Success, string Message) Extrude(double distance)
        {
            try
            {
                var partDoc = GetActivePart();
                if (partDoc == null)
                {
                    return (false, "No active part document found.");
                }

                var featureManager = partDoc.FeatureManager;
                featureManager.FeatureExtrusion2(true, false, false, 0, 0, distance, 0.01, 0.01, false, false, false, false, 0, 0, false, false, true, true, true, 0, 0, false);
                
                return (true, "Extrusion completed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error during extrusion: {ex.Message}");
            }
        }

        public (bool Success, string Message) Revolve(double angle)
        {
            try
            {
                var partDoc = GetActivePart();
                if (partDoc == null)
                {
                    return (false, "No active part document found.");
                }

                var featureManager = partDoc.FeatureManager;
                featureManager.FeatureRevolve2(true, false, false, false, true, false, 0, 0, angle, 0.0, false, false, 0.01, 0.01, 0, 0, 0, true, true, true);
                
                return (true, "Revolve completed successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Error during revolve: {ex.Message}");
            }
        }

        private PartDoc GetActiveOrCreatePart()
        {
            var modelDoc = _swApp.ActiveDoc;
            
            if (modelDoc == null)
            {
                var templatePath = _swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);
                modelDoc = _swApp.NewDocument(templatePath, 0, 0, 0);
            }
            
            return modelDoc as PartDoc;
        }

        private PartDoc GetActivePart()
        {
            return _swApp.ActiveDoc as PartDoc;
        }

        private double GetParameterValue(Dictionary<string, string> parameters, string key, double defaultValue)
        {
            if (parameters.TryGetValue(key, out var valueStr) && double.TryParse(valueStr, out var value))
            {
                return value / 1000.0; 
            }
            return defaultValue / 1000.0;
        }
    }
}
