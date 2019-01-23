using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Dynamics.Ax.Xpp;
using System.Globalization;
using System.Reflection;

namespace LabelFileGenerator
{
    class LabelFile
    {
        public string ModelPath;
        public string FilePath;
        public string LanguagePath;
        public string Language;
        public string Name;
        public IDictionary<string, string> Labels;
        public List<LabelFileSegment> Segments;
        public bool Verbose;

        public LabelFile()
        {
        }

        public LabelFile(string modelPath, string filePath, string languagePath, string language, string name, bool verbose = false)
        {
            ModelPath = modelPath;
            FilePath = filePath;
            LanguagePath = languagePath;
            Language = language;
            Name = name;
            Verbose = verbose;

            Labels = LabelHelper.GetAllLabels(Name, new CultureInfo(language, true));
            
            if (Labels != null && Labels.Count() > LabelFileSegment.SegmentSize)
            {
                CreateSegments();
            }
        }

        public void CreateSegments()
        {
            Segments = new List<LabelFileSegment>();

            int numberOfSegments = (int)Math.Ceiling((double)Labels.Count() / LabelFileSegment.SegmentSize);

            for(int i = 1; i <= numberOfSegments; i++)
            {
                Segments.Add(new LabelFileSegment(this, i));
            }
        }

        public void CreateFiles()
        {
            try
            {
                if (!Directory.Exists(LanguagePath))
                {
                    Directory.CreateDirectory(LanguagePath);
                }

                CreateXML();

                CreateTXT();
            }
            catch (Exception e)
            {
                Program.LogError(e);
            }
        }

        private void CreateXML()
        {
            string xmlFileName = string.Format("{0}_{1}.xml", Name, Language);
            string xmlContent = "";

            xmlContent += "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine;
            xmlContent += "<AxLabelFile xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">" + Environment.NewLine;
            xmlContent += "        <Name>{0}_{1}</Name>" + Environment.NewLine;
            xmlContent += "        <LabelContentFileName>{0}.{1}.label.txt</LabelContentFileName>" + Environment.NewLine;
            xmlContent += "        <LabelFileId>{0}</LabelFileId>" + Environment.NewLine;
            xmlContent += "        <Language>{1}</Language>" + Environment.NewLine;
            xmlContent += "        <RelativeUriInModelStore>{2}\\{0}.{1}.label.txt</RelativeUriInModelStore>" + Environment.NewLine;
            xmlContent += "</AxLabelFile>";

            xmlContent = string.Format(xmlContent, Name, Language, ModelPath + "\\" + Program.LabelResourcesPath + Language);

            CreateFile(FilePath + xmlFileName, xmlContent);
        }

        private void CreateTXT()
        {
            var txtFileName = string.Format("{0}.{1}.label.txt", Name, Language);
            var txtFileContent = "";

            if (Labels != null && Labels.Any())
            {
                foreach (KeyValuePair<string, string> label in Labels)
                {
                    txtFileContent += label.Key + "=" + label.Value + Environment.NewLine;
                }
            }

            CreateFile(LanguagePath + "\\" + txtFileName, txtFileContent);
        }

        protected virtual void CreateFile(string path, string content)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }

            using (var file = File.CreateText(path))
            {
                file.Write(content);

                if(Verbose)
                {
                    Console.WriteLine($"File created: {Path.GetFileName(path)}");
                }
            }
        }
    }

    class LabelFileInfo
    {
        public string XmlPath;
        public string XmlContent;
        public string TxtPath;
        public string TxtContent;

        public void CreateFiles(bool verbose = false)
        {
            CreateFile(XmlPath, XmlContent, verbose);
            CreateFile(TxtPath, TxtContent, verbose);
        }

        protected void CreateFile(string path, string content, bool verbose)
        {
            if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
                File.Delete(path);
            }

            using (var file = File.CreateText(path))
            {
                file.Write(content);

                if (verbose)
                {
                    Console.WriteLine($"File created: {Path.GetFileName(path)}");
                }
            }
        }
    }

    class LabelFileSegment : LabelFile
    {
        public static int SegmentSize = 900; // The best performance was verified with 900 labels in each Thread
        public int Sequence = 0;
        public LabelFileInfo Info = new LabelFileInfo();

        public LabelFileSegment(LabelFile labelFile, int sequence)
        {
            ModelPath = labelFile.ModelPath;
            FilePath = labelFile.FilePath;
            LanguagePath = labelFile.LanguagePath;
            Language = labelFile.Language;
            Name = labelFile.Name;
            Verbose = labelFile.Verbose;

            Sequence = sequence;
            int skipCount = 0;

            if(Sequence > 1)
            {
                skipCount = ((Sequence - 1) * SegmentSize);
            }

            var labelList = labelFile.Labels.Skip(skipCount).Take(SegmentSize);

            Labels = new Dictionary<string, string>();

            foreach (var label in labelList)
            {
                Labels.Add(label);
            }
        }

        protected override void CreateFile(string path, string content)
        {
            if (Path.GetExtension(path).ToLower() == ".xml")
            {
                Info.XmlPath = path;
                Info.XmlContent = content;
            }
            else
            {
                Info.TxtPath = path;
                Info.TxtContent = content;
            }
        }
        
        public static LabelFileInfo Merge(List<LabelFileSegment> segments)
        {
            LabelFileInfo info = new LabelFileInfo();

            foreach(var seg in segments)
            {
                info.XmlPath = seg.Info.XmlPath;
                info.XmlContent = seg.Info.XmlContent;
                info.TxtPath = seg.Info.TxtPath;

                info.TxtContent += seg.Info.TxtContent;
            }

            return info;
        }
    }

    class Options
    {
        [Option('l', "lang", Required = false, HelpText = "The target laguage in which the label files should be generated. For example: pt-BR.")]
        public string Language { get; set; }

        [Option('f', "folder", Required = false, HelpText = "The AOSService folder path. For example: K:\\AosService\\. (if not specified, first found will be used)")]
        public string AOSServiceFolder { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Display the created file names during the process.")]
        public bool Verbose { get; set; }
    }
    
    class Program
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

        public Options Arguments;
        public List<string> AvailableLanguages = new List<string>();
        public List<string> AvailableAOSServiceFolders = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine();

            Program program = new Program();

            var parser = new Parser(config => config.HelpWriter = Console.Out);

            parser.ParseArguments<Options>(args).WithParsed(o => { program.Arguments = o; });

            if (program.Arguments != null)
            {
                var assembly = Assembly.GetExecutingAssembly();

                var assemblyTitle = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute;
                var assemblyVersion = assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0] as AssemblyFileVersionAttribute;
                var assemblyCopyright = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0] as AssemblyCopyrightAttribute;

                Console.WriteLine($"{assemblyTitle.Title} {assemblyVersion.Version}");
                Console.WriteLine(assemblyCopyright.Copyright);
                
                program.Run();
            }

            Console.WriteLine();
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }

        public static void LogError(Exception e)
        {
            string msgDelimiter = "----------";
            string emptyLine = Environment.NewLine + Environment.NewLine;

            string msg = "Execution failed!" + emptyLine;

            if (e is UnauthorizedAccessException)
            {
                msg += e.Message
                    + emptyLine
                    + "Try to run this as Administrator."
                    + Environment.NewLine
                    + "You may also try to close Visual Studio or to restart the computer.";
            }
            else
            {
                msg += e.ToString();
            }

            msg = msgDelimiter + Environment.NewLine + msg + Environment.NewLine + msgDelimiter + Environment.NewLine;

            Console.WriteLine(msg);
        }

        string GetLanguageFromUserInput()
        {
            string lang = "";
            string fileName = Process.GetCurrentProcess().ProcessName;

            if (fileName.Contains("_"))
            {
                lang = formatLanguageInput(fileName.Split('_').Last());
                if (!ValidateLanguage(lang))
                {
                    lang = "";
                }
            }

            if (string.IsNullOrEmpty(lang))
            {
                Console.WriteLine();
            }

            while (string.IsNullOrEmpty(lang))
            {
                Console.WriteLine("Please, type the target language: ");
                lang = formatLanguageInput(Console.ReadLine().Trim());
                if (!ValidateLanguage(lang))
                {
                    lang = "";
                }
            }

            return lang;
        }

        string formatLanguageInput(string lang)
        {
            foreach(var language in AvailableLanguages)
            {
                if(lang.ToLower() == language.ToLower())
                {
                    lang = language;
                }
            }

            return lang;
        }

        bool ValidateLanguage(string lang)
        {
            bool ok = true;

            if(!AvailableLanguages.Contains(lang))
            {
                if(lang.ToLower() == "exit")
                {
                    Environment.Exit(0);
                }

                if (!string.IsNullOrEmpty(lang))
                {
                    Console.WriteLine();
                    Console.WriteLine($"Target language {lang} is invalid.");
                }

                Console.WriteLine("Available languages:");

                foreach (string language in AvailableLanguages)
                {
                    Console.WriteLine(language);
                }

                Console.WriteLine();

                ok = false;
            }
            
            return ok;
        }

        bool ValidateAOSServiceFolder(string aosServiceFolder)
        {
            bool ok = true;

            if (!AvailableAOSServiceFolders.Contains(aosServiceFolder))
            {
                Console.WriteLine($"AOSService folder {aosServiceFolder} is invalid.");
                Console.WriteLine("Available AOSService folders:");
                foreach (string folder in AvailableAOSServiceFolders)
                {
                    Console.WriteLine(folder);
                }
                Console.WriteLine();
            }

            return ok;
        }

        public bool init()
        {
            bool ok = true;
            initAvailableAOSServiceFolders();
            
            if(AvailableAOSServiceFolders.Count() == 0)
            {
                Console.WriteLine("Failed! No AOSService folder found.");
                ok = false;
            }

            if (ok)
            {
                if (string.IsNullOrEmpty(Arguments.AOSServiceFolder))
                {
                    Arguments.AOSServiceFolder = AvailableAOSServiceFolders[0];
                }
                else if (!ValidateAOSServiceFolder(Arguments.AOSServiceFolder))
                {
                    ok = false;
                }

                initAvailableLanguages();

                if (AvailableLanguages.Count() == 0)
                {
                    Console.WriteLine($"Failed! No languages found on AOSService folder {Arguments.AOSServiceFolder}.");
                    ok = false;
                }

                if (!string.IsNullOrEmpty(Arguments.Language))
                {
                    Arguments.Language = formatLanguageInput(Arguments.Language);
                    ok = ValidateLanguage(Arguments.Language);
                }
            }

            return ok;
        }

        public void initAvailableAOSServiceFolders()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();

            foreach (DriveInfo drive in drives)
            {
                if (drive.DriveType == DriveType.Fixed
                    && Directory.Exists(drive.Name + AOSServicePath + FoundationModelPath + LabelResourcesPath))
                {
                    AvailableAOSServiceFolders.Add(drive.Name + AOSServicePath);
                }
            }
        }

        public void initAvailableLanguages()
        {
            AvailableLanguages.AddRange(LabelHelper.GetInstalledLanguages().ToArray());
        }

        public void Run()
        {
            if (init())
            {
                if (string.IsNullOrEmpty(Arguments.Language))
                {
                    Arguments.Language = GetLanguageFromUserInput();
                }

                Console.WriteLine();
                Console.WriteLine($"AOSService folder: {Arguments.AOSServiceFolder}");
                Console.WriteLine($"Target language: {Arguments.Language}");
                Console.WriteLine();

                try
                {
                    var processStart = DateTime.Now;

                    GenerateLabelFiles();

                    var processEnd = DateTime.Now;

                    Console.WriteLine();
                    Console.WriteLine("Done!");
                    Console.WriteLine();
                    Console.WriteLine("Duration: " + (processEnd - processStart).ToString());
                }
                catch (Exception e)
                {
                    LogError(e);
                }
            }
        }

        public void GenerateLabelFiles()
        {
            Console.WriteLine("Preparing everything, please wait...");
            
            var labelFiles = GetLabelFiles();

            var tasks = new List<Task>();

            foreach (var labelFile in labelFiles)
            {
                if (labelFile.Labels != null && labelFile.Labels.Any())
                {
                    if (labelFile.Segments != null && labelFile.Segments.Any())
                    {
                        foreach (var segment in labelFile.Segments)
                        {
                            tasks.Add(new Task(() => segment.CreateFiles()));
                        }
                    }
                    else
                    {
                        tasks.Add(new Task(() => labelFile.CreateFiles()));
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("Creating label files...");

            if(Arguments.Verbose)
            {
                Console.WriteLine();
            }

            foreach (var t in tasks)
            {
                t.Start();
            }

            Task.WaitAll(tasks.ToArray());

            WriteSegmentedFiles(labelFiles);
        }
        
        private void WriteSegmentedFiles(List<LabelFile> labelFiles)
        {
            foreach (var labelFile in labelFiles)
            {
                if (labelFile.Segments != null && labelFile.Segments.Any())
                {
                    LabelFileSegment.Merge(labelFile.Segments).CreateFiles(Arguments.Verbose);
                }
            }
        }

        public List<LabelFile> GetLabelFiles()
        {
            var labelFiles = new List<LabelFile>();

            foreach (var modelPath in ModelsToProcess)
            {
                string labelFilePath =
                    Arguments.AOSServiceFolder + PackagesLocalDirectoryPath + modelPath
                    + "\\" + AxLabelFilePath;

                if (Directory.Exists(labelFilePath))
                {
                    string labelLanguagePath =
                        Arguments.AOSServiceFolder + PackagesLocalDirectoryPath + modelPath
                        + "\\" + LabelResourcesPath + Arguments.Language;

                    var fileNames = Directory.GetFiles(labelFilePath, "*" + enUSXmlSuffix);

                    if (fileNames != null && fileNames.Any())
                    {
                        foreach (var fileName in fileNames)
                        {
                            var suffix = "_" + fileName.Split('_').Last();
                            var labelFileName = fileName.Split(Path.DirectorySeparatorChar).Last().Replace(suffix, "");

                            var labelFile = new LabelFile(modelPath, labelFilePath, labelLanguagePath, Arguments.Language, labelFileName, Arguments.Verbose);
                            if (labelFile.Labels != null && labelFile.Labels.Any())
                            {
                                labelFiles.Add(labelFile);
                            }
                        }
                    }
                }
            }

            labelFiles.Sort((x, y) => x.Labels.Count().CompareTo(y.Labels.Count()));

            return labelFiles;
        }
    }
}
