using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVE.Core.Code.LIBS;
using MVE.Dto;

namespace MVE.Core.Code.Extensions
{
    public static class FileOperationExtensions
    {

        private static FileUploadDTO ComposeViewData(IFormFile formFile, UploadSection uploadSection)
        {
            FileUploadDTO fileUpload = new FileUploadDTO()
            {
                FileExtension = Path.GetExtension(formFile.FileName),
                FileOriginalName = Path.GetFileName(formFile.FileName),
                Name = $"{Path.GetFileNameWithoutExtension(formFile.FileName)}_{DateTime.UtcNow.Ticks}",
                FileSize = formFile.Length,
                SectionId = (int)uploadSection
            };
            return fileUpload;
        }

        private static FileUploadDTO ComposeViewData(FileUploadDTO fileUpload)
        {
            FileUploadDTO newFileUpload = new FileUploadDTO()
            {
                FileExtension = Path.GetExtension(fileUpload.Name),
                FileOriginalName = Path.GetFileName(fileUpload.Name),
                Name = $"{Path.GetFileNameWithoutExtension(fileUpload.Name)}_{DateTime.UtcNow.Ticks}",
                FileSize = fileUpload.FileSize,
                SectionId = fileUpload.SectionId,
                FileStreams = fileUpload.FileStreams,
                EntityId = fileUpload.EntityId,
                FId = fileUpload.FId,
                FilePath = fileUpload.FilePath,
                ImageOrder = fileUpload.ImageOrder
            };
            return newFileUpload;
        }

        private static void SaveFile(IFormFile formFile, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, FileUploadDTO fileUpload)
        {
            string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileUpload.Name, fileUpload.FileOriginalName);

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = string.Concat(filePath, "/", fileUpload.Name, fileUpload.FileExtension);


                using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(filestream);
                }
            }
        }
        private static void SaveFileByIFormFile(IFormFile formFile, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, FileUploadDTO fileUpload)
        {
            string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileUpload.Name, fileUpload.FileOriginalName);

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                filePath = string.Concat(filePath, "/", fileUpload.Name, fileUpload.FileExtension);


                using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                {
                    formFile.CopyTo(filestream);
                }
            }
        }

        public static string GetUploadDirectoryPath(IHostingEnvironment hostingEnvironment, UploadSection uploadSection, string fileName, string fileOriginalName)
        {
            string filePath = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    string datetimeString = fileName.Replace($"{Path.GetFileNameWithoutExtension(fileOriginalName)}_", "");
                    datetimeString = Path.GetFileNameWithoutExtension(datetimeString);
                    DateTime fileCreatedDateTime = new DateTime(Convert.ToInt64(datetimeString));
                    filePath = string.Concat((hostingEnvironment != null ? hostingEnvironment.WebRootPath : string.Empty),
                                                   "/", SiteKeys.UploadDirectory, "/",
                                                    uploadSection.GetEnumDescription(), "/",
                                                    fileCreatedDateTime.Year, "/",
                                                    fileCreatedDateTime.Month);
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }

        private static void SaveFile(IHostingEnvironment hostingEnvironment, FileUploadDTO fileUpload)
        {
            try
            {
                string filePath = GetUploadDirectoryPath(hostingEnvironment, (UploadSection)fileUpload.SectionId, fileUpload.Name, fileUpload.FileOriginalName);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    filePath = string.Concat(filePath, "/", fileUpload.Name, fileUpload.FileExtension);

                    string convert = fileUpload.FileStreams.Replace("data:image/png;base64,", String.Empty)
                        .Replace("data:image/gif;base64,", String.Empty)
                        .Replace("data:image/jpg;base64,", String.Empty)
                        .Replace("data:image/jpeg;base64,", String.Empty)
                        .Replace("data:video/mp4;base64,", String.Empty);



                    // image = LazZiya.ImageResize.ImageResize.ScaleByWidth(image, 2000);

                    byte[] byteBuffer = Convert.FromBase64String(convert);
                    //byteBuffer = byteBuffer.Where(val => val != 0).ToArray();
                    //MemoryStream memoryStream = new MemoryStream(byteBuffer);

                    //byte[] bytes = Convert.FromBase64String(fileUpload.FileStreams);
                    //FileStream pngStream = null;
                    //(new MemoryStream(bytes)).CopyTo(pngStream);
                    ////using (FileStream pngStream = new FileStream(filePath, FileMode.Create))

                    if (fileUpload.FileExtension == ".mp4")
                    {
                        File.WriteAllBytes(filePath, byteBuffer);
                    }
                    else
                    {
                        using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                        {
                            var imageFromStream = Image.FromStream(memoryStream);
                            var imageMemoryStream = new MemoryStream(byteBuffer);

                            ImageFormat imgFormat = ImageFormat.Jpeg;

                            if (fileUpload.FileExtension.ToLower() == ".png")
                            {
                                imgFormat = ImageFormat.Png;
                            }
                            else if (fileUpload.FileExtension.ToLower() == ".gif")
                            {
                                imgFormat = ImageFormat.Gif;
                            }
                            else if (fileUpload.FileExtension.ToLower() == ".jpg")
                            {
                                imgFormat = ImageFormat.Jpeg;
                            }
                            else if (fileUpload.FileExtension.ToLower() == ".jpeg")
                            {
                                imgFormat = ImageFormat.Jpeg;
                            }

                            var width = imageFromStream.Width;

                            if (width > 2000)
                            {
                                width = 2000;
                            }
                            if (fileUpload.FileExtension.ToLower() != ".gif")
                            {
                                imageFromStream = LazZiya.ImageResize.ImageResize.ScaleByWidth(imageFromStream, width);
                            }
                            var stream = new MemoryStream();
                            imageFromStream.Save(stream, imgFormat);
                            stream.Position = 0;

                            imageMemoryStream = stream;

                            using (var image = new Bitmap(imageMemoryStream))
                            {
                                image.Save(filePath);

                                //var resized = new Bitmap(image.Width, image.Height);


                                ////SaveImage(resized, image.Width, image.Height, image.Width, filePath);
                                //using (var graphics = Graphics.FromImage(resized))
                                //{
                                //    //ImageCodecInfo encoder = GetEncoder(imgFormat);

                                //    //System.Drawing.Imaging.Encoder QualityEncoder = System.Drawing.Imaging.Encoder.Quality;

                                //    //EncoderParameters encoderParameters = new EncoderParameters(1);

                                //    //EncoderParameter encoderParameter = new EncoderParameter(QualityEncoder, 80);
                                //    //encoderParameters.Param[0] = encoderParameter;

                                //    graphics.CompositingQuality = CompositingQuality.HighSpeed;
                                //    graphics.InterpolationMode = InterpolationMode.Low;
                                //    graphics.CompositingMode = CompositingMode.SourceCopy;
                                //    graphics.DrawImage(image, 0, 0, image.Width, image.Height);

                                //    resized.Save(filePath);
                                //}
                            }


                        }
                    }

                    //using (Image image = Image.FromStream(new MemoryStream(bytes)))
                    //{
                    //    image.Save("output.jpg", ImageFormat.Jpeg);  // Or Png
                    //}

                    //using (FileStream filestream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    formFile.CopyTo(filestream);
                    //}
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private static void SaveImage(Bitmap image, int maxWidth, int maxHeight, int quality, string filePath)
        {
            // Get the image's original width and height
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // To preserve the aspect ratio
            float ratioX = (float)maxWidth / (float)originalWidth;
            float ratioY = (float)maxHeight / (float)originalHeight;
            float ratio = Math.Min(ratioX, ratioY);

            // New width and height based on aspect ratio
            int newWidth = (int)(originalWidth * ratio);
            int newHeight = (int)(originalHeight * ratio);


            // Convert other formats (including CMYK) to RGB.
            Bitmap newImage = new Bitmap(newWidth, newHeight, PixelFormat.Format24bppRgb);


            // Draws the image in the specified size with quality mode set to HighQuality
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            // Get an ImageCodecInfo object that represents the JPEG codec.
            ImageCodecInfo imageCodecInfo = GetEncoderInfo(ImageFormat.Jpeg);

            // Create an Encoder object for the Quality parameter.
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object. 
            EncoderParameters encoderParameters = new EncoderParameters(1);


            // Save the image as a JPEG file with quality level.
            EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
            encoderParameters.Param[0] = encoderParameter;
            newImage.Save(filePath, imageCodecInfo, encoderParameters);
        }
        private static ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        public static string GetFilePath<T>(this T entity, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileNameProperty(entity), originalName(entity));

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                filePath = string.Concat(filePath, "/", fileNameProperty(entity), fileExtension(entity));
                if (hostingEnvironment == null)
                {
                    filePath = SiteKeys.ImageDomain + "/" + filePath;
                }
            }
            return filePath;
        }

        public static string GetGalleryImagePath<T>(this T entity, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileNameProperty(entity), originalName(entity));

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                filePath = string.Concat(filePath, "/", fileNameProperty(entity), fileExtension(entity));
                if (hostingEnvironment == null)
                {
                    filePath = SiteKeys.ImageDomain + "/" + filePath;
                }
            }
            return filePath;
        }


        public static string GetWebFilePath<T>(this T entity, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            try
            {
                string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileNameProperty(entity), originalName(entity));


                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    filePath = string.Concat(filePath, "/", fileNameProperty(entity), fileExtension(entity));
                    if (hostingEnvironment == null)
                    {
                        filePath = SiteKeys.ImageDomain + "/" + filePath;
                    }
                }
                return filePath;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static List<string> GetFilePath<T>(this List<T> entities, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            List<string> files = new List<string>();
            foreach (var entity in entities)
            {
                files.Add(entity.GetFilePath<T>(hostingEnvironment, uploadSection, fileNameProperty, fileExtension, originalName));
            }
            return files;
        }

        public static FileUploadDTO SaveFileToDirectory(this IFormFile formFile, IHostingEnvironment hostingEnvironment, UploadSection uploadSection)
        {
            FileUploadDTO uploadDetail = ComposeViewData(formFile, uploadSection);

            SaveFile(formFile, hostingEnvironment, uploadSection, uploadDetail);

            return uploadDetail;
        }
        public static FileUploadDTO SaveFileToDirectoryByIFormFile(this IFormFile formFile, IHostingEnvironment hostingEnvironment)
        {
            FileUploadDTO uploadDetail = ComposeViewData(formFile, UploadSection.PackageDetailActivity);

            SaveFileByIFormFile(formFile, hostingEnvironment, UploadSection.PackageDetailActivity, uploadDetail);

            return uploadDetail;
        }
        public static FileUploadDTO SaveFileToDirectory(this FileUploadDTO entity, IHostingEnvironment hostingEnvironment)
        {
            FileUploadDTO uploadDetail = ComposeViewData(entity);

            SaveFile(hostingEnvironment, uploadDetail);

            return uploadDetail;
        }


        //public static List<FileUploadDTO> SaveFileToDirectory<T>(this IFormFile formFile, IWebHostEnvironment hostingEnvironment, UploadSection uploadSection, List<T> entities, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        //{
        //    List<FileUploadDTO> fileUploads = new List<FileUploadDTO>();
        //    foreach (var entity in entities)
        //    {
        //        FileUploadDTO uploadDetail = formFile.SaveFileToDirectory(hostingEnvironment, uploadSection, entity, fileNameProperty, fileExtension, originalName);

        //        fileUploads.Add(uploadDetail);
        //    }

        //    return fileUploads;
        //}

        public static void DeleteFileFromDirectory<T>(this T entity, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            string filePath = entity.GetFilePath(hostingEnvironment, uploadSection, fileNameProperty, fileExtension, originalName);

            FileInfo fileInfo = new FileInfo(filePath);
            fileInfo.Delete();
        }

        public static void DeleteFileFromDirectory<T>(this List<T> entities, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            foreach (var entity in entities)
            {
                entity.DeleteFileFromDirectory(hostingEnvironment, uploadSection, fileNameProperty, fileExtension, originalName);
            }
        }

        public static void MoveFileToDeleted<T>(this T entity, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileNameProperty(entity), originalName(entity));

            if (!string.IsNullOrWhiteSpace(filePath))
            {
                filePath = string.Concat(filePath, fileNameProperty(entity), fileExtension(entity));

                var fileMoveToPath = string.Concat(filePath, "/Deleted/", fileNameProperty(entity), fileExtension(entity));

                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    fileInfo.MoveTo(fileMoveToPath);
                }
            }
        }

        public static void MoveFileToDeleted<T>(this List<T> entities, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            foreach (var entity in entities)
            {
                entity.MoveFileToDeleted(hostingEnvironment, uploadSection, fileNameProperty, fileExtension, originalName);
            }
        }

        public static void MoveBackFileToOriginalDirectory<T>(this T entity, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            string filePath = GetUploadDirectoryPath(hostingEnvironment, uploadSection, fileNameProperty(entity), originalName(entity));

            filePath = string.Concat(filePath, "/Deleted/", fileNameProperty(entity), fileExtension(entity));

            var fileMoveToPath = string.Concat(filePath, fileNameProperty(entity), fileExtension(entity));

            FileInfo fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                fileInfo.MoveTo(fileMoveToPath);
            }
        }

        public static void MoveBackFileToOriginalDirectory<T>(this List<T> entities, IHostingEnvironment hostingEnvironment, UploadSection uploadSection, Func<T, string> fileNameProperty, Func<T, string> fileExtension, Func<T, string> originalName)
        {
            foreach (var entity in entities)
            {
                entity.MoveFileToDeleted(hostingEnvironment, uploadSection, fileNameProperty, fileExtension, originalName);
            }
        }

        public static T ComposeEntityData<T>(this FileUploadDTO fileUpload, T entity, Func<T, string, string> fileNameProperty, Func<T, string, string> fileExtension, Func<T, string, string> originalName, Func<T, Int64, Int64> fileSize, Func<T, int, int> uploadSection = null)
        {
            fileNameProperty(entity, fileUpload.Name);
            fileExtension(entity, fileUpload.FileExtension);
            originalName(entity, fileUpload.FileOriginalName);
            fileSize(entity, fileUpload.FileSize);

            if (uploadSection != null)
            {
                uploadSection(entity, fileUpload.SectionId);
            }

            return entity;
        }
    }
}
