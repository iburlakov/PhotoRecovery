﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using NLog;

using PhotoRecovery.Core.Data;

namespace PhotoRecovery.Core.Cache
{
    public class DirRepository
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

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
            var isIndexCreated = false;
            foreach (var model in this.context.Dirs.Where(d => d.ParentId == parentDir.Id))
            {
                var dir = new Dir(model, parentDir);
                this.allDirs[dir.Id] = dir;

                if (!isIndexCreated)
                {
                    this.parentIdIndex[parentDir.Id] = new List<long>();

                    isIndexCreated = true;
                }

                parentIdIndex[parentDir.Id].Add(dir.Id);

                this.LoadSubDirs(dir);
            }
        }

        private List<long> rootDirIndex = new List<long>();
        private Dictionary<long, List<long>> parentIdIndex = new Dictionary<long, List<long>>();

        private static DirRepository instance;
        public static DirRepository Load(Context context)
        {
            if (instance == null)
            {
                instance = new DirRepository(context);
            }

            return instance;
        }

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

        public Dir SetRestored(Dir dir)
        {
            try
            {
                var model = this.context.Dirs.Where(d => d.Id == dir.Id).Single();

                model.Restored = true;

                this.context.Entry(model).State = EntityState.Modified;

                this.context.SaveChanges();

                var updatedDir = new Dir(model, this.allDirs[model.ParentId.Value]);

                this.allDirs[updatedDir.Id] = updatedDir;

                return updatedDir;
            }
            catch(Exception e)
            {
                log.Error(e, "Could not ser {0} to Restored = true", dir);

                return null;
            }
        }
    }
}
