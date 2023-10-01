namespace Inovatec.OfficeManagementTool.Common.Helpers
{
    public class TaskSchedulerHelper
    {
        public static void ScheduleTask(DateOnly date)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "schtasks.exe";
            startInfo.Arguments = $"/create /sc ONCE /tn \"{ConfigProvider.ScheduledTaskFolder}\\{ConfigProvider.ScheduledTaskName}\" /tr \"'{Directory.GetCurrentDirectory()}\\Inovatec.OfficeManagementTool.Scheduler.exe'\" /st \"09:00\" /sd \"{date.ToString("MM/dd/yyyy")}\"";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static void DeleteScheduled()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "schtasks.exe";
            startInfo.Arguments = $"/delete /tn \"{ConfigProvider.ScheduledTaskFolder}\\{ConfigProvider.ScheduledTaskName}\" /f";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        public static bool IsScheduled()
        {
            bool result;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "powershell.exe";
            startInfo.Arguments = $"-File \"{ConfigProvider.TaskExistsScript}\"";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            StreamReader reader = process.StandardOutput;
            string output = reader.ReadLine();
            result = int.Parse(output ?? "0") == 1;

            return result;
        }
    }
}