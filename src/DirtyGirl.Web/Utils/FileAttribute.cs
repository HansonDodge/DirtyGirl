using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Utils
{
    public class FileAttribute : ValidationAttribute
    {
        public int MaxContentLength = int.MaxValue;
        public string[] AllowedFileExtensions;
        public string[] AllowedContentTypes;

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;

            //this should be handled by [Required]
            if (file == null)
                return true;

            if (file.ContentLength > MaxContentLength)
            {
                float maxSize = MaxContentLength / 1024;
                string sizeUnit;
                if (maxSize > 1024)
                {
                    maxSize = maxSize / 1024;
                    sizeUnit = "MB";
                }
                else
                {
                    sizeUnit = "KB";
                }
                ErrorMessage = String.Format("File is too large, maximum allowed is: {0}{1}", maxSize, sizeUnit);
                return false;
            }

            if (AllowedFileExtensions != null)
            {
                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                    ErrorMessage = "Please upload file of type: " + string.Join(", ", AllowedFileExtensions);
                    return false;
                }
            }

            if (AllowedContentTypes != null)
            {
                if (!AllowedContentTypes.Contains(file.ContentType))
                {
                    ErrorMessage = "Please upload file of type: " + string.Join(", ", AllowedContentTypes);
                    return false;
                }
            }

            return true;
        }
    }
}