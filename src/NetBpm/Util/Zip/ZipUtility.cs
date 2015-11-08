using ICSharpCode.SharpZipLib.Zip;
using NetBpm.Workflow.Definition;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetBpm.Util.Zip
{
    public class ZipUtility
    {
        /// <summary> reads the zipFile-InputStream and puts all entries in a Map, for which
        /// the keys are the path-names.
        /// </summary>
        /// <returns> a Map with the entry-path-names as keys and the byte-arrays as the contents.
        /// </returns>
        public static IDictionary<string, byte[]> ReadEntries(Stream processArchiveStream)
        {
            try
            {
                IDictionary<string, byte[]> entries = new Dictionary<string, byte[]>();
                ZipInputStream s = new ZipInputStream(processArchiveStream);
                ZipEntry entry;
                // extract the file or directory entry
                while ((entry = s.GetNextEntry()) != null)
                {
                    if (!entry.IsDirectory)
                    {
                        byte[] content = ZipStreamToByte(s);
                        entries.Add(entry.Name, content);
                    }
                }

                s.Close();
                return entries;
            }
            catch (IOException e)
            {
                throw new NpdlException("couldn't deploy process archive, the processArchiveBytes do not seem to be a valid jar-file : " + e.Message, e);
            }
        }

        private static byte[] ZipStreamToByte(ZipInputStream s)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            while (true)
            {
                int size = 2048;
                byte[] data = new byte[2048];

                size = s.Read(data, 0, data.Length);
                if (size > 0)
                {
                    bw.Write(data, 0, size);
                }
                else
                {
                    break;
                }
            }
            bw.Flush();
            // deletes leading bytes in memStream
            ms.SetLength(ms.Position);
            ms.Seek(0, SeekOrigin.Begin);

            byte[] content = new Byte[ms.Length];
            ms.Read(content, 0, (int)ms.Length);
            return content;
        }
    }
}
