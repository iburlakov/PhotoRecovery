using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PhotoRecovery.Core.Data;
using PhotoRecovery.Core.Data.Models;

namespace PhotoRecovery.Core.Cache
{
    public class DirRepository
    {
        private Context context;

        public Dictionary<long, Dir> AllDirs { get { return this.allDirs; } }
        private Dictionary<long, Dir> allDirs = new Dictionary<long, Dir>();


        private DirRepository(Context context)
        {
            this.context = context;

            // load root folders
            foreach (var model in context.Dirs.Where(d => d.ParentId == null))
            {
                var dir = new Dir(model);
                this.allDirs[dir.Id] = dir;

                this.rootDirIndex.Add(dir.Id);

                // load sub dirs
                this.LoadSubDirs(dir);
            }
        }

        private void LoadSubDirs(Dir parentDir)
        {
            List<long> index = null;
            foreach (var model in this.context.Dirs.Where(d => d.ParentId == parentDir.Id))
            {
                var dir = new Dir(model, parentDir);
                this.allDirs[dir.Id] = dir;

                if (index == null)
                {
                    if (!this.parentIdIndex.TryGetValue(parentDir.Id, out index))
                    {
                        parentIdIndex[parentDir.Id] = index = new List<long>();
                    }
                }

                parentIdIndex[parentDir.Id].Add(dir.Id);

                this.LoadSubDirs(dir);

            }
        }


        private List<long> rootDirIndex = new List<long>();
        private Dictionary<long, List<long>> parentIdIndex = new Dictionary<long, List<long>>();

        public IEnumerable<Dir> GetRootDirs()
        {
            foreach (var rootDirId in this.rootDirIndex)
            {
                yield return this.allDirs[rootDirId];
            }
        }

        public IEnumerable<Dir> GetDirsForParentDirId(long parentDirId)
        {
            List<long> index;
            if (this.parentIdIndex.TryGetValue(parentDirId, out index))
            {
                foreach (var dirId in index)
                {
                    yield return this.allDirs[dirId];
                }
            }
           
        }

        private static DirRepository instance;
        public static DirRepository Load(Context context)
        {
            if (instance == null)
            {
                instance = new DirRepository(context);
            }

            return instance;
        }
    }
}
