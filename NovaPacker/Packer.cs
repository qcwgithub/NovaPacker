using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace NovaPacker
{
    public class Packer
    {
        public static string UnityPath = null;
        public static string ProjectPath = null;
        public static string version = "1.0";

        public static void PackAndroid(Action<int> p, bool tip)
        {
            PrepareClientFolders("Android");
            p(0);

            if (File.Exists(Packer.ProjectPath + ".\\Build\\Nova.apk"))
                File.Delete(Packer.ProjectPath + ".\\Build\\Nova.apk");

            p(1);
            int stepCount = 4;
            for (int i = 0; i < stepCount; i++)
            {
                PackClient("Android", i);
                p((i + 1) * 100 / stepCount);
            }

            p(100);
        }

        public static bool DoInfoPlistModifier()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "InfoPlistModifier.exe";
            ps.Arguments = Packer.ProjectPath + "\\Build\\Client\\IOS\\xcodeproject\\Info.plist " + version;
            ps.RedirectStandardOutput = true;
            ps.RedirectStandardError = true;
            ps.UseShellExecute = false;
            Process p = new Process();
            p.StartInfo = ps;
            return p.Start();
        }

        public static void PackIOS(Action<int> p, bool tip)
        {
            PrepareClientFolders("IOS");
            p(0);

            if (Directory.Exists(Packer.ProjectPath + ".\\Build\\xcodeproject"))
                Directory.Delete(Packer.ProjectPath + ".\\Build\\xcodeproject", true);

            p(1);
            int stepCount = 4;
            for (int i = 0; i < stepCount; i++)
            {
                PackClient("IOS", i);
                p((i + 1) * 100 / stepCount);
            }

            
            if (!DoInfoPlistModifier())
                MessageBox.Show("执行 InfoPlistModifier 失败");

            p(100);
        }

        public static void PackWindows(Action<int> p, bool tip)
        {
            PrepareClientFolders("Windows");
            p(0);

            if (File.Exists(Packer.ProjectPath + ".\\Build\\Nova.exe"))
                File.Delete(Packer.ProjectPath + ".\\Build\\Nova.exe");

            if (Directory.Exists(Packer.ProjectPath + ".\\Build\\Nova_Data"))
                Directory.Delete(Packer.ProjectPath + ".\\Build\\Nova_Data", true);

            p(1);
            int stepCount = 4;
            for (int i = 0; i < stepCount; i++)
            {
                PackClient("Windows", i);
                p((i + 1) * 100 / stepCount);
            }

            p(100);
        }

        public static bool UpdateSVN()
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = "svn.exe";
            ps.WorkingDirectory = Packer.ProjectPath;
            ps.Arguments = "update";
            ps.RedirectStandardOutput = true;
            ps.RedirectStandardError = true;
            ps.UseShellExecute = false;
            Process p = new Process();
            p.StartInfo = ps;
            if (!p.Start())
                return false;

            p.WaitForExit();

            string str = p.StandardOutput.ReadLine();
            while (str != null)
            {
                if (str.IndexOf("Error") >= 0)
                    return false;
                else if (str.IndexOf("At revision") >= 0)
                    return false;
                else if (str.IndexOf("Update to revision") >= 0)
                    return true;

                str = p.StandardOutput.ReadLine();
            }

            return false;
        }

        public static bool CopyAllCsvCs(string from, string to)
        {
            string fromPath = "" + ProjectPath + "\\Client" + from + "\\Assets\\Scripts\\Logic\\Csv\\CsvAllDataAllText.cs";
            string toPath = "" + ProjectPath + "\\Client" + to + "\\Assets\\Scripts\\Logic\\Csv\\CsvAllDataAllText.cs";
            if (File.Exists(toPath))
                File.Delete(toPath);

            File.Copy(fromPath, toPath);
            return File.Exists(toPath);
        }

        public static bool ClearAndExportAllCsvText(string platform)
        {
            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = UnityPath;
            ps.Arguments = "-batchmode -nographics -quit -logFile .\\packlog_all.txt -projectPath \"" + ProjectPath + "\\Client" + platform + "\" -executeMethod AutoBuild.ClearAndExportAllCsvText";

            Process p = new Process();
            p.StartInfo = ps;

            if (!p.Start())
                return false;

            p.WaitForExit();

            return File.Exists(ProjectPath + "\\config\\all.csv");
        }

        public static bool PackClient(string platform, int step)
        {
            if (!File.Exists(ProjectPath + "\\config\\all.csv"))
            {
                MessageBox.Show("找不到 all.csv，请先成功执行导表操作");
                return false;
            }

            ProcessStartInfo ps = new ProcessStartInfo();
            ps.FileName = UnityPath;
            ps.Arguments = "-batchmode -nographics -quit -logFile .\\packlog_" + platform + "_" + step + ".txt -projectPath \"" + ProjectPath + "\\Client" + platform + "\" -executeMethod AutoBuild.Build" + platform + "_" + step;

            Process p = new Process();
            p.StartInfo = ps;

            if (!p.Start())
                return false;

            p.WaitForExit();
            return true;
        }

        public static void PrepareServerFolders()
        {
            if (!Directory.Exists(ProjectPath + "\\Build\\Server"))
                Directory.CreateDirectory(ProjectPath + "\\Build\\Server");

            if (Directory.Exists(ProjectPath + "\\Build\\Server\\Config"))
                Directory.Delete(ProjectPath + "\\Build\\Server\\Config", true);

            Directory.CreateDirectory(ProjectPath + "\\Build\\Server\\Config");

            if (File.Exists(ProjectPath + "\\Build\\Server\\Server.exe"))
                File.Delete(ProjectPath + "\\Build\\Server\\Server.exe");

            if (File.Exists(ProjectPath + "\\Build\\Server\\mysql.data.dll"))
                File.Delete(ProjectPath + "\\Build\\Server\\mysql.data.dll");
        }

        public static void PrepareClientFolders(string platform)
        {
            if (!Directory.Exists(ProjectPath + "\\Build\\Client"))
                Directory.CreateDirectory(ProjectPath + "\\Build\\Client");

            if (Directory.Exists(ProjectPath + "\\Build\\Client\\" + platform))
                Directory.Delete(ProjectPath + "\\Build\\Client\\" + platform, true);

            Directory.CreateDirectory(ProjectPath + "\\Build\\Client\\" + platform);
        }

        public static void CompileServer()
        {
            // todo

            CopyServerFile();
        }

        static void CopyServerFile()
        {
            // Server
            PrepareServerFolders();
            File.Copy(ProjectPath + "\\Server\\Server\\bin\\Debug\\Server.exe", ProjectPath + "\\Build\\Server\\Server.exe");
            File.Copy(ProjectPath + "\\Server\\Server\\bin\\Debug\\mysql.data.dll", ProjectPath + "\\Build\\Server\\mysql.data.dll");
            
            string[] fs1 = Directory.GetFiles(ProjectPath + "\\Config", "*.csv");
            string[] fs2 = Directory.GetFiles(ProjectPath + "\\Config", "*.sc");
            string[] fs3 = Directory.GetFiles(ProjectPath + "\\Config", "*.txt");
            IEnumerable<string> fs = fs1.Concat(fs2).Concat(fs3);
            foreach (string f in fs)
                File.Copy(f, ProjectPath + "\\Build\\Server\\Config\\" + f.Substring(f.LastIndexOf("\\") + 1));
        }
    }
}
