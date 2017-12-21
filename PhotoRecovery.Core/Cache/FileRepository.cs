using PhotoRecovery.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoRecovery.Core.Cache
{
    public class FileRepository
    {
        private Context context;
        private DirRepository dirRepo;

        public Dictionary<long, File> AllFiles { get { return this.allFiles; } }
        private Dictionary<long, File> allFiles = new Dictionary<long, File>();


        private FileRepository(Context context, DirRepository folderRepository)
        {
            this.context = context;
            this.dirRepo = folderRepository;

            foreach (var rootDir in this.dirRepo.GetRootDirs())
            {
                this.LoadFilesForFolder(rootDir);
            }
        }

        private void LoadFilesForFolder(Dir parentDir)
        {

            var isIndexCreated = false;
            foreach (var model in this.context.Files.Where(f => f.ParentId == parentDir.Id))
            {
                var file = new File(model, parentDir);
                this.allFiles[file.Id] = file;

                if(!isIndexCreated)
                {
                    this.parentDirIdIndex[parentDir.Id] = new List<long>();

                    isIndexCreated = true;
                }

                this.parentDirIdIndex[parentDir.Id].Add(file.Id);
            }

            foreach (var subDir in this.dirRepo.GetDirsForParentDirId(parentDir.Id)) 
            {
                this.LoadFilesForFolder(subDir);
            }
        }

        private static FileRepository instance;
        public static FileRepository Load(Context context, DirRepository folderRepository)
        {
            if (instance == null)
            {
                instance = new FileRepository(context, folderRepository);
            }

            return instance;
        }

        private Dictionary<long, List<long>> parentDirIdIndex = new Dictionary<long, List<long>>();

        public IEnumerable<File> GetFilesForDirectoryId(long dirId)
        {
            List<long> ids;
            if (this.parentDirIdIndex.TryGetValue(dirId, out ids))
            {
                foreach (var id in ids)
                {
                    File file;
                    if (this.allFiles.TryGetValue(id, out file))
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}
