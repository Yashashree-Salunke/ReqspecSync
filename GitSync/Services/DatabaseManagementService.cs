using System;
using System.Collections.Generic;
using System.Data.SqlClient;


namespace GitSync.Services
{
    public class DatabaseManagementService : IDatabaseManagementService
    {
        public List<FileParameter> GetValuesFromDb(string connectionString, int userstoryId)
        {
            List<FileParameter> Newlist = new List<FileParameter>();
            SqlConnection cons = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand("SELECT u.Id,u.ParentId,u.RootId,u.Title,u.HasChildren,u.Description,a.Title AS A_Title,a.Gwt,t.Title AS T_Title FROM UserStories AS u JOIN AcceptanceCriterias AS a ON a.UserstoryId =u.Id JOIN UserstoryTags as st ON st.UserstoryId=u.Id JOIN Tags AS t ON t.Id=st.TagId WHERE u.Id = @StoryId");
            command.Parameters.AddWithValue("@StoryId", userstoryId);
            command.Connection = cons;
            command.Connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            FileParameter p = new FileParameter();
            while (reader.Read())
            {
                p.Id = (int)reader["Id"];
                p.HasChild = (bool)reader["HasChildren"];
                p.ParentId = reader["ParentId"] == DBNull.Value ? 0 : (int)reader["ParentId"];
                p.RootId = reader["RootId"] == DBNull.Value ? 0 : (int)reader["RootId"];
                p.Title = (string)reader["Title"];
                p.Description = (string)reader["Description"];
                p.ACriteriaName = (string)reader["A_Title"];
                p.GWT = (string)reader["Gwt"];
                p.TagName = (string)reader["T_Title"];
                Newlist.Add(p);
            }
            reader.Close();
            return Newlist;
        }

    }
}

