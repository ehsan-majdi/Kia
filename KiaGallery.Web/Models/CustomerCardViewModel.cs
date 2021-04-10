using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class CustomerCardViewModel
    {
        public int? id { get; set; }
        public string customerCode { get; set; }
        public long price { get; set; }
        public string branch { get; set; }
    }
    public class CustomerCardSearchViewModel
    {
        public string word { get; set; }
        public int count { get; set; }
        public int page { get; set; }
    }
    public class UserInfoResponseViewModel
    {

        public string cariCode { get; set; }
        public string responseStatus { get; set; }
        public string jtoday { get; set; }
        public List<UserInfoResponseListViewModel> lstUser { get; set; }

    }

    public class LoadUserInfoViewModel
    {
        public int id { get; set; }
        public string responseStatus { get; set; }
        public string cariorcard { get; set; }
        public string gtoday { get; set; }
        public string servertime { get; set; }
        public string jtoday { get; set; }
        public string email { get; set; }
        public string birthDate { get; set; }
        public string ntCode { get; set; }
        public string fullName { get; set; }
        public string mobile { get; set; }
        public string telHome { get; set; }
        public string cariCode { get; set; }
        public string cardNumber { get; set; }
        public string cardNo { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string homeAddr { get; set; }
        public string area { get; set; }
        public DateTime? gBirthDate { get; set; }
        public DateTime? marriedDate { get; set; }
        public string persianMarriedDate { get; set; }
        public double pointPercent { get; set; }
        public string replica { get; set; }
        public string branchName { get; set; }

    }

    public class UserInfoRequestViewModel
    {
        public string adminusername { get; set; }
        public string adminpassword { get; set; }
        public string cariorcard { get; set; }
        public string cardNo { get; set; }
        public string mobile { get; set; }
    }

    public class UserInfoResponseListViewModel
    {
        public string Gtoday { get; set; }
        public string Servertime { get; set; }
        public string CariCode { get; set; }
        public string CardNo { get; set; }
        public string MerchantParent { get; set; }
        public string BirthDate { get; set; }
        public string NtCode { get; set; }
        public string IsLoyal { get; set; }
        public string Fullname { get; set; }
        public double DiscountPercent { get; set; }
        public double PointPercent { get; set; }
        public string ShetacRate { get; set; }
        public string ShetacAcceptor { get; set; }
        public string ShetacProvider { get; set; }
        public string Happy_merchant_shadow { get; set; }
        public string Happy_card_shadow { get; set; }
        public string Serial { get; set; }
        public string TerminalId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string Logo { get; set; }
        public string Frontimage { get; set; }
        public string Backgroundimage { get; set; }
        public string Categorylogo { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string homeAddr { get; set; }
    }
    public class CardReportRequestViewModel
    {
        public string adminusername { get; set; }
        public string adminpassword { get; set; }
        public string mobile { get; set; }
        public string cariorcard { get; set; }
        //public string password { get; set;}
        public string fDate { get; set; }
        public string tDate { get; set; }
        public string fromAmount { get; set; }
        public string toAmount { get; set; }
        public string operationType { get; set; }
        public string dcCode { get; set; }
        public string merchantName { get; set; }
        public string pageNumber { get; set; }
        public string pageSize { get; set; }
    }
    public class CardReportResponseViewModel
    {

        public long localBalance { get; set; }
        public string responseStatus { get; set; }
        public string localBalanceSeparator { get; set; }
        public double debitCount { get; set; }
        public double creditCount { get; set; }
        public double debitSum { get; set; }
        public double creditSum { get; set; }
        public string debitCountSeparator { get; set; }
        public string fullName { get; set; }
        public string totalTrnCount { get; set; }
        public string creditCountSeparator { get; set; }
        public string debitSumSeparator { get; set; }
        public string creditSumSeparator { get; set; }
        public double kiaPoint { get; set; }
        public double pointPercent { get; set; }
        public string level { get; set; }
        public string kiaLevel { get; set; }
        public List<CardReportListResponseViewModel> lstTransaction { get; set; }
    }
    public class CardReportListResponseViewModel
    {
        public string trnDate { get; set; }
        public string jDate { get; set; }
        public long kiapoint { get; set; }
        public string level { get; set; }
        public string persianTrnDate { get; set; }
        public double debitAmount { get; set; }
        public double creditAmount { get; set; }
        public string creditAmountSeparator { get; set; }
        public string debitAmountSeparator { get; set; }
        public string fullName { get; set; }
    }
    public class DebitRequestViewModel
    {
        /// <summary>
        /// نام کاربری ادمین
        /// </summary>
        public string adminusername { get; set; }
        /// <summary>
        /// کلمه عبور ادمین
        /// </summary>
        public string adminpassword { get; set; }
        /// <summary>
        /// شماره کارت/کد جاری/شماره موبایل
        /// </summary>
        public string card { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string cariorcard { get; set; }
        /// <summary>
        /// کلمه عبور
        /// </summary>
        //public string Password { get; set; }
        /// <summary>
        /// شناسه پذیرنده
        /// </summary>
        public string merchantid { get; set; }
        /// <summary>
        /// مبلغ
        /// </summary>
        public string amount { get; set; }
        /// <summary>
        /// کد ارجاع
        /// </summary>
        public string refcode { get; set; }
        /// <summary>
        /// شماره ارجاع
        /// </summary>
        public string rrn { get; set; }
        /// <summary>
        /// شناسه کانال
        /// </summary>
        public string channelid { get; set; }
        /// <summary>
        /// کد رهگیری
        /// </summary>
        public string stan { get; set; }
        /// <summary>
        /// کانال ورودی
        /// </summary>
        public string channelvalue { get; set; }
        /// <summary>
        /// تعداد اقساط
        /// </summary>
        public string installmentcount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public double pointPercent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userInfoId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userId { get; set; }
    }

    public class DebitResponseViewModel
    {
        public string responseStatus { get; set; }
        public string gtoday { get; set; }
        public string servertime { get; set; }
        public string jtoday { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
        public string birthDate { get; set; }
        public string ntCode { get; set; }
        public string balanceAmount { get; set; }
        public string amount { get; set; }
        public string cariCode { get; set; }
        public string trnBizKey { get; set; }
        public string trnRefCode { get; set; }
        public string fullname { get; set; }
        public string isLoyal { get; set; }
        public string shetakActualAmount { get; set; }
        public string shetakDiscountAmount { get; set; }
        public string shetakLoyalAmount { get; set; }
    }
    public class BalanceRequestViewModel
    {
        public string adminusername { get; set; }
        public string adminpassword { get; set; }
        public string card { get; set; }
        //public string password { get; set; }
        //public string merchantId { get; set; }

    }

    public class BalanceResponseViewModel
    {
        public string responseStatus { get; set; }
        public string gtoday { get; set; }
        public string servertime { get; set; }
        public string balanceAmount { get; set; }
        public string isLoyal { get; set; }
    }

    public class UpdateUserInfoRequestViewModel
    {
        public string adminusername { get; set; }
        public string adminpassword { get; set; }
        public string cariorcard { get; set; }
        public string cardNumber { get; set; }
        public string cardNo { get; set; }
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string mobileNumber { get; set; }
        public string jbirthDate { get; set; }
        public string ntCode { get; set; }
        public string telHome { get; set; }
        public string telWork { get; set; }
        public string homeAddr { get; set; }
        public string area { get; set; }
        public string persianMarriedDate { get; set; }
        public string point { get; set; }
        public bool gift { get; set; }
        public string productCode { get; set; }

    }
    public class UpdateUserInfoResponseViewModel
    {
        public string responseStatus { get; set; }
        public string gtoday { get; set; }
        public string servertime { get; set; }
        public string isLoyal { get; set; }
    }

    public class ChargeAccountMerchantRequestViewModel
    {
        public int id { get; set; }
        public string merchantId { get; set; }
        public string merchantpassword { get; set; }
        public string cariorcard { get; set; }
        public string amount { get; set; }
        public CardTransactionDescription descriptionType { get; set; }
        public string referencenumber { get; set; }
        public string transactionnumber { get; set; }
        public int ipgcode { get; set; }
        public double pointPercent { get; set; }
    }
    public class ChargeAccountMerchantResponseViewModel
    {
        public string ResponseStatus { get; set; }
        public string Gtoday { get; set; }
        public string Servertime { get; set; }
        public string isLoyal { get; set; }
    }
    public class ForgotPasswordRequestViewModel
    {
        public string cariorcard { get; set; }
        public string mobile { get; set; }
    }
    public class ForgotPasswordResponseViewModel
    {
        public string newPassword { get; set; }
    }
    public class ChangeCardViewModel
    {
        public bool cardType { get; set; }
        public string cariorcard { get; set; }
        public string newCariorCard { get; set; }
        public string adminusername { get; set; }
        public string adminpassword { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string jbirthDate { get; set; }
        public string ntCode { get; set; }
        public string telHome { get; set; }
        public string homeAddr { get; set; }
        public long? kiaPoint { get; set; }
        public double debitAmount { get; set; }
        public double chargeAmount { get; set; }
    }
    public class ChangeCardBalanceViewModel
    {
        public string cariorcard { get; set; }
        public string credit { get; set; }
        public string debit { get; set; }
    }
    public class CardChargeAggreementViewModel
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int branch { get; set; }
        public int userInfoId { get; set; }
        public string word { get; set; }
        public string productCode { get; set; }
        public double amount { get; set; }
        public double requestAmount { get; set; }
        public string requestAmountSeparator { get; set; }
        public double pointPercent { get; set; }
        public string amountSeparator { get; set; }
        public string description { get; set; }
        public string checkoutDescription { get; set; }
        public string fullName { get; set; }
        public string mobile { get; set; }
        public string createUser { get; set; }
        public DateTime createDate { get; set; }
        public string persianCreateDate { get; set; }
        public string cariorCode { get; set; }
        public string descriptionTypeTitle { get; set; }
        public string factorNumber { get; set; }
        public string createPerson { get; set; }
        public int count { get; set; }
        public int page { get; set; }
        public CardTransactionStatus status { get; set; }
        public CardTransactionDescription? descriptionType { get; set; }

    }

    public class LoadCardChargeViewModel
    {
        public int id { get; set; }
        public long amount { get; set; }
        public CardTransactionDescription? descriptionType { get; set; }
        public string description { get; set; }
        public string factorNumber { get; set; }
        public string cariorCode { get; set; }
        public string productCode { get; set; }
        public int userInfoId { get; set; }
    }

    public class CheckoutViewModel
    {
        public int id { get; set; }
        public string description { get; set; }
    }
    public class CustomerFactorProductCodeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int userInfoId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int personId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string factorNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string persianDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string productCode { get; set; }
    }
}

