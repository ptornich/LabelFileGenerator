using System;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using Microsoft.Dynamics.Ax.Xpp;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace LabelFileGenerator
{
    public partial class MainForm : Form
    {
        #region ModelsToProcess
        public static string[] ModelsToProcess =
        {
            "AccountsPayableMobile\\AccountsPayableMobile",
            "ApplicationFoundation\\ApplicationFoundation",
            "ApplicationPlatform\\ApplicationPlatform",
            "ApplicationSuite\\Electronic Reporting Application Suite Integration",
            "ApplicationSuite\\Foundation Upgrade",
            "ApplicationSuite\\SCMControls",
            "ApplicationSuite\\Tax Books Application Suite Integration",
            "ApplicationSuite\\Tax Engine Application Suite Integration",
            "ApplicationWorkspaces\\ApplicationWorkspaces",
            "CostAccountingAX\\CostAccountingAX",
            "CustomFields\\CustomFields",
            "DataExpansionTool\\DataExpansionTool",
            "DataImpExpApplication\\DataImpExpApplication",
            "DataSharing\\DataSharing",
            "DataUpgrade\\DataUpgrade",
            "DataUpgradePlatform\\Data Upgrade Platform",
            "demodatasuite\\DemoDataSuite",
            "DOM\\DOM",
            "ExpenseMobile\\ExpenseMobile",
            "FinancialReporting\\FinancialReporting",
            "FinancialReportingEntityStore\\FinancialReportingEntityStore",
            "FleetManagement\\FleetManagement",
            "FleetManagementExtension\\FleetManagementExtension",
            "GeneralLedger\\GeneralLedger",
            "InventoryDimensionConversion\\InventoryDimensionConversion",
            "PaymentPredictor\\PaymentPredictor",
            "Project\\Project",
            "ProjectMobile\\ProjectMobile",
            "SCMMobile\\SCMMobile",
            "SelfHealing\\SelfHealing",
            "SelfHealingRules\\SelfHealingRules",
            "SystemHealth\\SystemHealth",
            "TestEssentials\\TestEssentials",
            "WMSAdvancedMigration\\WMSAdvancedMigration"
        };
        #endregion

        public static string AOSServicePath = @"AOSService\";
        public static string PackagesLocalDirectoryPath = @"PackagesLocalDirectory\";
        public static string FoundationModelPath = PackagesLocalDirectoryPath + @"ApplicationSuite\Foundation\";
        public static string AxLabelFilePath = @"AxLabelFile\";
        public static string LabelResourcesPath = AxLabelFilePath + @"LabelResources\";
        public static string enUSXmlSuffix = @"_en-US.xml";

        public DateTime processStart;
        public DateTime processEnd;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log("NOTE: Label descriptions cannot be recovered by code, that's why they won't be loaded on the newly created files.");
            LogNewLine();

            LoadServiceFolders();
        }

        private void GenerateLabelFilesButton_Click(object sender, EventArgs e)
        {
            string selectedServiceFolder = aosServiceFolderComboBox.SelectedItem as string;
            string selectedLanguage = languageComboBox.SelectedItem as string;

            if (string.IsNullOrEmpty(selectedServiceFolder) || string.IsNullOrEmpty(selectedLanguage))
            {
                Log("Failed!");
                Log("Please select the AOS service folder and the target language.");
                LogNewLine();
            }
            else
            {
                Thread th = new Thread(() => GenerateLabels(selectedServiceFolder, selectedLanguage, progressLabel, progressBar));
                th.Start();
            }
        }
        
        private void GenerateLabels(
            string selectedServiceFolder, string selectedLanguage,
            Label progressLabel, ProgressBar progressBar)
        {
            GenerateLabelFilesButton.Invoke((MethodInvoker)delegate { GenerateLabelFilesButton.Enabled = false; });
            aosServiceFolderComboBox.Invoke((MethodInvoker)delegate { aosServiceFolderComboBox.Enabled = false; });
            languageComboBox.Invoke((MethodInvoker)delegate { languageComboBox.Enabled = false; });

            processStart = DateTime.Now;
            Log("Begin label generation at " + processStart.ToString());
            LogNewLine();

            int progressBarMax = ModelsToProcess.Count();
            int progressBarValue = 0;

            progressLabel.Invoke((MethodInvoker)delegate
            {
                progressLabel.Visible = true;
                progressLabel.Refresh();
            });

            progressBar.Invoke((MethodInvoker)delegate
            {
                progressBar.Maximum = progressBarMax;
                progressBar.Value = progressBarValue;
            });
            
            foreach (string modelPath in ModelsToProcess)
            {
                progressBarValue++;
                int percent = (int)(((double)progressBarValue / (double)progressBarMax) * 100);

                progressBar.Invoke((MethodInvoker)delegate
                {
                    progressBar.Value = progressBarValue;
                    progressBar.Refresh();
                    progressBar.CreateGraphics().DrawString(percent.ToString() + "%",
                       new Font("Arial", (float)8.25, FontStyle.Regular),
                       Brushes.Black,
                       new PointF(292, 2));
                });

                string axLabelFilePath =
                    selectedServiceFolder + PackagesLocalDirectoryPath + modelPath
                    + "\\" + AxLabelFilePath;

                string labelLanguagePath =
                    selectedServiceFolder + PackagesLocalDirectoryPath + modelPath
                    + "\\" + LabelResourcesPath + selectedLanguage;

                if(!CreateDirectory(labelLanguagePath))
                {
                    return;
                }
                
                var fileNames = Directory.GetFiles(axLabelFilePath, "*" + enUSXmlSuffix);

                if (fileNames != null && fileNames.Any())
                {
                    Log("Model: " + modelPath.Split(Path.DirectorySeparatorChar).Last());

                    foreach (var fileName in fileNames)
                    {
                        var suffix = "_" + fileName.Split('_').Last();
                        var labelFileName = fileName.Split(Path.DirectorySeparatorChar).Last().Replace(suffix, "");

                        if(!CreateXML(axLabelFilePath, modelPath, labelFileName, selectedLanguage))
                        {
                            return;
                        }

                        if(!CreateTXT(labelLanguagePath, labelFileName, selectedLanguage))
                        {
                            return;
                        }
                    }

                    LogNewLine();
                }
            }

            processEnd = DateTime.Now;
            Log("End label generation at " + processEnd.ToString());
            LogNewLine();
            Log("Duration: " + (processEnd - processStart).ToString());

            GenerateLabelFilesButton.Invoke((MethodInvoker)delegate { GenerateLabelFilesButton.Enabled = true; });
            aosServiceFolderComboBox.Invoke((MethodInvoker)delegate { aosServiceFolderComboBox.Enabled = true; });
            languageComboBox.Invoke((MethodInvoker)delegate { languageComboBox.Enabled = true; });
        }
        
        private bool CreateDirectory(string path)
        {
            bool ret = true;

            try
            {
                if (Directory.Exists(path))
                {
                    var files = Directory.GetFiles(path);

                    foreach (var filePath in files)
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                        File.Delete(filePath);
                    }

                    Directory.Delete(path, true);
                }

                Directory.CreateDirectory(path);
            }
            catch (UnauthorizedAccessException e)
            {
                LogNewLine();
                Log("Failed!");
                Log(e.Message);
                LogNewLine();
                Log("Try closing Visual Studio or running this as Administrator.");
                LogNewLine();
                ret = false;
            }
            catch (Exception e)
            {
                LogNewLine();
                Log("Failed!");
                Log(e.Message);
                LogNewLine();
                ret = false;
            }

            return ret;
        }

        private bool CreateXML(string axLabelFilePath, string modelPath, string labelFileName, string language)
        {
            string xmlFileName = string.Format("{0}_{1}.xml", labelFileName, language);
            string xmlContent = "";

            xmlContent += "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine;
            xmlContent += "<AxLabelFile xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" + Environment.NewLine;
            xmlContent += "        <Name>{0}_{1}</Name>" + Environment.NewLine;
            xmlContent += "        <LabelContentFileName>{0}.{1}.label.txt</LabelContentFileName>" + Environment.NewLine;
            xmlContent += "        <LabelFileId>{0}</LabelFileId>" + Environment.NewLine;
            xmlContent += "        <Language>{1}</Language>" + Environment.NewLine;
            xmlContent += "        <RelativeUriInModelStore>{2}\\{0}.{1}.label.txt</RelativeUriInModelStore>" + Environment.NewLine;
            xmlContent += "</AxLabelFile>";

            xmlContent = string.Format(xmlContent, labelFileName, language, modelPath + "\\" + LabelResourcesPath + language);

            return CreateTextFile(axLabelFilePath + xmlFileName, xmlContent);
        }

        private bool CreateTXT(string labelLanguagePath, string labelFileName, string language)
        {
            string txtFileName = string.Format("{0}.{1}.label.txt", labelFileName, language);
            string txtFileContent = "";

            CultureInfo cultureInfo = new CultureInfo(language, true);

            var labels = LabelHelper.GetAllLabels(labelFileName, cultureInfo);

            if (labels != null && labels.Any())
            {
                labelFileProgressBar.Invoke((MethodInvoker)delegate
                {
                    labelFileProgressBar.Value = 0;
                    labelFileProgressBar.Maximum = labels.Count();
                });
                
                foreach (KeyValuePair<string, string> entry in labels)
                {
                    labelFileProgressBar.Invoke((MethodInvoker)delegate
                    {
                        labelFileProgressBar.Value++;
                        labelFileProgressBar.Refresh();

                        string progressTxt = labelFileProgressBar.Value == labelFileProgressBar.Maximum ? "" : labelFileName + ":" + entry.Key;

                        labelFileProgressBar.CreateGraphics().DrawString(
                            progressTxt,
                            new Font("Arial", (float)8.25, FontStyle.Regular),
                            Brushes.Black,
                            new PointF(10, 2));
                    });
                    
                    txtFileContent += entry.Key + "=" + entry.Value + Environment.NewLine;
                }
            }

            return CreateTextFile(labelLanguagePath + "\\" + txtFileName, txtFileContent);
        }

        private bool CreateTextFile(string path, string content)
        {
            bool ret = true;

            try
            {
                if (File.Exists(path))
                {
                    File.SetAttributes(path, FileAttributes.Normal);
                    File.Delete(path);
                }

                using (var file = File.CreateText(path))
                {
                    file.Write(content);
                }

                Log("File created: " + path.Split(Path.DirectorySeparatorChar).Last());
            }
            catch (UnauthorizedAccessException e)
            {
                LogNewLine();
                Log("Failed!");
                Log(e.Message);
                LogNewLine();
                Log("Try closing Visual Studio or running this as Administrator.");
                LogNewLine();
                ret = false;
            }
            catch (Exception e)
            {
                LogNewLine();
                Log("Failed!");
                Log(e.Message);
                LogNewLine();
                ret = false;
            }

            return ret;
        }

        private void LoadServiceFolders()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach(DriveInfo drive in drives)
            {
                if(drive.DriveType == DriveType.Fixed
                    && Directory.Exists(drive.Name + AOSServicePath + FoundationModelPath + LabelResourcesPath))
                {
                    aosServiceFolderComboBox.Items.Add(drive.Name + AOSServicePath);
                }
            }

            if(aosServiceFolderComboBox.Items.Count > 0)
            {
                aosServiceFolderComboBox.SelectedItem = aosServiceFolderComboBox.Items[0];
            }
        }

        private void LoadLanguages(string aosServiceFolderPath)
        {
            var foundationLabelResourcesFolder = aosServiceFolderPath + FoundationModelPath + LabelResourcesPath;
            var folderPaths = Directory.EnumerateDirectories(foundationLabelResourcesFolder);
            
            foreach (string folderPath in folderPaths)
            {
                string folderName = folderPath.Split(Path.DirectorySeparatorChar).Last();

                if (folderName.ToLower() != "en-us")
                {
                    languageComboBox.Items.Add(folderName);
                }
            }
        }

        private void Log(string txt)
        {
            logTextBox.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText(txt);
            });
            
            LogNewLine();
        }

        private void LogNewLine()
        {
            logTextBox.Invoke((MethodInvoker)delegate
            {
                logTextBox.AppendText(Environment.NewLine);
            });
        }

        private void aosServiceFolderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedServiceFolder = aosServiceFolderComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedServiceFolder))
            {
                LoadLanguages(selectedServiceFolder);
            }
        }
    }
}
