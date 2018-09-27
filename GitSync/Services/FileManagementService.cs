using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibGit2Sharp;

namespace GitSync.Services 
{
    public class FileManagementService : IFileManagementService
    {
        public void AddFile(int userstoryId, List<FileParameter> list, string repositoryUrl, Repository repository)
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

        public void DeleteFile(int userstoryId, List<FileParameter> list, string repositoryUrl)
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

        public void UpdateFile(int userstoryId, List<FileParameter> list, string repositoryUrl)
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
    }
}
