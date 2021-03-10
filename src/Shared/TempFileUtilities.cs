// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.Build.Shared.FileSystem;

namespace Microsoft.Build.Shared
{
    /// <summary>
    /// This class contains utility methods for file IO.
    /// It is in a separate file so that it can be selectively included into an assembly.
    /// </summary>
    internal static partial class FileUtilities
    {
        /// <summary>
        /// Generates a unique directory name in the temporary folder.
        /// Caller must delete when finished.
        /// </summary>
        /// <param name="createDirectory"></param>
        /// <param name="subfolder"></param>
        internal static string GetTemporaryDirectory(bool createDirectory = true, string subfolder = null)
        {
            string temporaryDirectory = Path.Combine(Path.GetTempPath(), "Temporary" + Guid.NewGuid().ToString("N"), subfolder ?? string.Empty);

            if (createDirectory)
            {
                Directory.CreateDirectory(temporaryDirectory);
            }

            return temporaryDirectory;
        }

        /// <summary>
        /// Generates a unique temporary file name with a given extension in the temporary folder.
        /// File is guaranteed to be unique.
        /// Extension may have an initial period.
        /// File will NOT be created.
        /// May throw IOException.
        /// </summary>
        internal static string GetTemporaryFileName(string extension)
        {
            return GetTemporaryFile(extension: extension,
                                    directory: null,
                                    createFile: false);
        }

        /// <summary>
        /// Generates a unique temporary file name with a given extension in the temporary folder.
        /// If no extension is provided, uses ".tmp".
        /// File is guaranteed to be unique.
        /// Caller must delete it when finished.
        /// </summary>
        internal static string GetTemporaryFile()
        {
            return GetTemporaryFile(extension: ".tmp");
        }

        /// <summary>
        /// Generates a unique temporary file name with a given extension in the temporary folder.
        /// File is guaranteed to be unique.
        /// Extension may have an initial period.
        /// Caller must delete it when finished.
        /// May throw IOException.
        /// </summary>
        internal static string GetTemporaryFile(string extension)
        {
            return GetTemporaryFile(extension: extension,
                                    directory: null,
                                    createFile: true,
                                    subfolder: null);
        }

        /// <summary>
        /// Creates a file with unique temporary file name with a given extension in the specified folder.
        /// File is guaranteed to be unique.
        /// Extension may have an initial period.
        /// If folder is null, a folder named MSBuild will be created inside the temp directory.
        /// Caller must delete it when finished.
        /// May throw IOException.
        /// </summary>
        internal static string GetTemporaryFile(string extension, string directory = null, bool createFile = true, string subfolder = null)
        {
            ErrorUtilities.VerifyThrowArgumentLengthIfNotNull(directory, nameof(directory));
            ErrorUtilities.VerifyThrowArgumentLength(extension, nameof(extension));

            if (extension[0] != '.')
            {
                extension = '.' + extension;
            }

            try
            {
                directory ??= GetTemporaryDirectory(true, subfolder);

                string file = Path.Combine(directory, $"tmp{Guid.NewGuid():N}{extension}");

                ErrorUtilities.VerifyThrow(!FileSystems.Default.FileExists(file), "Guid should be unique");

                if (createFile)
                {
                    File.WriteAllText(file, string.Empty);
                }

                return file;
            }
            catch (Exception ex) when (ExceptionHandling.IsIoRelatedException(ex))
            {
                throw new IOException(ResourceUtilities.FormatResourceStringStripCodeAndKeyword("Shared.FailedCreatingTempFile", ex.Message), ex);
            }
        }

        internal static void CopyDirectory(string source, string dest)
        {
            Directory.CreateDirectory(dest);

            DirectoryInfo sourceInfo = new DirectoryInfo(source);
            foreach (var fileInfo in sourceInfo.GetFiles())
            {
                string destFile = Path.Combine(dest, fileInfo.Name);
                fileInfo.CopyTo(destFile);
            }
            foreach (var subdirInfo in sourceInfo.GetDirectories())
            {
                string destDir = Path.Combine(dest, subdirInfo.Name);
                CopyDirectory(subdirInfo.FullName, destDir);
            }
        }

        public class TempWorkingDirectory : IDisposable
        {
            public string Path { get; }

            public TempWorkingDirectory(string sourcePath, [CallerMemberName] string name = null)
            {
                Path = name == null
                    ? GetTemporaryDirectory()
                    : System.IO.Path.Combine(System.IO.Path.GetTempPath(), name);

                if (FileSystems.Default.DirectoryExists(Path))
                {
                    Directory.Delete(Path, true);
                }

                CopyDirectory(sourcePath, Path);
            }

            public void Dispose()
            {
                Directory.Delete(Path, true);
            }
        }
    }
}
