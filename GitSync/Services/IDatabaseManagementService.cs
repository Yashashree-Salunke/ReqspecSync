using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GitSync.Services 
{
    public interface IDatabaseManagementService
    {
        List<FileParameter> GetValuesFromDb(string connectionString, int userstoryId);
    }
}
