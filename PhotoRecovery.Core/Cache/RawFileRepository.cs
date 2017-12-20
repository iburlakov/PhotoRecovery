using PhotoRecovery.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoRecovery.Core.Cache
{
    public class RawFileRepository
    {
        private Context context;


        public Dictionary<long, RawFile> AllRawFiles { get { return this.allRawFiles; } }
        private Dictionary<long, RawFile> allRawFiles = new Dictionary<long, RawFile>();

        private RawFileRepository(Context context)
        {
            this.context = context;

            foreach (var model in this.context.RawFiles)
            {
                var file = new RawFile(model);
                allRawFiles[file.Id] = file;
            }
        }

        private static RawFileRepository instance;
        public static RawFileRepository Load(Context context)
        {
            if (instance == null)
            {
                instance = new RawFileRepository(context);
            }

            return instance;
        }
    }
}
