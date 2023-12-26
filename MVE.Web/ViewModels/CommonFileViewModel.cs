using System.Text.RegularExpressions;
using TCP.Core.Code;
using TCP.Core.Code.LIBS;
using TCP.Core.Models;
using TCP.Data.Models;
using TCP.Dto;

namespace TCP.Web.ViewModels
{
    public class CommonFileViewModel
    {
        public static void FileUpload(IFormFile fileObj, string FilesPath)
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

        public static dynamic GetNoOfTravellers()
        {
            int[] numbers = new int[151]; // Create an array to hold 151 numbers (0 to 150)

            for (int i = 1; i <= 150; i++)
            {
                numbers[i] = i; // Assign each number to the corresponding index in the array
            }
            return numbers;
        }

        public static User RegisterNewUser(RegisterViewModel model, IFormFile formFile = null)
        {
            User usermodel = new User();

            usermodel.FirstName = (model.FirstName ?? "").Trim();
            usermodel.LastName = (model.LastName ?? "").Trim();
            usermodel.Email = model.Email;
            usermodel.DateOfBirth = model.DateOfBirth;
            usermodel.SaltKey = PasswordEncryption.CreateSaltKey();
            var password = PasswordEncryption.GenerateRandomPassword();
            model.Password = password; //jb$93osH

            usermodel.EncryptedPassword = PasswordEncryption.GenerateEncryptedPassword(password, usermodel.SaltKey, "MD5");  //Encoding.UTF8.GetBytes(model.Password);

            usermodel.Address = model.Address?.Trim();
            usermodel.City = model.City?.Trim();
            usermodel.CountryId = model.CountryId;
            usermodel.MobilePhone = model.MobilePhone?.Trim();
            usermodel.Gender = model.Gender;
            usermodel.ZipCode = model.ZipCode?.Trim();
            usermodel.StateOrCounty = model.StateOrCounty;
            usermodel.IsEmailVerified = false;
            usermodel.IsDeleted = false;
            usermodel.IsActive = false;
            usermodel.EmailVerificationOtpExpired = DateTime.Now.AddHours(-24);
            usermodel.CreationOn = DateTime.UtcNow;
            usermodel.EmailVerificationToken = PasswordEncryption.GenerateEncryptedPassword(PasswordEncryption.GenerateRandomOTP(), usermodel.SaltKey, "MD5");
            usermodel.CreatedBy = 0;


            if (formFile?.FileName != null)
            {
                FileUpload(formFile, SiteKeys.UploadFilesCountry);
                usermodel.ImageName = formFile?.FileName;
            }
            else
            {
                usermodel.ImageName = "";
            }
            return usermodel;
        }


        //public static string GetFilePathByAdmin(UploadSection uploadSection, string ImageName, string OriginalImageName, string ImageExtension)
        //{
        //    string filePath = string.Empty;
        //    string returnFilePath = string.Empty;

        //    try
        //    {
        //        //UploadSection uploadSection;
        //        UploadSection us = (UploadSection)uploadSection;
        //        if (!string.IsNullOrWhiteSpace(ImageName))
        //        {
        //            string datetimeString = ImageName.Replace($"{Path.GetFileNameWithoutExtension(OriginalImageName)}_", "");
        //            datetimeString = Path.GetFileNameWithoutExtension(datetimeString);
        //            DateTime fileCreatedDateTime = new DateTime(Convert.ToInt64(datetimeString));
        //            returnFilePath = string.Concat(((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty),
        //                                           "/", SiteKeys.UploadDirectory, "/",
        //                                            us.GetEnumDescription(), "/",
        //                                            fileCreatedDateTime.Year, "/",
        //            fileCreatedDateTime.Month);

        //            if (!string.IsNullOrWhiteSpace(returnFilePath))
        //            {
        //                filePath = string.Concat(returnFilePath, "/", ImageName, ImageExtension);
        //                if (filePath != null)
        //                {
        //                    filePath = SiteKeys.ImageDomain + "/" + filePath;
        //                    returnFilePath = filePath;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //    }

        //    bool fileExists = CheckIfFileExists(returnFilePath, "");
        //    returnFilePath = fileExists == false ? SiteKeys.NoImagePath_Square : returnFilePath;

        //    return returnFilePath;
        //}

        public static string GetFilePathByAdmin(ref string RetImageName, string ImageName, UploadSection uploadSection, string OriginalImageName, string ImageExtension, string NoImagePath)
        {
            string filePath = string.Empty;
            string InnerfilePath = string.Empty;
            try
            {
                InnerfilePath = "";
                //UploadSection uploadSection;
                //UploadSection us = (UploadSection)imagesObj.ImageSection;
                UploadSection us = uploadSection;
                if (!string.IsNullOrWhiteSpace(ImageName))
                {
                    string datetimeString = ImageName.Replace($"{Path.GetFileNameWithoutExtension(OriginalImageName)}_", "");
                    datetimeString = Path.GetFileNameWithoutExtension(datetimeString);
                    DateTime fileCreatedDateTime = new DateTime(Convert.ToInt64(datetimeString));
                    InnerfilePath = string.Concat(((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty),
                                                   "/", SiteKeys.UploadDirectory, "/",
                                                    us.GetEnumDescription(), "/",
                                                    fileCreatedDateTime.Year, "/",
                                                    fileCreatedDateTime.Month);

                    if (!string.IsNullOrWhiteSpace(InnerfilePath))
                    {
                        filePath = string.Concat(InnerfilePath, "/", ImageName, ImageExtension);
                        if (filePath != null)
                        {
                            filePath = SiteKeys.ImageDomain + "/" + filePath;
                            InnerfilePath = filePath;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            bool fileExists = CheckIfFileExists(InnerfilePath, "");
            //ImageName = fileExists == false ? SiteKeys.NoImagePath : SiteKeys.UploadFilesTheme + OriginalImageName;
            ImageName = fileExists == false ? NoImagePath : InnerfilePath;


            return ImageName;
        }

        public static bool CheckIfFileExists(string filepath, string imageName)
        {
            try
            {
                // string filePath = Path.Combine(_hostingEnvironment.WebRootPath + filepath + imageName);
                string filePath = Path.Combine((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty + filepath + imageName);
                //return System.IO.File.Exists(filePath);

                //if (filePath != "")
                //{
                //    return true;
                //}
                //return false;
                string isdomainContains = filePath.Contains("http") || filePath.Contains("https") ? filePath : SiteKeys.ImageDomain + "/";
                //bool imageExists = ImageExists(isdomainContains + filePath);
                bool imageExists = ImageExists(filePath);


                return imageExists;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool CheckIfFileExistsForDestination(string filepath, string imageName)
        {
            try
            {
                // string filePath = Path.Combine(_hostingEnvironment.WebRootPath + filepath + imageName);
                string filePath = Path.Combine((SiteKeys.AdminImageWebroot_Path == null || SiteKeys.AdminImageWebroot_Path == "") ? SiteKeys.AdminImageWebroot_Path : string.Empty + filepath + imageName);
                //return System.IO.File.Exists(filePath);

                //if (filePath != "")
                //{
                //    return true;
                //}
                //return false;
                string isdomainContains = filePath.Contains("http") || filePath.Contains("https") ? filePath : SiteKeys.ImageDomain + "/";
                bool imageExists = ImageExists(isdomainContains + filePath);
                //bool imageExists = ImageExists(filePath);


                return imageExists;

            }
            catch (Exception ex)
            {
                return false;
            }
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
                var tagcontent = html.Contains("<");
                string plainText = Regex.Replace(html, "<.*?>", string.Empty);

                if (plainText.Length <= length)
                {
                    return html;
                }
                else
                {
                    string trimmedHtml = "";
                    string trimmedText = plainText.Substring(0, length);
                    if (tagcontent == true)
                        trimmedHtml = $"<p>{trimmedText}...</p>";
                    else
                        trimmedHtml = trimmedText + "..";


                    return trimmedHtml;
                }
            }
            else { return html; }
        }


        public partial class CustomThemeViewModel
        {
            public int Id { get; set; }

            public string Name { get; set; } = null!;
            public string ShortName { get; set; } = null!;

            public string? Description { get; set; }

            public string ImageName { get; set; } = null!;

        }
    }
}
