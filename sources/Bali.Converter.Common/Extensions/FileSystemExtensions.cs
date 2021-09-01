namespace Bali.Converter.Common.Extensions
{
    using System;
    using System.IO;

    public static class FileSystemExtensions
    {
        public static bool WaitForFile(this FileInfo file)
        {
            int numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    using var fs = new FileStream(file.FullName,
                                                  FileMode.Open, FileAccess.ReadWrite,
                                                  FileShare.None, 100);
                    fs.ReadByte();

                    break;
                }
                catch (Exception)
                {
                    if (numTries > 10)
                    {
                        return false;
                    }

                    System.Threading.Thread.Sleep(500);
                }
            }

            return true;
        }

        public static void SafeCreate(this FileInfo file)
        {
            if (!file.Exists)
            {
                file.Create();
            }
        }

        public static void CreateDirectory(this FileInfo file)
        {
            if (file.Directory is {Exists: false})
            {
                file.Directory.Create();
            }
        }

        public static void SafeDelete(this FileSystemInfo fs)
        {
            if (fs.Exists)
            {
                fs.Delete();
            }
        }
    }
}
