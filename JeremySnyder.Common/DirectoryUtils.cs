/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.IO;

namespace JeremySnyder.Common
{
    public static class DirectoryUtils
    {
        public static void CreateDirectoryIfNotExists(string path)
        {
            var dirs = path.Split("/");
            var thisDir = string.Empty;
            
            // The last one will be the file name or file naming pattern and we don't want that as a directory
            foreach (var dir in dirs)
            {
                if (string.IsNullOrWhiteSpace(dir))
                {
                    continue;
                }
                
                thisDir = $"{thisDir}/{dir}";

                if (!Directory.Exists(thisDir))
                {
                    try
                    {
                        Directory.CreateDirectory(thisDir);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception($"Failed to create the logging directory:{Environment.NewLine}[{exception.Message}]", exception);
                    }
                }
            }
        }
    }
}
