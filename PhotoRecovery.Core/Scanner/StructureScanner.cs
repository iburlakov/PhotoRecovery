using System;

using System.IO;

using NLog;

using PhotoRecovery.Core.Data;
using PhotoRecovery.Core.Data.Models;
using System.Diagnostics;

namespace PhotoRecovery.Core.Scanner
{
    public class StructureScanner : IScanner
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Context context;

        private readonly Stopwatch stopwatch;
        private int statFilesNum = 0;
        private int statDirNum = 0;

        public StructureScanner()
        {
            this.context = new Context();
            this.stopwatch = new Stopwatch();
        }

        public void Scan(string path)
        {
            if (Directory.Exists(path))
            {
                this.stopwatch.Start();
                log.Info("Starting scanning {0}", path);

                this.ScanDir(path);

                log.Info("Done, scanned {0} folders {1} files in {2:c}", this.statDirNum, this.statFilesNum,  this.stopwatch.Elapsed);
                this.stopwatch.Stop();
            }
        }

        private void ScanDir(string dirPath, Dir parentDir = null)
        {
            var dir = this.context.Dirs.Add(new Dir() { Name = Path.GetFileName(dirPath), ParentId = parentDir?.Id });
            context.SaveChanges();

            this.statDirNum++;

            foreach (var fileName in Directory.GetFiles(dirPath))
            {
                var filePath = Path.Combine(dirPath, fileName);
                this.ScanFile(filePath, dir);
            }
            context.SaveChanges();

            foreach (var subDirName in Directory.GetDirectories(dirPath))
            {
                var subDirPath = Path.Combine(dirPath, subDirName);
                this.ScanDir(subDirPath, dir);
            }
        }

        private void ScanFile(string filePath, Dir parentDir)
        {
            var fileInfo = new FileInfo(filePath);
            this.context.Files.Add(new Data.Models.File() { Name = fileInfo.Name, ParentId = parentDir.Id, Length = fileInfo.Length, Created = fileInfo.CreationTime, Modified = fileInfo.LastWriteTime });
  

            this.statFilesNum++;
            if (this.statFilesNum == 1000 || this.statFilesNum == 5000 || this.statFilesNum == 10000)
            {
                log.Info("Scanned {0} files in {1:c}", this.statFilesNum, this.stopwatch.Elapsed);
            }

            if (this.statFilesNum % 1000 == 0)
            {
                log.Info("Scanned {0} files", this.statFilesNum);
            }
        }
    }
}
