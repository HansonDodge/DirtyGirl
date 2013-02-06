using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Utils
{
    /// <summary>
    /// Helper class to identify file type by the file header, not file extension.
    /// </summary>
    public static class Detective
    {
        #region Constants

        // file headers are taken from here:
        //http://www.garykessler.net/library/file_sigs.html
        //mime types are taken from here:
        //http://www.webmaster-toolkit.com/mime-types.shtml

        // graphics
        public readonly static FileType JPEG = new FileType(new byte?[] { 0xFF, 0xD8, 0xFF }, "jpg", "image/jpeg");
        public readonly static FileType PNG = new FileType(new byte?[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, "png", "image/png");
        public readonly static FileType GIF = new FileType(new byte?[] { 0x47, 0x49, 0x46, 0x38, null, 0x61 }, "gif", "image/gif");
        
        // all the file types to be put into one list
        private readonly static List<FileType> Types = new List<FileType> { 
            JPEG, PNG, GIF};

        // number of bytes we read from a file
        private const int MaxHeaderSize = 560;  // some file formats have headers offset to 512 bytes

        #endregion

        #region Main Methods

        /// <summary>
        /// Read header of a file and depending on the information in the header
        /// return object FileType.
        /// Return null in case when the file type is not identified. 
        /// Throws Application exception if the file can not be read or does not exist
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>FileType or null not identified</returns>
        public static FileType GetFileType(this IEnumerable<byte> byteArray)
        {
            // read first n-bytes from the file
            Byte[] fileHeader = byteArray.Take(MaxHeaderSize).ToArray();

            // compare the file header to the stored file headers
            foreach (FileType type in Types)
            {
                int matchingCount = 0;
                for (int i = 0; i < type.Header.Length; i++)
                {
                    // if file offset is not set to zero, we need to take this into account when comparing.
                    // if byte in type.header is set to null, means this byte is variable, ignore it
                    if (type.Header[i] != null && type.Header[i] != fileHeader[i + type.HeaderOffset])
                    {
                        // if one of the bytes does not match, move on to the next type
                        matchingCount = 0;
                        break;
                    }
                    else
                    {
                        matchingCount++;
                    }
                }
                if (matchingCount == type.Header.Length)
                {
                    // if all the bytes match, return the type
                    return type;
                }
            }
            // if none of the types match, return null
            return null;
        }

        /// <summary>
        /// Gets the type of the file.
        /// </summary>
        /// <param name="postedFile">The posted file.</param>
        /// <returns></returns>
        public static FileType GetFileType(this HttpPostedFileBase postedFile)
        {
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(postedFile.InputStream))
            {
                fileData = binaryReader.ReadBytes(postedFile.ContentLength);
            }
            return GetFileType(fileData);
        }

        /// <summary>
        /// Gets the list of FileTypes based on list of extensions in Comma-Separated-Values string
        /// </summary>
        /// <param name="csv">The CSV String with extensions</param>
        /// <returns>List of FileTypes</returns>
        private static List<FileType> GetFileTypesByExtensions(String csv)
        {
            var extensions = csv.ToUpper().Replace(" ", "").Split(',');

            return Types.Where(type => extensions.Contains(type.Extension.ToUpper())).ToList();
        }

        #endregion

        #region isType functions

        /// <summary>
        /// Determines whether the specified file is of provided type
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="byteArray"></param>
        /// <param name="type">The FileType</param>
        /// <returns>
        ///   <c>true</c> if the specified file is type; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsType(this IEnumerable<byte> byteArray, FileType type)
        {
            FileType actualType = GetFileType(byteArray);

            if (null == actualType)
                return false;

            return (actualType.Equals(type));
        }

        private static bool IsType(this HttpPostedFileBase postedFile, FileType type)
        {
            FileType actualType = GetFileType(postedFile);

            if (null == actualType)
                return false;

            return (actualType.Equals(type));
        }


        /// <summary>
        /// Determines whether the specified file is JPEG image
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>
        ///   <c>true</c> if the specified file info is JPEG; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJpeg(this IEnumerable<byte> byteArray)
        {
            if (byteArray == null) throw new ArgumentNullException("byteArray");
            return byteArray.IsType(JPEG);
        }

        public static bool IsJpeg(this HttpPostedFileBase postedFile)
        {
            if (postedFile == null) throw new ArgumentNullException("postedFile");
            postedFile.InputStream.Position = 0;
            return postedFile.IsType(JPEG);
        }

        /// <summary>
        /// Determines whether the specified file is PNG.
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>
        ///   <c>true</c> if the specified file info is PNG; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPng(this IEnumerable<byte> byteArray)
        {
            if (byteArray == null) throw new ArgumentNullException("byteArray");
            return byteArray.IsType(PNG);
        }

        public static bool IsPng(this HttpPostedFileBase postedFile)
        {
            if (postedFile == null) throw new ArgumentNullException("postedFile");
            postedFile.InputStream.Position = 0;
            return postedFile.IsType(PNG);
        }

        /// <summary>
        /// Determines whether the specified file is GIF image
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns>
        ///   <c>true</c> if the specified file info is GIF; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGif(this IEnumerable<byte> byteArray)
        {
            if (byteArray == null) throw new ArgumentNullException("byteArray");
            return byteArray.IsType(GIF);
        }

        public static bool IsGif(this HttpPostedFileBase postedFile)
        {
            if (postedFile == null) throw new ArgumentNullException("postedFile");
            postedFile.InputStream.Position = 0;
            return postedFile.IsType(GIF);
        }

        #endregion
    }




}