using System;
using System.IO;
using System.Diagnostics;

namespace KeyNStroke.Updater
{
    class Updater
    {
        public static Statemachine Instance
        {
            get { return Statemachine.Instance; }
        }


        private Updater()
        {
        }

        /// <summary>
        /// Returns true if the program should exit
        /// </summary>
        /// <param name="args"></param>
        static public bool HandleArgs(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "--update-step2" && args.Length == 3)
                {
                    UpdateStep2(args[1], int.Parse(args[2]));
                    return true;
                }
                if (args[0] == "--update-step3" && args.Length == 3)
                {
                    UpdateStep3(args[1], int.Parse(args[2]));
                    return false; // continue running
                }
                if (Admininstration.HandleArgs(args))
                {
                    return true;
                }
            }
            return false;
        }



        static public void TriggerUpdateStep2(byte[] update)
        {
            Random r = new Random();
            string oldExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            string tmpExeName = $"PxKeystrokes_Updater.exe";
            string oldExeParentFolder = Path.GetDirectoryName(oldExePath); // may not be writable
            string systemTmpFolder = Path.GetTempPath(); // Should be writable in any case
            string tmpExePath = Path.Combine(systemTmpFolder, tmpExeName);

            using (var fs = new FileStream(tmpExePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(update, 0, update.Length);
            }
            int ownPid = Process.GetCurrentProcess().Id;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = tmpExePath;
            psi.WorkingDirectory = Path.GetDirectoryName(tmpExePath);
            psi.Arguments = $"--update-step2 \"{oldExePath}\" {ownPid}";

            if (!Utils.IsDirectoryWritable(oldExeParentFolder))
            {
                psi.UseShellExecute = true;
                psi.Verb = "runas";
            }

            Process p = Process.Start(psi);
            // Shutdown own process immediately!
        }

        /// <summary>
        /// Handles the swap of the executables.
        /// The we are the updated executable and the previous executable (the one that started us) must be overwritten by us.
        /// 
        /// </summary>
        static public void UpdateStep2(string oldExePath, int startingProcessPid)
        {
            try
            {
                Process orig = Process.GetProcessById(startingProcessPid);
                if (!orig.WaitForExit(60000))
                {
                    Console.WriteLine("ERROR: Calling process did not exit in time");
                    return;
                }
                orig.Close();
            } catch (ArgumentException)
            {
                // The process specified by the processId parameter is not running. The identifier might be expired.
            }

            string tmpExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            Log.e("UPDATE", $"Copy from {tmpExePath} to {oldExePath}");
            if (File.Exists(oldExePath))
            {
                File.Delete(oldExePath);
            }
            File.Copy(tmpExePath, oldExePath);

            int ownPid = Process.GetCurrentProcess().Id;

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = oldExePath;
            psi.WorkingDirectory = Path.GetDirectoryName(oldExePath);
            psi.Arguments = $"--update-step3 \"{tmpExePath}\" {ownPid}";

            Process.Start(psi); // Even if the updater has been started as Admin, this will start as the original user again.
            // Shutdown own process immediately
        }

        /// <summary>
        /// Delete the temporary executable
        /// </summary>
        /// <param name="tmpExePath"></param>
        /// <param name="startingProcessPid"></param>
        static public void UpdateStep3(string tmpExePath, int startingProcessPid)
        {
            try
            {
                Process orig = Process.GetProcessById(startingProcessPid);
                if (!orig.WaitForExit(60000))
                {
                    Console.WriteLine("ERROR: Calling process did not exit in time");
                    return;
                }
                orig.Close();
            }
            catch (ArgumentException)
            {
                // The process specified by the processId parameter is not running. The identifier might be expired.
            }

            if (File.Exists(tmpExePath))
            {
                Log.e("UPDATE", $"Deleted {tmpExePath}");
                File.Delete(tmpExePath);
            }
        }
    }
}
