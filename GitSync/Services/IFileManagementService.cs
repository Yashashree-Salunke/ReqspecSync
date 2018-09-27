using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitSync.Services 
{
    public interface IFileManagementService
    {
        void AddFile(int userstoryId, List<FileParameter> list, string repositoryUrl, Repository repository);
        void DeleteFile(int userstoryId, List<FileParameter> list, string repositoryUrl);
        void UpdateFile(int userstoryId, List<FileParameter> list, string repositoryUrl);
    }
}
