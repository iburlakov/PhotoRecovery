using NLog;
using PhotoRecovery.Core.Cache;
using PhotoRecovery.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PhotoRecovery.Core.Restore
{
    public class Restorer
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        private Context context;

        private DirRepository dirRepo;
        private FileRepository fileRepo;
        private RawFileRepository rawRepo;

        public Restorer()
        {
            this.context = new Context();

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            this.dirRepo = DirRepository.Load(this.context);
            log.Info("All dirs are loaded {0:c}", stopwatch.Elapsed);

            this.fileRepo = FileRepository.Load(this.context, this.dirRepo);
            log.Info("All files are loaded {0:c}", stopwatch.Elapsed);

            this.rawRepo = RawFileRepository.Load(this.context);
            log.Info("Raw files are loaded {0:c}", stopwatch.Elapsed);

            var restoreFolderName = "restore";
            var restoreFolderAttempt = Path.Combine(Environment.CurrentDirectory, restoreFolderName);
            var attempts = 0;

            while (Directory.Exists(restoreFolderAttempt))
            {
                attempts++;
                restoreFolderAttempt = Path.Combine(Environment.CurrentDirectory, restoreFolderName + "_" + attempts.ToString());
            }

            this.toRestorePath = restoreFolderAttempt;
            Directory.CreateDirectory(toRestorePath);
        }

        private readonly string toRestorePath;

        public void Restore(long dirId)
        {
            this.Restore(dirId, this.toRestorePath);

            log.Warn("Report for restoring dir id = {0}, {3} scanned. SUCCESS:\n{1}\nERRORS:\n{2}", dirId, this.success, this.failed, this.numFilesRestored);
        }

        int numFilesRestored = 0;
        StringBuilder success = new StringBuilder();
        StringBuilder failed = new StringBuilder();

        private void Restore(long dirId, string restoreToPath)
        {
            var dir = this.dirRepo.AllDirs[dirId];
            var path = Path.Combine(restoreToPath, dir.Name);

            Directory.CreateDirectory(path);

            foreach (var file in this.fileRepo.GetFilesForDirectoryId(dir.Id))
            {
                var fileExtension = Path.GetExtension(file.Path).ToLower();
                var possibleContent = this.rawRepo.GetRawFilesForLength(file.Length).Where(f => Path.GetExtension(f.Path) == fileExtension);

                var attempts = 0;
                foreach (var content in possibleContent)
                {
                    var fileName = attempts == 0 ? file.Name : Path.GetFileNameWithoutExtension(file.Name) + "_" + attempts.ToString() + Path.GetExtension(file.Name);
                    var restoredFilePath = Path.Combine(path, fileName);

                    try
                    {
                        System.IO.File.Copy(content.Path, restoredFilePath);
                        attempts++;
                    }
                    catch (Exception e)
                    {
                        log.Error(e, "Could not copy {0} to {1}", content.Path, restoredFilePath);
                    }
                }

                if (attempts > 0)
                {
                    this.success.AppendLine(string.Format("{0} [{1} found]", file.Path, attempts + 1));
                }
                else
                {
                    this.failed.AppendLine(string.Format("{0} [NOT FOUND]", file.Path));
                }
                this.numFilesRestored++;
            }

            foreach (var subDir in this.dirRepo.GetDirsForParentDirId(dir.Id))
            {
                this.Restore(subDir.Id, path);
            }
        }
    }
}
