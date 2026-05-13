using System;
using System.Drawing;
using System.Windows.Forms;

namespace SolidWorksNLPPlugin
{
    public partial class MainPanel : UserControl
    {
        private readonly NlpCommandHandler _commandHandler;
        private TextBox _inputTextBox;
        private Button _sendButton;
        private RichTextBox _outputTextBox;
        private Button _clearButton;
        private Button _executeButton;
        private Button _saveSkillButton;
        private TabControl _tabControl;
        private TabPage _chatTab;
        private TabPage _skillsTab;
        private ListBox _skillsListBox;
        private TextBox _apiKeyTextBox;
        private TextBox _apiUrlTextBox;
        private TextBox _modelNameTextBox;
        private Button _configureButton;
        private Label _statusLabel;
        private string _currentScript;

        public MainPanel(NlpCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Size = new Size(450, 650);
            this.BackColor = Color.White;

            _tabControl = new TabControl
            {
                Location = new Point(10, 10),
                Size = new Size(430, 630)
            };

            CreateChatTab();
            CreateSkillsTab();

            _tabControl.TabPages.Add(_chatTab);
            _tabControl.TabPages.Add(_skillsTab);

            this.Controls.Add(_tabControl);
            this.ResumeLayout(false);

            AddWelcomeMessage();
        }

        private void CreateChatTab()
        {
            _chatTab = new TabPage("AI Assistant");
            _chatTab.BackColor = Color.White;

            var configGroup = new GroupBox
            {
                Text = "API Configuration",
                Location = new Point(10, 10),
                Size = new Size(390, 110)
            };

            var apiKeyLabel = new Label
            {
                Text = "API Key:",
                Location = new Point(10, 25),
                Size = new Size(60, 20)
            };

            _apiKeyTextBox = new TextBox
            {
                Location = new Point(75, 22),
                Size = new Size(300, 20),
                PasswordChar = '*'
            };

            var apiUrlLabel = new Label
            {
                Text = "API URL:",
                Location = new Point(10, 50),
                Size = new Size(60, 20)
            };

            _apiUrlTextBox = new TextBox
            {
                Location = new Point(75, 47),
                Size = new Size(300, 20),
                Text = "https://api.openai.com/v1/chat/completions"
            };

            var modelLabel = new Label
            {
                Text = "Model:",
                Location = new Point(10, 75),
                Size = new Size(60, 20)
            };

            _modelNameTextBox = new TextBox
            {
                Location = new Point(75, 72),
                Size = new Size(300, 20),
                Text = "gpt-4"
            };

            _configureButton = new Button
            {
                Text = "Connect",
                Location = new Point(310, 80),
                Size = new Size(65, 23),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _configureButton.Click += ConfigureButton_Click;

            configGroup.Controls.Add(apiKeyLabel);
            configGroup.Controls.Add(_apiKeyTextBox);
            configGroup.Controls.Add(apiUrlLabel);
            configGroup.Controls.Add(_apiUrlTextBox);
            configGroup.Controls.Add(modelLabel);
            configGroup.Controls.Add(_modelNameTextBox);
            configGroup.Controls.Add(_configureButton);

            _outputTextBox = new RichTextBox
            {
                Location = new Point(10, 130),
                Size = new Size(390, 200),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft YaHei", 9F)
            };

            var scriptGroup = new GroupBox
            {
                Text = "Generated Script",
                Location = new Point(10, 340),
                Size = new Size(390, 130)
            };

            var scriptTextBox = new RichTextBox
            {
                Location = new Point(10, 20),
                Size = new Size(370, 80),
                ReadOnly = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 8F)
            };

            _executeButton = new Button
            {
                Text = "Execute Script",
                Location = new Point(10, 102),
                Size = new Size(120, 23),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _executeButton.Click += ExecuteButton_Click;

            _saveSkillButton = new Button
            {
                Text = "Save as Skill",
                Location = new Point(140, 102),
                Size = new Size(120, 23),
                BackColor = Color.FromArgb(102, 153, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _saveSkillButton.Click += SaveSkillButton_Click;

            scriptGroup.Controls.Add(scriptTextBox);
            scriptGroup.Controls.Add(_executeButton);
            scriptGroup.Controls.Add(_saveSkillButton);

            var inputLabel = new Label
            {
                Text = "Describe what you want to do:",
                Location = new Point(10, 480),
                Size = new Size(390, 20)
            };

            _inputTextBox = new TextBox
            {
                Location = new Point(10, 500),
                Size = new Size(390, 60),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft YaHei", 10F),
                AcceptsReturn = true,
                ScrollBars = ScrollBars.Vertical
            };

            _sendButton = new Button
            {
                Text = "Generate Script",
                Location = new Point(10, 570),
                Size = new Size(190, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold)
            };
            _sendButton.Click += SendButton_Click;

            _clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(210, 570),
                Size = new Size(190, 30),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei", 10F)
            };
            _clearButton.Click += ClearButton_Click;

            _statusLabel = new Label
            {
                Text = "Ready - Please configure API key first",
                Location = new Point(10, 605),
                Size = new Size(390, 20),
                ForeColor = Color.Gray,
                Font = new Font("Microsoft YaHei", 8F)
            };

            _chatTab.Controls.Add(configGroup);
            _chatTab.Controls.Add(_outputTextBox);
            _chatTab.Controls.Add(scriptGroup);
            _chatTab.Controls.Add(inputLabel);
            _chatTab.Controls.Add(_inputTextBox);
            _chatTab.Controls.Add(_sendButton);
            _chatTab.Controls.Add(_clearButton);
            _chatTab.Controls.Add(_statusLabel);
        }

        private void CreateSkillsTab()
        {
            _skillsTab = new TabPage("Skills");
            _skillsTab.BackColor = Color.White;

            var skillsLabel = new Label
            {
                Text = "Saved Skills:",
                Location = new Point(10, 10),
                Size = new Size(390, 20)
            };

            _skillsListBox = new ListBox
            {
                Location = new Point(10, 35),
                Size = new Size(390, 450),
                BorderStyle = BorderStyle.FixedSingle
            };
            _skillsListBox.DoubleClick += SkillsListBox_DoubleClick;

            var useSkillButton = new Button
            {
                Text = "Use Selected Skill",
                Location = new Point(10, 495),
                Size = new Size(190, 30),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            useSkillButton.Click += UseSkillButton_Click;

            var deleteSkillButton = new Button
            {
                Text = "Delete Selected",
                Location = new Point(210, 495),
                Size = new Size(190, 30),
                BackColor = Color.FromArgb(204, 51, 51),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deleteSkillButton.Click += DeleteSkillButton_Click;

            var refreshButton = new Button
            {
                Text = "Refresh",
                Location = new Point(10, 535),
                Size = new Size(390, 30),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            refreshButton.Click += RefreshButton_Click;

            _skillsTab.Controls.Add(skillsLabel);
            _skillsTab.Controls.Add(_skillsListBox);
            _skillsTab.Controls.Add(useSkillButton);
            _skillsTab.Controls.Add(deleteSkillButton);
            _skillsTab.Controls.Add(refreshButton);
        }

        private void ConfigureButton_Click(object sender, EventArgs e)
        {
            var apiKey = _apiKeyTextBox.Text.Trim();
            var apiUrl = _apiUrlTextBox.Text.Trim();
            var modelName = _modelNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(apiKey))
            {
                MessageBox.Show("Please enter an API key", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _commandHandler.InitializeLlmClient(apiKey, apiUrl, modelName);
            _statusLabel.Text = "Connected - Ready";
            _statusLabel.ForeColor = Color.Green;
            AppendMessage("API configuration saved. Ready to generate scripts.\n", Color.Green);
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var userInput = _inputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }

            if (!_commandHandler.IsLlmInitialized)
            {
                MessageBox.Show("Please configure API key first", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            AppendMessage($"You: {userInput}\n", Color.Navy);
            _statusLabel.Text = "Generating script...";
            _sendButton.Enabled = false;
            _inputTextBox.Clear();

            try
            {
                var result = await _commandHandler.ProcessCommandAsync(userInput);

                if (result.Success)
                {
                    _currentScript = result.Script;
                    AppendMessage($"Assistant: {result.Description}\n", Color.Green);
                    AppendMessage($"Script generated successfully!\n", Color.Green);

                    var scriptGroup = _chatTab.Controls[3] as GroupBox;
                    if (scriptGroup != null)
                    {
                        var scriptTextBox = scriptGroup.Controls[0] as RichTextBox;
                        scriptTextBox.Text = result.Script;
                    }

                    _executeButton.Enabled = true;
                    _saveSkillButton.Enabled = true;
                    _statusLabel.Text = "Script generated - Ready to execute";
                }
                else
                {
                    AppendMessage($"Error: {result.ErrorMessage}\n", Color.Red);
                    _statusLabel.Text = "Error - " + result.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                AppendMessage($"Error: {ex.Message}\n", Color.Red);
                _statusLabel.Text = "Error - " + ex.Message;
            }
            finally
            {
                _sendButton.Enabled = true;
            }
        }

        private void ExecuteButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentScript))
            {
                MessageBox.Show("No script to execute", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _statusLabel.Text = "Executing script...";
            var result = _commandHandler.ExecuteLastScript();

            if (result.Success)
            {
                AppendMessage("Script executed successfully!\n", Color.Green);
                _statusLabel.Text = "Execution complete";
            }
            else
            {
                AppendMessage($"Execution failed: {result.ErrorMessage}\n", Color.Red);
                _statusLabel.Text = "Execution failed";
            }
        }

        private void SaveSkillButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveSkillDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var skill = _commandHandler.SaveCurrentAsSkill(dialog.SkillName, dialog.SkillDescription);
                    if (skill != null)
                    {
                        AppendMessage($"Skill '{skill.Name}' saved successfully!\n", Color.Green);
                        _statusLabel.Text = "Skill saved";
                    }
                    else
                    {
                        MessageBox.Show("Failed to save skill", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SkillsListBox_DoubleClick(object sender, EventArgs e)
        {
            UseSelectedSkill();
        }

        private void UseSkillButton_Click(object sender, EventArgs e)
        {
            UseSelectedSkill();
        }

        private void UseSelectedSkill()
        {
            if (_skillsListBox.SelectedItem is Skill skill)
            {
                _tabControl.SelectedTab = _chatTab;
                _currentScript = skill.Script;
                AppendMessage($"Loaded skill: {skill.Name}\n", Color.Navy);
                AppendMessage($"Description: {skill.Description}\n", Color.Navy);

                var scriptGroup = _chatTab.Controls[3] as GroupBox;
                if (scriptGroup != null)
                {
                    var scriptTextBox = scriptGroup.Controls[0] as RichTextBox;
                    scriptTextBox.Text = skill.Script;
                }

                _executeButton.Enabled = true;
                _saveSkillButton.Enabled = false;
            }
        }

        private void DeleteSkillButton_Click(object sender, EventArgs e)
        {
            if (_skillsListBox.SelectedItem is Skill skill)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete '{skill.Name}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _commandHandler.DeleteSkill(skill);
                    RefreshSkillsList();
                    AppendMessage($"Skill '{skill.Name}' deleted.\n", Color.Gray);
                }
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshSkillsList();
        }

        private void RefreshSkillsList()
        {
            _skillsListBox.Items.Clear();
            var skills = _commandHandler.GetAllSkills();
            foreach (var skill in skills)
            {
                _skillsListBox.Items.Add(skill);
            }
            _skillsListBox.DisplayMember = "Name";
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            _outputTextBox.Clear();
            _inputTextBox.Clear();
            _currentScript = null;
            _executeButton.Enabled = false;
            _saveSkillButton.Enabled = false;

            var scriptGroup = _chatTab.Controls[3] as GroupBox;
            if (scriptGroup != null)
            {
                var scriptTextBox = scriptGroup.Controls[0] as RichTextBox;
                scriptTextBox.Clear();
            }

            AddWelcomeMessage();
        }

        private void AddWelcomeMessage()
        {
            AppendMessage("Welcome to SolidWorks AI Script Assistant!\n\n", Color.FromArgb(0, 122, 204), true);
            AppendMessage("1. Configure your API key first\n", Color.Black);
            AppendMessage("2. Describe what you want to do in SolidWorks\n", Color.Black);
            AppendMessage("3. The AI will generate a C# script for you\n", Color.Black);
            AppendMessage("4. Execute the script or save it as a reusable skill\n\n", Color.Black);
        }

        private void AppendMessage(string message, Color color, bool bold = false)
        {
            _outputTextBox.SelectionStart = _outputTextBox.TextLength;
            _outputTextBox.SelectionLength = 0;
            _outputTextBox.SelectionColor = color;
            _outputTextBox.SelectionFont = new Font(_outputTextBox.Font, bold ? FontStyle.Bold : FontStyle.Regular);
            _outputTextBox.AppendText(message);
            _outputTextBox.SelectionColor = _outputTextBox.ForeColor;
            _outputTextBox.ScrollToCaret();
        }
    }

    public class SaveSkillDialog : Form
    {
        public string SkillName { get; private set; }
        public string SkillDescription { get; private set; }

        private TextBox _nameTextBox;
        private TextBox _descTextBox;

        public SaveSkillDialog()
        {
            this.Text = "Save as Skill";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var nameLabel = new Label
            {
                Text = "Skill Name:",
                Location = new Point(20, 20),
                Size = new Size(100, 20)
            };

            _nameTextBox = new TextBox
            {
                Location = new Point(120, 17),
                Size = new Size(250, 20)
            };

            var descLabel = new Label
            {
                Text = "Description:",
                Location = new Point(20, 50),
                Size = new Size(100, 20)
            };

            _descTextBox = new TextBox
            {
                Location = new Point(120, 47),
                Size = new Size(250, 60),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            var okButton = new Button
            {
                Text = "Save",
                Location = new Point(210, 120),
                Size = new Size(75, 30),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            okButton.Click += (s, e) =>
            {
                SkillName = _nameTextBox.Text.Trim();
                SkillDescription = _descTextBox.Text.Trim();
                if (string.IsNullOrEmpty(SkillName))
                {
                    MessageBox.Show("Please enter a skill name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(295, 120),
                Size = new Size(75, 30),
                DialogResult = DialogResult.Cancel
            };

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;

            this.Controls.Add(nameLabel);
            this.Controls.Add(_nameTextBox);
            this.Controls.Add(descLabel);
            this.Controls.Add(_descTextBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }
    }
}
