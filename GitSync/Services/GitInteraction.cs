using System;
using System.Collections.Generic;
using System.Text;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;

namespace GitSync.Services
{
    public class GitInteraction : IGitInteraction
    {
        public string CreateBranch(Repository repository)
        {
            var branch = "SYNC_20180927_37";
            var BranchName = repository.CreateBranch(branch);
            repository.Branches.Update(BranchName,
                                b => b.Remote = "origin",
                                b => b.UpstreamBranch = BranchName.CanonicalName);
            return branch;
        }

        public void PullRequest(string username, string password, Repository repository)
        {
            PullOptions options = new PullOptions();
            options.FetchOptions = new FetchOptions();
            options.FetchOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) =>
            new UsernamePasswordCredentials()
            {
                Username = username ,
                Password = password 
            });
            var signature = new Signature(new Identity(username , username), DateTimeOffset.Now);
           LibGit2Sharp.Commands.Pull(repository, signature, options);
        }

        public void PushAndComit(string username, string password, Repository repository,string branchname)
        {
            Signature author = new Signature(username, username, DateTime.Now);
            Signature committer = author;
            Commit commit = repository.Commit("File is modified", author, committer);  // Commit to the repository

            PushOptions options = new PushOptions();
            options.CredentialsProvider = new CredentialsHandler(
                (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = username,
                        Password = password
                    });
            repository.Network.Push(repository.Branches[branchname], options);
        }


    }
}
