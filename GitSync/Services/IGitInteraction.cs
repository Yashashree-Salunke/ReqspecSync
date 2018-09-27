using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace GitSync.Services
{
    public interface IGitInteraction
    {
        string CreateBranch(Repository repository);
        void PushAndComit(string username, string password, Repository repository,string branchname);
        void PullRequest(string username, string password, Repository repository);
    }
}
