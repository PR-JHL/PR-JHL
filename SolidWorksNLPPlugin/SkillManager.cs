using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace SolidWorksNLPPlugin
{
    public class SkillManager
    {
        private readonly string _skillsDirectory;

        public SkillManager(string skillsDirectory = null)
        {
            _skillsDirectory = skillsDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SolidWorksNLPPlugin",
                "Skills"
            );
            
            if (!Directory.Exists(_skillsDirectory))
            {
                Directory.CreateDirectory(_skillsDirectory);
            }
        }

        public List<Skill> GetAllSkills()
        {
            var skills = new List<Skill>();
            
            try
            {
                var skillFiles = Directory.GetFiles(_skillsDirectory, "*.json");
                
                foreach (var file in skillFiles)
                {
                    try
                    {
                        var json = File.ReadAllText(file);
                        var skill = JsonConvert.DeserializeObject<Skill>(json);
                        if (skill != null)
                        {
                            skill.FilePath = file;
                            skills.Add(skill);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load skill from {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to get skills: {ex.Message}");
            }
            
            return skills.OrderBy(s => s.Name).ToList();
        }

        public Skill SaveSkill(string name, string description, string script, string originalInput, Dictionary<string, object> parameters = null)
        {
            var skill = new Skill
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Description = description,
                Script = script,
                OriginalInput = originalInput,
                Parameters = parameters ?? new Dictionary<string, object>(),
                CreatedDate = DateTime.Now,
                LastUsedDate = DateTime.Now
            };
            
            var fileName = $"{SanitizeFileName(name)}_{skill.Id.Substring(0, 8)}.json";
            var filePath = Path.Combine(_skillsDirectory, fileName);
            skill.FilePath = filePath;
            
            var json = JsonConvert.SerializeObject(skill, Formatting.Indented);
            File.WriteAllText(filePath, json);
            
            return skill;
        }

        public Skill LoadSkill(string filePath)
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var skill = JsonConvert.DeserializeObject<Skill>(json);
                if (skill != null)
                {
                    skill.FilePath = filePath;
                    return skill;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load skill: {ex.Message}");
            }
            
            return null;
        }

        public bool UpdateSkill(Skill skill)
        {
            try
            {
                skill.LastUsedDate = DateTime.Now;
                var json = JsonConvert.SerializeObject(skill, Formatting.Indented);
                File.WriteAllText(skill.FilePath, json);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update skill: {ex.Message}");
                return false;
            }
        }

        public bool DeleteSkill(Skill skill)
        {
            try
            {
                if (File.Exists(skill.FilePath))
                {
                    File.Delete(skill.FilePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete skill: {ex.Message}");
            }
            
            return false;
        }

        public void RecordSkillUse(Skill skill)
        {
            skill.LastUsedDate = DateTime.Now;
            skill.UseCount++;
            UpdateSkill(skill);
        }

        private string SanitizeFileName(string name)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(name.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
            return sanitized.Length > 50 ? sanitized.Substring(0, 50) : sanitized;
        }
    }

    public class Skill
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Script { get; set; }
        public string OriginalInput { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUsedDate { get; set; }
        public int UseCount { get; set; }
        
        [JsonIgnore]
        public string FilePath { get; set; }
    }
}
