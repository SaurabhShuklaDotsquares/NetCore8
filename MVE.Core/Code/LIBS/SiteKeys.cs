using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVE.Core.Code.LIBS
{
    public class SiteKeys
    {
        private static IConfigurationSection _configuration;

        public static void Configure(IConfigurationSection configuration)
        {
            _configuration = configuration;
        }
        public static bool IsLive => Convert.ToBoolean(_configuration["AppSetting:IsLive"]);
        //public static string Domain => _configuration["Domain"].ToLower();
        public static string Domain => _configuration["Domain"];
        public static string ImageDomain => _configuration["ImageDomain"];
        public static string FrontDomain => _configuration["FrontDomain"];

        public static string MailBCC => _configuration["MailBCC"];
        public static string AdminEmailAddress => _configuration["AdminEmailAddress"];
        public static string AdminDisplayName => _configuration["AdminDisplayName"];
        public static string SMTPUserName => _configuration["SMTPUserName"];
        public static string SMTPUserPassword => _configuration["SMTPUserPassword"];
        public static string SmtpServer => _configuration["SmtpServer"];
        public static string SmtpPort => _configuration["SmtpPort"];
        public static string SmtpSsl => _configuration["SmtpSsl"];
        public static string DefaultFromEmail => _configuration["DefaultFromEmail"];
        public static string DefaultFromName => _configuration["DefaultFromName"];
        public static string DateFormat => _configuration["DateFormat"];
        public static string DateFormatWithoutTime => _configuration["DateFormatWithoutTime"];
        public static string DateFormatWithMonthName => _configuration["DateFormatWithMonthName"];
        public static string DateFormatCalenderWithoutTime => _configuration["DateFormatCalenderWithoutTime"];
        public static string FilesPath => _configuration["FilesPath"];
        //public static string UploadRootDirectory => _configuration["UploadRootDirectory"];
        public static string UploadDirectory => _configuration["UploadDirectory"];
        public static string UploadFiles => _configuration["UploadFiles"];
        public static string UploadFilesTheme => _configuration["UploadFilesTheme"];
        public static string UploadFilesCountry => _configuration["UploadFilesCountry"];
        public static string UploadFilesCategory => _configuration["UploadFilesCategory"];
        public static string UploadFilesBanner => _configuration["UploadFilesBanner"];
        public static string UploadFilesQueryReply => _configuration["UploadFilesQueryReply"];


        public static string FromName_subject => _configuration["FromName_subject"];
        public static string NoImagePath => _configuration["NoImagePath"];
        public static string NoImagePath_Square => _configuration["NoImagePath_Square"];
        public static string AdminImageWebroot_Path => _configuration["AdminImageWebroot_Path"];
        public static string UploadFilesUsers => _configuration["UploadFilesUsers"];
        public static string AdminEmailLogs => _configuration["AdminEmailLogs"]; 
        public static string UploadFilesDestination => _configuration["UploadFilesDestination"];

        public static string Stripekey => _configuration["Stripekey"];
        public static string StripeCurrency => _configuration["StripeCurrency"];
        public static string UploadFilesNotifications => _configuration["UploadFilesNotifications"];


        public static string AdminPageLimit => _configuration["AdminPageLimit"];
        public static string SupportEmail => _configuration["SupportEmail"];
        public static string SupportMobile => _configuration["SupportMobile"];
        public static string LogoImageName => _configuration["LogoImageName"];
        public static string LogoImageNameDark => _configuration["LogoImageNameDark"];

    }
}
