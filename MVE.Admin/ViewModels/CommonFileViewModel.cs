using System.Text.RegularExpressions;
using MVE.Core.Code.LIBS;

namespace MVE.Admin.ViewModels
{
    public class CommonFileViewModel
    {
        public static void FileUpload(IFormFile fileObj,string FilesPath)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory() + "/wwwroot/" + FilesPath);

            //create folder if not exist
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //get file extension
            FileInfo fileInfo = new FileInfo(DateTime.Now.Ticks + fileObj.FileName);
            string fileEx = fileInfo.Extension;
            string fileNameWithPath = Path.Combine(path, fileObj.FileName);

            using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
            {
                fileObj.CopyTo(stream);
            }
        }
        public static string GetFullName(string firstName, string lastName)
        {
            return firstName + " " + lastName;
        }

        public static string SplitString(string input, int length)
        {
            string subString;
            if (!string.IsNullOrEmpty(input) && length > 0)
            {
                if (input.Length <= length)
                {
                    return input;
                }
                else
                {
                    subString = input.Substring(0, length);
                    return $"{subString}...";
                }
            }
            else { return input; }

        }

        public static string TrimHtmlText(string html, int length)
        {
            if (!string.IsNullOrEmpty(html) && length > 0)
            {
                string plainText = Regex.Replace(html, "<.*?>", string.Empty);

                if (plainText.Length <= length)
                {
                    return html;
                }
                else
                {
                    string trimmedText = plainText.Substring(0, length);
                    string trimmedHtml = $"<p>{trimmedText}...</p>";

                    return trimmedHtml;
                }
            }
            else { return html; }
        }
        public static bool CheckIfFileExists(string filepath, string imageName)
        {
            try
            {
                // string filePath = Path.Combine(_hostingEnvironment.WebRootPath + filepath + imageName);
                string filePath = Path.Combine((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty + filepath + imageName);
                //return System.IO.File.Exists(filePath);
                if (filePath != "")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool IsFileExistsOnLocation(string filepath)
        {
            try
            {
                bool imageExists = ImageExists(filepath);
                return imageExists;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<bool> DeleteFileFromLocation(string sitekeyFilePath, string fileName)
        {

            string physicalPath = SiteKeys.AdminImageWebroot_Path + "/" + sitekeyFilePath + "/" + fileName;

            // Check if the file exists
            if (!System.IO.File.Exists(physicalPath))
            {
                return false;
            }

            // Delete the file
            System.IO.File.Delete(physicalPath);
            return true;
        }
        static bool ImageExists(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.GetAsync(imageUrl).Result;
                    return response.IsSuccessStatusCode;
                }
                catch (HttpRequestException)
                {
                    // An exception occurred, indicating that the image does not exist or there was an issue with the request.
                    return false;
                }
            }
        }
    }
    public class EnqViewModel
    {
        public int EnquiryID { get; set; }
        public string UserName { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserMobileNumber { get; set; }
        public DateTime EnquiryReceivedOn { get; set; }
        public string EnquiryStatus { get; set; }
        public string GetStatus { get; set; }
    }
}
