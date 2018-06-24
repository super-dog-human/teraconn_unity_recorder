using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;

public class ZipUtility
{
    private const int utf8_codepage = 65001;
    public static void UnzipFromStream (System.IO.Stream stream, string outDir)
    {
        ZipConstants.DefaultCodePage = utf8_codepage;
        ZipInputStream zipInputStream = new ZipInputStream(stream);
        ZipEntry zipEntry = zipInputStream.GetNextEntry();
        while (zipEntry != null) {
            string entryFileName = zipEntry.Name;
            System.Diagnostics.Debug.WriteLine(entryFileName);
            byte[] buffer = new byte[4096];	// 4K is optimum

            string fullZipToPath = Path.Combine(outDir, entryFileName);
            string directoryName = Path.GetDirectoryName(fullZipToPath);
            if (directoryName.Length > 0) Directory.CreateDirectory(directoryName);

            string fileName = Path.GetFileName(fullZipToPath);
            if (fileName.Length == 0)
            {
                zipEntry = zipInputStream.GetNextEntry();
                continue;
            }

            using (FileStream streamWriter = File.Create(fullZipToPath)) {
                StreamUtils.Copy(zipInputStream, streamWriter, buffer);
            }
            zipEntry = zipInputStream.GetNextEntry();
        }
    }
}