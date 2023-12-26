using Microsoft.EntityFrameworkCore.Diagnostics;
using TCP.Data.Models;

namespace TCP.Web.ViewModels
{
    public class QuoteFeedbackViewModel
    {
        public long? QuoteForEnqueryId { get; set; }
        public string FeedbackForEnqueryTypeName { get; set; }

        public int? QuoteVersion { get; set; }

        public int? QuoteStatusId { get; set; }

        //public bool IsQuoteVerified { get; set; }

        //public DateTime? QuoteVerificationOtpExpired { get; set; }

        //public string? QuoteVerificationToken { get; set; }

        public long? ParentPackageId { get; set; }

        public string Email { get; set; }
        public long UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }

        public string PackageName { get; set; }
        public long PackageId { get; set; }



        //Saveing data properties
        public string Subject { get; set; }

        public long? PkgIdForQuote { get; set; }
        //public string FeedbackDescription { get; set; } = null!;

        public int? EnqueryTypeForQuote { get; set; }

        public string? PackageUrlForQuote { get; set; }
        public string? EmailContentForQuote { get; set; }

        public long? CreatedByQuote { get; set; }

        public DateTime? CreatedOnQuote { get; set; }

        public long? ModifiedByQuote { get; set; }

        public DateTime? ModifiedOnQuote { get; set; }

        public int? QuteFeedbackStatus { get; set; }

        public string EmailContent { get; set; }

        public bool IsQuoteFullfillRequirement { get; set; }
        public bool IsItRequiredMoreChanges { get; set; }
        public string IsQuoteFullfillRequirementVal { get; set; }
        public string IsItRequiredMoreChangesVal { get; set; }

        public string? PreviousEmailContentForQuote { get; set; }
        //public string? PreviousFeedbackDescriptionForQuote { get; set; }



        public QuoteFeedbackForModel LastQuoteFeedbackForModel { get; set; }
        public List<QuoteFeedbackForModel> _QuoteFeedbackForModel { get; set; }
    }
    public class QuoteFeedbackForModel
    {

        public long Id { get; set; }

        public long PackageId { get; set; }

        public int? FeedbackForEnqueryType { get; set; }

        //public string FeedbackDescription { get; set; } = null!;

        public string IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public long CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public long? ModifiedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? PkgIdForQuote { get; set; }

        public int? EnqueryTypeForQuote { get; set; }

        public string? PackageUrlForQuote { get; set; }
        public string? EmailContentForQuote { get; set; }

        public long? CreatedByQuote { get; set; }

        public DateTime? CreatedOnQuote { get; set; }

        public long? ModifiedByQuote { get; set; }

        public DateTime? ModifiedOnQuote { get; set; }

        public int? QuteFeedbackStatus { get; set; }

        //public virtual Package Package { get; set; } = null!;

        //public virtual Package? PkgIdForQuoteNavigation { get; set; }

        public string? PreviousEmailContentForQuote { get; set; }
        //public string? PreviousFeedbackDescriptionForQuote { get; set; }
    }
}
