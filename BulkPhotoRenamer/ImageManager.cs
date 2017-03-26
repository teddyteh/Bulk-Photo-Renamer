using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BulkPhotoRenamer
{
    class ImageManager
    {
        private static Regex r = new Regex(":");
        public List<PhotoInfo> images { get; set; }

        public ImageManager()
        {
            images = new List<PhotoInfo>();
        }

        public bool add(PhotoInfo photo)
        {
            // Get the file name of the photo's file path
            string fileName = Path.GetFileNameWithoutExtension(photo.filePath);

            /* 
             * Get date taken or date created
             */
            // First check if the date taken property exists for the photo
            bool dateTakenExists = DateTakenExists(photo.filePath);
            DateTime dateTime = DateTime.MinValue;
            PhotoInfo.dtType dateTimeType;
            // Date taken will always have priority over date created
            if (dateTakenExists)
            {
                dateTime = GetDateTaken(photo.filePath);
                dateTimeType = PhotoInfo.dtType.Taken;
            }
            else
            {
                dateTime = GetFileCreated(photo.filePath);
                dateTimeType = PhotoInfo.dtType.Created;
            }

            // Get the extension
            string extension = Path.GetExtension(photo.filePath);

            // Check if a prefix is given
            bool prefixGiven = false;
            if (photo.prefix != null)
                prefixGiven = true;

            // Check if the add timestep option has been selected
            bool addTimestamp = false;
            if (photo.addTimestamp)
                addTimestamp = true;

            // Generate new file name
            string baseFileName, newFileName = "";
            if (prefixGiven)
            {
                if (addTimestamp)
                {
                    newFileName = photo.prefix + "_" + dateTime.ToString("yyyyMMdd_HHmmss") + extension; 
                }
                else if (!addTimestamp)
                {
                    newFileName = photo.prefix + "_" + dateTime.ToString("yyyyMMdd") + extension;
                }
            }
            else if (!prefixGiven)
            {
                if (addTimestamp)
                {
                    newFileName = dateTime.ToString("yyyyMMdd_HHmmss") + extension;
                }
                else if (!addTimestamp)
                {
                    newFileName = dateTime.ToString("yyyyMMdd") + extension;
                }
            }
            baseFileName = newFileName;

            // Account for duplicates
            int count = countDuplicate(newFileName);
            if (count > 0)
            {
                if (prefixGiven)
                {
                    if (addTimestamp)
                    {
                        newFileName = photo.prefix + "_" + dateTime.ToString("yyyyMMdd_HHmmss") + "_" + (count + 1) + extension;
                    }
                    else if (!addTimestamp)
                    {
                        newFileName = photo.prefix + "_" + dateTime.ToString("yyyyMMdd") + "_" + (count + 1) +  extension;
                    }
                }
                else if (!prefixGiven)
                {
                    if (addTimestamp)
                    {
                        newFileName = dateTime.ToString("yyyyMMdd_HHmmss") + "_" + (count + 1) + extension;
                    }
                    else if (!addTimestamp)
                    {
                        newFileName = dateTime.ToString("yyyyMMdd") + "_" + (count + 1) + extension;
                    }
                }
            }

            photo.originalFileName = fileName;
            photo.dateTime = dateTime;
            photo.dateTimeType = dateTimeType;
            photo.baseFileName = baseFileName;
            photo.newFileName = newFileName;

            images.Add(photo);

            return true;
        }

        public void clear(string[] filePath)
        {
            for (int i = 0; i < images.Count - 1; i++)
            {
                for (int y = 0; y < filePath.Length; y++)
                {
                    if (filePath[y].Equals(images[i].filePath))
                    {
                        images.RemoveAt(i);
                    }
                }
            }
        }

        public void clearAll()
        {
            images.Clear();
        }

        public void renameAll()
        {
            foreach (PhotoInfo i in images)
            {
                System.IO.File.Move(i.filePath, Path.GetDirectoryName(i.filePath) + "\\" + i.newFileName);
            }

            clearAll();
        }

        public int countDuplicate(string fileName)
        {
            int v = 0;

            foreach (PhotoInfo i in images)
            {
                if (i.baseFileName.Equals(fileName))
                    v++;
            }

            return v;
        }

        public static bool DateTakenExists(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (Image myImage = Image.FromStream(fs, false, false))
                {
                    var propertyExists = myImage.PropertyItems.Any(p => p.Id == 0x0112);

                    if (propertyExists)
                        return true;

                    return false;
                }
            }
        }

        public static DateTime GetDateTaken(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(0x132);

                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);

                return DateTime.Parse(dateTaken);
            }
        }

        public static DateTime GetFileCreated(string path)
        {
            return File.GetCreationTime(path);
        }
    }
}
