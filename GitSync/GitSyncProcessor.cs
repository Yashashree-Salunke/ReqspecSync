using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace GitSync
{
    public class GitSyncProcessor : IGitSyncProcessor
    {
        public void Execute(GitSyncContext context)
        {
            string Username = context.Username;
            string Password = context.Password;
            //Clone Repository to Temperory Path
            var co = new CloneOptions();
            co.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = context .Username , Password = context .Password };
            Repository.Clone("https://github.com/Yashashree-Salunke/ReqspecSync.git", context .RepositoryUrl);
            var TempPath = context.RepositoryUrl;

            using (var repository = new Repository(TempPath))
            {
                foreach (var record in context.Records)
                {
                    List<FileParameter> list = new List<FileParameter>();
                    SqlConnection cons = new SqlConnection(context.SourceConnectionString);
                    SqlCommand command = new SqlCommand("SELECT u.Id,u.ParentId,u.RootId,u.Title,u.HasChildren,u.Description,a.Title AS A_Title,a.Gwt,t.Title AS T_Title FROM UserStories AS u JOIN AcceptanceCriterias AS a ON a.UserstoryId =u.Id JOIN UserstoryTags as st ON st.UserstoryId=u.Id JOIN Tags AS t ON t.Id=st.TagId WHERE u.Id = @StoryId");
                    command.Parameters.AddWithValue("@StoryId", record.UserstoryId);
                    command.Connection = cons;
                    command.Connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    FileParameter p = new FileParameter();
                    while (reader.Read())
                    {
                        p.Id = (int)reader["Id"];
                        p.HasChild = (bool)reader["HasChildren"];

                        if (reader["ParentId"] == DBNull.Value)
                        {
                            p.ParentId = 0;
                        }
                        else
                        {
                            p.ParentId = (int)reader["ParentId"];
                        }

                        if (reader["RootId"] == DBNull.Value)
                        {
                            p.RootId = 0;
                        }
                        else
                        {
                            p.RootId = (int)reader["RootId"];
                        }

                        p.Title = (string)reader["Title"];
                        p.Description = (string)reader["Description"];
                        p.ACriteriaName = (string)reader["A_Title"];
                        p.GWT = (string)reader["Gwt"];
                        p.TagName = (string)reader["T_Title"];
                        list.Add(p);
                    }
                    var ActionTypeId = record.UserstorySyncActionTypeId;
                    switch (ActionTypeId)
                    {
                        case 1:
                            Add_File(record.UserstoryId, list, TempPath, repository);
                            break;
                        case 2:
                            Delete_File(record.UserstoryId, list, TempPath);
                            break;
                        case 3:
                            Update_File(record.UserstoryId, list, TempPath);
                            break;
                    }

                }
                PushAndCommit(Username, Password, repository);
                Directory.Delete(TempPath);  //Delete Temperory Created Folder
            }

        }

        private void Add_File(int userstoryId, List<FileParameter> list, string repositoryUrl, Repository repository)
        {

            DirectoryInfo ParentDirectory = new DirectoryInfo(repositoryUrl);
            List<FileParameter> NewList = new List<FileParameter>();
            NewList.AddRange(list);
            foreach (var item in list)
            {
                if (userstoryId == item.Id)
                {
                    if (item.ParentId == 0 && item.RootId == 0)
                    {
                        if (item.HasChild == true)
                        {
                            var FolderName = item.Title.Replace(" ", "_");
                            var Folder = Directory.CreateDirectory(
                                 Path.Combine(ParentDirectory.FullName, FolderName));
                            string FileName = item.Title.Replace(" ", "");
                            string fullpath = Path.Combine(repository.Info.WorkingDirectory, FileName + ".feature");
                            var contents = "Feature:" + item.Title + "\n" + item.Description + "\n" + "@" + item.TagName + "\n"
                                     + "Scenario:" + item.ACriteriaName + "\n" + "\t" + item.GWT;
                            File.WriteAllText(fullpath, contents.Replace('*', ' '));
                            repository.Index.Add(FileName + ".feature");
                            LibGit2Sharp.Commands.Stage(repository, fullpath);
                        }
                        else
                        {
                            string FileName = item.Title.Replace(" ", "");
                            string fullpath = Path.Combine(repository.Info.WorkingDirectory, FileName + ".feature");
                            var contents = "Feature:" + item.Title + "\n" + item.Description + "\n" + "@" + item.TagName + "\n"
                                    + "Scenario:" + item.ACriteriaName + "\n" + "\t" + item.GWT;
                            File.WriteAllText(fullpath, contents.Replace('*', ' '));
                            repository.Index.Add(FileName + ".feature");
                            LibGit2Sharp.Commands.Stage(repository, fullpath);
                        }
                    }
                    if (item.ParentId > 0)
                    {
                        if (item.HasChild == true)
                        {
                            var FolderName = item.Title.Replace(" ", "_");
                            foreach (var items in NewList)
                            {
                                if (items.Id == item.ParentId)
                                {
                                    string title = items.Title;
                                    List<string> folders = ParentDirectory.GetDirectories(title.Replace(" ", "_"), SearchOption.AllDirectories).Select(t => t.FullName).ToList();
                                    string Fullpath = folders.Single();
                                    var folder = Directory.CreateDirectory(Path.Combine(Fullpath, FolderName));
                                    string file = item.Title.Replace(" ", "");
                                    string filepath = Path.Combine(Fullpath, title + ".feature");
                                    var contents = "Feature:" + item.Title + "\n" + item.Description + "\n" + "@" + item.TagName + "\n"
                                    + "Scenario:" + item.ACriteriaName + "\n" + "\t" + item.GWT;
                                    File.WriteAllText(filepath, contents.Replace('*', ' '));
                                    repository.Index.Add(title + ".feature");
                                    LibGit2Sharp.Commands.Stage(repository, filepath);

                                }
                            }
                        }
                        else
                        {
                            string Filename = item.Title.Replace(" ", "");
                            foreach (var items in NewList)
                            {
                                if (items.Id == item.ParentId)
                                {

                                    string title = items.Title;
                                    List<string> folders = ParentDirectory.GetDirectories(title.Replace(" ", "_"), SearchOption.AllDirectories).Select(t => t.FullName).ToList();
                                    string Fullpath = folders.Single();
                                    string newpath = Path.Combine(Fullpath, Filename + ".feature");
                                    int fileIndex = repository.Info.WorkingDirectory.Count();
                                    string file = newpath.Remove(0, fileIndex);
                                    var contents = "Feature:" + item.Title + "\n" + item.Description + "\n" + "@" + item.TagName + "\n"
                                    + "Scenario:" + item.ACriteriaName + "\n" + "\t" + item.GWT;
                                    File.WriteAllText(newpath, contents.Replace('*', ' '));
                                    repository.Index.Add(file);
                                    LibGit2Sharp.Commands.Stage(repository, newpath);
                                }
                            }
                        }
                    }

                }
            }

        }

        private void Update_File(int userstoryId, List<FileParameter> list, string repositoryUrl)
        {
            foreach (var item in list)
            {
                if (userstoryId == item.Id)
                {
                    DirectoryInfo ParentDirectory = new DirectoryInfo(repositoryUrl);
                    string FileName = (item.Title.Replace(" ", "") + ".feature");
                    var folder = ParentDirectory.GetFiles(FileName, SearchOption.AllDirectories).Select(t => t.FullName).ToList();
                    string Fullpath = folder.Single();
                    if (File.Exists(Fullpath))
                    {
                        using (StreamWriter sw = new StreamWriter(Fullpath))
                        {
                            sw.Write(string.Empty);
                            Console.WriteLine("Enter The new content in a file ");
                            string Content = Console.ReadLine();
                            sw.WriteLine(Content);
                            sw.Close();
                        }
                    }
                }
            }
        }

        private void Delete_File(int userstoryId, List<FileParameter> list, string repositoryUrl)
        {
            foreach (var item in list)
            {
                if (userstoryId == item.Id)
                {
                    DirectoryInfo ParentDirectory = new DirectoryInfo(repositoryUrl);
                    string FileName = (item.Title.Replace(" ", "") + ".feature");
                    var folder = ParentDirectory.GetFiles(FileName, SearchOption.AllDirectories).Select(t => t.FullName).ToList();
                    string Fullpath = folder.Single();
                    if (File.Exists(Fullpath))
                    {
                        File.Delete(Fullpath);
                    }
                }
            }
        }
        private void PushAndCommit(string username, string password, Repository repository)
        {

            var branch = "TEST_24";
            var BranchName = repository.CreateBranch(branch);
            repository.Branches.Update(BranchName,
                                b => b.Remote = "origin",
                                b => b.UpstreamBranch = BranchName.CanonicalName);
            Signature author = new Signature("Yashashree-Salunke", username, DateTime.Now);
            Signature committer = author;
            Commit commit = repository.Commit("file is modified", author, committer);  // Commit to the repository

            PushOptions options = new PushOptions();
            options.CredentialsProvider = new CredentialsHandler(
                (url, usernameFromUrl, types) =>
                    new UsernamePasswordCredentials()
                    {
                        Username = username,
                        Password = password
                    });
            repository.Network.Push(repository.Branches[branch], options);
        }

    }
}
