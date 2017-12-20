using NLog;
using PhotoRecovery.Core.Cache;
using PhotoRecovery.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public void Restore(long dirId)
        {
            // TODO
        }
    }
}
