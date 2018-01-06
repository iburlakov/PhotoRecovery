using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using NLog;

using PhotoRecovery.Core.Data;

namespace PhotoRecovery.Core.Scanner
{
    public class RawScanner : IScanner
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private readonly Context context;

        private readonly Stopwatch stopwatch;
        private int statFilesNum = 0;
        private int statDirNum = 0;

        public RawScanner()
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

                this.SaveFiles();

                log.Info("Done, scanned {0} folders {1} files in {2:c}", this.statDirNum, this.statFilesNum, this.stopwatch.Elapsed);
                this.stopwatch.Stop();
            }
        }

        private const int FILES_TO_SAVE_CACHE_SIZE = 2500;
        private List<Data.Models.RawFile> filesToSave = new List<Data.Models.RawFile>();

        private void ScanDir(string dirPath)
        {
            this.statDirNum++;

            foreach (var fileName in Directory.GetFiles(dirPath))
            {
                var filePath = Path.Combine(dirPath, fileName);
                this.ScanFile(filePath);
            }

            foreach (var subDirName in Directory.GetDirectories(dirPath))
            {
                var subDirPath = Path.Combine(dirPath, subDirName);
                this.ScanDir(subDirPath);
            }
        }

        private void ScanFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);

            this.filesToSave.Add(new Data.Models.RawFile() { Path = filePath, Length = fileInfo.Length, Created = fileInfo.CreationTime, Modified = fileInfo.LastWriteTime });

            this.statFilesNum++;
            if (this.statFilesNum == 1000 || this.statFilesNum == 5000 || this.statFilesNum == 10000)
            {
                log.Info("Scanned {0} files in {1:c}", this.statFilesNum, this.stopwatch.Elapsed);
            }

            if (this.statFilesNum % 1000 == 0)
            {
                log.Info("Scanned {0} files", this.statFilesNum);
            }

            if (this.filesToSave.Count > FILES_TO_SAVE_CACHE_SIZE)
            {
                this.SaveFiles();
            }
        }

        private void SaveFiles()
        {
            this.filesToSave.ForEach(f => this.context.RawFiles.Add(f));

            var start = this.stopwatch.Elapsed;
            this.context.SaveChanges();
            log.Info("Saved {0} files to database, taken {1:c}", this.filesToSave.Count, this.stopwatch.Elapsed - start);

            start = this.stopwatch.Elapsed;
            this.context.Vacuum();
            log.Info("Vacuum, taken {0:c}", this.stopwatch.Elapsed - start);

            this.filesToSave.Clear();
        }
    }
}
