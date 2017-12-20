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
            foreach (var model in this.context.Files.Where(f => f.ParentId == parentDir.Id))
            {
                var file = new File(model, parentDir);
                this.allFiles[file.Id] = file;
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
    }
}
