using System;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace SolidWorksNLPPlugin
{
    [ComVisible(true)]
    [Guid("B1C2D3E4-F5A6-4B7C-8D9E-0F1A2B3C4D5E")]
    public class Plugin : ISwAddin
    {
        private SldWorks _swApp;
        private int _addinId;
        private NlpCommandHandler _commandHandler;
        private MainPanel _mainPanel;

        public bool ConnectToSW(object thisSW, int cookie)
        {
            try
            {
                _swApp = (SldWorks)thisSW;
                _addinId = cookie;

                InitializeServices();
                RegisterUserInterface();

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to connect to SolidWorks: {ex.Message}");
                return false;
            }
        }

        public bool DisconnectFromSW()
        {
            try
            {
                UnregisterUserInterface();
                CleanupServices();
                Marshal.ReleaseComObject(_swApp);
                _swApp = null;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void InitializeServices()
        {
            _commandHandler = new NlpCommandHandler(_swApp);
        }

        private void CleanupServices()
        {
            _commandHandler?.Dispose();
            _mainPanel?.Dispose();
        }

        private void RegisterUserInterface()
        {
            CreateTaskPane();
            AddMenuItems();
        }

        private void UnregisterUserInterface()
        {
            _swApp.RemoveTaskpane2(_addinId);
        }

        private void CreateTaskPane()
        {
            try
            {
                _mainPanel = new MainPanel(_commandHandler);
                var taskPaneView = _swApp.CreateTaskpaneView2("", "NLP Modeling");
                
                if (taskPaneView != null)
                {
                    taskPaneView.DisplayWindowFromHandlex64(_mainPanel.Handle.ToInt64());
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to create task pane: {ex.Message}");
            }
        }

        private void AddMenuItems()
        {
            try
            {
                var cmdMgr = _swApp.GetCommandManager(_addinId);
                if (cmdMgr == null) return;

                var cmdGroup = cmdMgr.AddCommandGroup2(0, "NLP Modeling", "", "", -1, -1);
                if (cmdGroup == null) return;

                var cmdIndex = 0;
                cmdGroup.AddCommandItem2(
                    "Show NLP Panel", 
                    -1, 
                    -1, 
                    -1, 
                    0, 
                    nameof(ShowNlpPanel), 
                    "", 
                    "Show Natural Language Modeling Panel", 
                    0, 
                    cmdIndex++);

                cmdGroup.Activate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to add menu items: {ex.Message}");
            }
        }

        private void ShowNlpPanel()
        {
            _mainPanel?.Show();
        }
    }
}
