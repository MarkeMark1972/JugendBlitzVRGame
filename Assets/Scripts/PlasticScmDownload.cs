using System;
using System.Diagnostics;
using System.IO;

public class PlasticScmDownload
{
    private const string PlasticPath = @"C:\Windows\System32\cm.exe"; 
    // Update this with the real path to your 'cm' executable, if needed
    
    public static void SyncWorkspace(string workspacePath, string repositoryPath)
    {
        EnsureDirectoryExists(workspacePath);
        RunPlasticCommand($"mkwk {workspacePath} {repositoryPath}");
        RunPlasticCommand($"update {workspacePath}");
    }

    private static void RunPlasticCommand(string command)
    {
        var startInfo = new ProcessStartInfo(PlasticPath, command);
        using var process = new Process { StartInfo = startInfo };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Failed to run Plastic SCM command: {command}");
        }
    }

    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
}