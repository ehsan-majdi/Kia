using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KiaGallery.Model.Context.BranchesPayments
{
    /// <summary>
    /// پرداخت شعب
    /// </summary>
    public class BranchesPayments
    {
        /// <summary>
        /// سازنده
        /// </summary>
        public BranchesPayments()
        {
            SendMessageList = new List<BranchesPaymentsSendMessage>();
            BranchesPaymentsDetails = new List<BranchesPaymentsDetails>();
            BranchesPaymentsLogList = new List<BranchesPaymentsLog>();
        }
        /// <summary>
        /// ردیف
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// نوع پرداخت
        /// </summary>
        public TypePayments TypePayments { get; set; }
        /// <summary>
        /// وزن اجرت طلا
        /// </summary>
        public float? GoldWage { get; set; }
        /// <summary>
        /// فی اجرت طلا
        /// </summary>
        public int? GoldWageUnit { get; set; }
        /// <summary>
        /// مبلغ اجرت طلا
        /// </summary>
        public long? PriceGoldWage { get; set; }
        /// <summary>
        /// وزن انتخاب اولی
        /// </summary>
        public float? SelectedWeight1 { get; set; }
        /// <summary>
        /// فی انتخاب اولی
        /// </summary>
        public int? SelectedUnit1 { get; set; }
        /// <summary>
        /// مبلغ انتخاب اولی
        /// </summary>
        public long? PriceSelected1 { get; set; }
        /// <summary>
        /// وزن انتخاب دومی
        /// </summary>
        public float? SelectedWeight2 { get; set; }
        /// <summary>
        /// فی انتخاب دومی
        /// </summary>
        public int? SelectedUnit2 { get; set; }
        /// <summary>
        /// مبلغ انتخاب دومی
        /// </summary>
        public long? PriceSelected2 { get; set; }
        /// <summary>
        /// وزن انتخاب هشت
        /// </summary>
        public float? SelectedWeight8 { get; set; }
        /// <summary>
        /// فی انتخاب هشت
        /// </summary>
        public int? SelectedUnit8 { get; set; }
        /// <summary>
        /// مبلغ انتخاب هشت
        /// </summary>
        public long? PriceSelected8 { get; set; }
        /// <summary>
        /// وزن انتخاب نهمی
        /// </summary>
        public float? SelectedWeight9 { get; set; }
        /// <summary>
        /// فی انتخاب نهمی
        /// </summary>
        public int? SelectedUnit9 { get; set; }
        /// <summary>
        /// مبلغ انتخاب نهمی
        /// </summary>
        public long? PriceSelected9 { get; set; }
        /// <summary>
        /// وزن انتخاب ده
        /// </summary>
        public float? SelectedWeight10 { get; set; }
        /// <summary>
        /// فی انتخاب ده
        /// </summary>
        public int? SelectedUnit10 { get; set; }
        /// <summary>
        /// مبلغ انتخاب ده
        /// </summary>
        public long? PriceSelected10 { get; set; }
        /// <summary>
        /// وزن انتخاب یازده
        /// </summary>
        public float? SelectedWeight11 { get; set; }
        /// <summary>
        /// فی انتخاب یازده
        /// </summary>
        public int? SelectedUnit11 { get; set; }
        /// <summary>
        /// مبلغ انتخاب یازده
        /// </summary>
        public long? PriceSelected11 { get; set; }
        /// <summary>
        /// وزن انتخاب دوازده
        /// </summary>
        public float? SelectedWeight12 { get; set; }
        /// <summary>
        /// فی انتخاب دوازده
        /// </summary>
        public int? SelectedUnit12 { get; set; }
        /// <summary>
        /// مبلغ انتخاب دوازده
        /// </summary>
        public long? PriceSelected12 { get; set; }
        /// <summary>
        /// وزن کسر شده
        /// </summary>
        public float? WeightDeducted { get; set; }
        /// <summary>
        /// فی کسر شده
        /// </summary>
        public int? DeductedUnit { get; set; }
        /// <summary>
        /// مبلغ کسر شده
        /// </summary>
        public long? PriceDeducted { get; set; }
        /// <summary>
        /// وزن هزینه سنگ
        /// </summary>
        public float? StoneWeight { get; set; }
        /// <summary>
        /// فی هزینه سنگ
        /// </summary>
        public int? StoneUnit { get; set; }
        /// <summary>
        /// مبلغ هزینه سنگ
        /// </summary>
        public long? PriceStone { get; set; }
        /// <summary>
        /// وزن هزینه چرم
        /// </summary>
        public float? LeatherWeight { get; set; }
        /// <summary>
        /// فی هزینه چرم
        /// </summary>
        public int? LeatherUnit { get; set; }
        /// <summary>
        /// مبلغ هزینه چرم
        /// </summary>
        public long? PriceLeather { get; set; }
        /// <summary>
        /// وزن چرم یک دور
        /// </summary>
        public float? OneRoundLeatherWeight { get; set; }
        /// <summary>
        /// فی چرم یک دور
        /// </summary>
        public int? OneRoundLeatherUnit { get; set; }
        /// <summary>
        /// مبلغ چرم یک دور
        /// </summary>
        public long? PriceOneRoundLeather { get; set; }
        /// <summary>
        /// وزن چرم یک دور
        /// </summary>
        public float? TwoRoundLeatherWeight { get; set; }
        /// <summary>
        /// فی چرم یک دور
        /// </summary>
        public int? TwoRoundLeatherUnit { get; set; }
        /// <summary>
        /// مبلغ چرم یک دور
        /// </summary>
        public long? PriceTwoRoundLeather { get; set; }
        /// <summary>
        /// وزن انتخاب سومی
        /// </summary>
        public float? SelectedWeight3 { get; set; }
        /// <summary>
        /// فی انتخاب سومی
        /// </summary>
        public int? SelectedUnit3 { get; set; }
        /// <summary>
        /// مبلغ انتخاب سومی
        /// </summary>
        public long? PriceSelected3 { get; set; }
        /// <summary>
        /// وزن انتخاب چهارمی
        /// </summary>
        public float? SelectedWeight4 { get; set; }
        /// <summary>
        /// فی انتخاب چهارمی
        /// </summary>
        public int? SelectedUnit4 { get; set; }
        /// <summary>
        /// مبلغ انتخاب چهارمی
        /// </summary>
        public long? PriceSelected4 { get; set; }
        /// <summary>
        /// وزن انتخاب پنجمی
        /// </summary>
        public float? SelectedWeight5 { get; set; }
        /// <summary>
        /// فی انتخاب پنجمی
        /// </summary>
        public int? SelectedUnit5 { get; set; }
        /// <summary>
        /// مبلغ انتخاب پنجمی
        /// </summary>
        public long? PriceSelected5 { get; set; }
        /// <summary>
        /// وزن انتخاب ششمی
        /// </summary>
        public float? SelectedWeight6 { get; set; }
        /// <summary>
        /// فی انتخاب ششمی
        /// </summary>
        public int? SelectedUnit6 { get; set; }
        /// <summary>
        /// مبلغ انتخاب ششمی
        /// </summary>
        public long? PriceSelected6 { get; set; }
        /// <summary>
        /// وزن انتخاب هفتمی
        /// </summary>
        public float? SelectedWeight7 { get; set; }
        /// <summary>
        /// فی انتخاب هفتمی
        /// </summary>
        public int? SelectedUnit7 { get; set; }
        /// <summary>
        /// مبلغ انتخاب هفتمی
        /// </summary>
        public long? PriceSelected7 { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int BranchId { get; set; }
        /// <summary>
        /// بدهی حساب طلا
        /// </summary>
        public float GoldDebt { get; set; }
        /// <summary>
        /// بدهی حساب ریال
        /// </summary>
        public long RialDebt { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// مهر و امضا فروشنده
        /// </summary>
        public string DealerSignature { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string FactorNumber { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// اجرت طلا
        /// </summary>
        public long? Wage { get; set; }
        /// <summary>
        /// توضیحات انتخاب اولی
        /// </summary>
        public string DescriptionSelected1 { get; set; }
        /// <summary>
        /// توضیحات انتخاب دومی
        /// </summary>
        public string DescriptionSelected2 { get; set; }
        /// <summary>
        /// توضیحات انتخاب سومی
        /// </summary>
        public string DescriptionSelected3 { get; set; }
        /// <summary>
        /// توضیحات انتخاب چهارمی
        /// </summary>
        public string DescriptionSelected4 { get; set; }
        /// <summary>
        /// توضیحات انتخاب پنجمی
        /// </summary>
        public string DescriptionSelected5 { get; set; }
        /// <summary>
        /// توضیحات انتخاب ششمی
        /// </summary>
        public string DescriptionSelected6 { get; set; }
        /// <summary>
        /// توضیحات انتخاب هفتمی
        /// </summary>
        public string DescriptionSelected7 { get; set; }
        /// <summary>
        /// توضیحات انتخاب هشتمی
        /// </summary>
        public string DescriptionSelected8 { get; set; }
        /// <summary>
        /// توضیحات انتخاب نهمی
        /// </summary>
        public string DescriptionSelected9 { get; set; }
        /// <summary>
        /// توضیحات انتخاب دهمی
        /// </summary>
        public string DescriptionSelected10 { get; set; }
        /// <summary>
        /// توضیحات انتخاب یازده
        /// </summary>
        public string DescriptionSelected11 { get; set; }
        /// <summary>
        /// توضیحات انتخاب دوازده
        /// </summary>
        public string DescriptionSelected12 { get; set; }
        /// <summary>
        /// حساب آخرین دوره
        /// </summary>
        public long? LastPeriodAccount { get; set; }
        /// <summary>
        /// حساب آخرین دوره
        /// </summary>
        public float? LastPeriodAccountWeight { get; set; }
        /// <summary>
        /// ردیف کاربر ایجاد کننده
        /// </summary>
        public int CreateUserId { get; set; }
        /// <summary>
        /// ردیف کاربر ویرایش کننده
        /// </summary>
        public int ModifyUserId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// تاریخ آخرین ویرایش
        /// </summary>
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        [MaxLength(45)]
        public string Ip { get; set; }
        /// <summary>
        /// کاربر ایجاد کننده
        /// </summary>
        public virtual User CreateUser { get; set; }
        /// <summary>
        /// کاربر ویرایش کننده
        /// </summary>
        public virtual User ModifyUser { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual Branch Branch { get; set; }
        /// <summary>
        /// شعبه
        /// </summary>
        public virtual List<BranchesPaymentsSendMessage> SendMessageList { get; set; }
        /// <summary>
        /// جزئیات پرداخت شعب
        /// </summary>
        public virtual List<BranchesPaymentsDetails> BranchesPaymentsDetails { get; set; }
        /// <summary>
        /// لیست تغییر وضعیت های فاکتور
        /// </summary>
        public virtual List<BranchesPaymentsLog> BranchesPaymentsLogList { get; set; }
    }
}
