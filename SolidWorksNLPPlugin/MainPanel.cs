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
        private Label _statusLabel;

        public MainPanel(NlpCommandHandler commandHandler)
        {
            _commandHandler = commandHandler;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(400, 600);
            this.BackColor = Color.White;
            
            var titleLabel = new Label
            {
                Text = "NLP Modeling Assistant",
                Font = new Font("Microsoft YaHei", 12F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(360, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            _outputTextBox = new RichTextBox
            {
                Location = new Point(20, 60),
                Size = new Size(360, 300),
                ReadOnly = true,
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft YaHei", 9F)
            };
            
            var inputLabel = new Label
            {
                Text = "Enter your command:",
                Location = new Point(20, 370),
                Size = new Size(360, 20)
            };
            
            _inputTextBox = new TextBox
            {
                Location = new Point(20, 390),
                Size = new Size(360, 80),
                Multiline = true,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Microsoft YaHei", 10F),
                AcceptsReturn = true,
                ScrollBars = ScrollBars.Vertical
            };
            
            _sendButton = new Button
            {
                Text = "Send",
                Location = new Point(20, 480),
                Size = new Size(175, 40),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei", 10F, FontStyle.Bold)
            };
            _sendButton.FlatAppearance.BorderSize = 0;
            _sendButton.Click += SendButton_Click;
            
            _clearButton = new Button
            {
                Text = "Clear",
                Location = new Point(205, 480),
                Size = new Size(175, 40),
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Microsoft YaHei", 10F)
            };
            _clearButton.FlatAppearance.BorderSize = 0;
            _clearButton.Click += ClearButton_Click;
            
            var examplesGroup = new GroupBox
            {
                Text = "Example Commands",
                Location = new Point(20, 530),
                Size = new Size(360, 60)
            };
            
            var examplesLabel = new Label
            {
                Text = "Create a cube 100mm | Create cylinder 50x200 | Extrude 50mm",
                Location = new Point(10, 25),
                Size = new Size(340, 25),
                ForeColor = Color.Gray,
                Font = new Font("Microsoft YaHei", 8F)
            };
            examplesGroup.Controls.Add(examplesLabel);
            
            _statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(20, 575),
                Size = new Size(360, 20),
                ForeColor = Color.Gray,
                Font = new Font("Microsoft YaHei", 8F)
            };
            
            this.Controls.Add(titleLabel);
            this.Controls.Add(_outputTextBox);
            this.Controls.Add(inputLabel);
            this.Controls.Add(_inputTextBox);
            this.Controls.Add(_sendButton);
            this.Controls.Add(_clearButton);
            this.Controls.Add(examplesGroup);
            this.Controls.Add(_statusLabel);
            
            this.ResumeLayout(false);
            
            AddWelcomeMessage();
        }

        private void AddWelcomeMessage()
        {
            AppendMessage("Welcome to SolidWorks NLP Modeling Assistant!\n\n", Color.FromArgb(0, 122, 204), true);
            AppendMessage("You can use natural language to create 3D models. Try commands like:\n", Color.Black);
            AppendMessage("- \"Create a cube 100mm on each side\"\n", Color.Gray);
            AppendMessage("- \"Create a cylinder with diameter 50mm and height 200mm\"\n", Color.Gray);
            AppendMessage("- \"Extrude the sketch by 50mm\"\n", Color.Gray);
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            var userInput = _inputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput))
            {
                return;
            }
            
            AppendMessage($"You: {userInput}\n", Color.Navy);
            
            _statusLabel.Text = "Processing...";
            _sendButton.Enabled = false;
            _inputTextBox.Clear();
            
            try
            {
                var response = await _commandHandler.ProcessCommandAsync(userInput);
                AppendMessage($"Assistant: {response}\n", Color.Green);
            }
            catch (Exception ex)
            {
                AppendMessage($"Error: {ex.Message}\n", Color.Red);
            }
            finally
            {
                _statusLabel.Text = "Ready";
                _sendButton.Enabled = true;
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            _outputTextBox.Clear();
            _inputTextBox.Clear();
            AddWelcomeMessage();
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
}
