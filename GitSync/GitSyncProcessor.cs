using GitSync.Services;
using LibGit2Sharp;
using ReqspecModels;
using System.Collections.Generic;
using System.IO;

namespace GitSync
{
    public class GitSyncProcessor : IGitSyncProcessor
    {
        private readonly IDatabaseManagementService _databaseManagementService;
        private readonly IFileManagementService _fileManagementService;
        private readonly IGitInteraction _gitInteraction;

        public GitSyncProcessor (
            IDatabaseManagementService databaseManagementService,
            IFileManagementService fileManagementService,
            IGitInteraction gitInteraction
            )
        {
            _databaseManagementService = databaseManagementService;
            _fileManagementService = fileManagementService;
            _gitInteraction = gitInteraction;
        }
        public void Execute(GitSyncContext context)
        {
            string Username = context.Username;
            string Password = context.Password;

            //Clone Repository to Temperory Path
            var co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = context.Username, Password = context.Password };
            var TempPath = "C:/TemperoryFolder";
            Repository.Clone(context.RepositoryUrl, TempPath);

            using (var repository = new Repository(TempPath))
            {
                string branch=_gitInteraction.CreateBranch(repository);
                foreach (var record in context .Records)
                {
                   
                  List <FileParameter> list=_databaseManagementService.GetValuesFromDb(context.SourceConnectionString, record.UserstoryId);
                    var actionType = (UserstorySyncActionTypeEnum)record.UserstorySyncActionTypeId;  
                    switch (actionType )
                    {
                        case UserstorySyncActionTypeEnum.ADD:
                            _fileManagementService.AddFile(record.UserstoryId,list, TempPath, repository);
                            break;
                        case UserstorySyncActionTypeEnum.DELETE:
                            _fileManagementService.DeleteFile(record.UserstoryId, list, TempPath);
                            break;
                        case UserstorySyncActionTypeEnum.UPDATE:
                            _fileManagementService.UpdateFile(record.UserstoryId, list, TempPath);
                            break;
                    }
                   
                }
                _gitInteraction.PushAndComit(Username, Password, repository,branch);
                _gitInteraction.PullRequest(Username, Password, repository);
                Directory.Delete(TempPath,true);
            }

        }

    }
}



          