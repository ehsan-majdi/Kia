using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class BranchesPaymentsViewModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// نوع پرداخت
        /// </summary>
        public TypePayments typePayments { get; set; }
        /// <summary>
        /// وزن انتخاب هشت
        /// </summary>
        public float? selectedWeight8 { get; set; }
        /// <summary>
        /// فی انتخاب هشت
        /// </summary>
        public int? selectedUnit8 { get; set; }
        /// <summary>
        /// مبلغ انتخاب هشت
        /// </summary>
        public long? priceSelected8 { get; set; }
        /// <summary>
        /// وزن انتخاب نهمی
        /// </summary>
        public float? selectedWeight9 { get; set; }
        /// <summary>
        /// فی انتخاب نهمی
        /// </summary>
        public int? selectedUnit9 { get; set; }
        /// <summary>
        /// مبلغ انتخاب نهمی
        /// </summary>
        public long? priceSelected9 { get; set; }
        /// <summary>
        /// وزن انتخاب ده
        /// </summary>
        public float? selectedWeight10 { get; set; }
        /// <summary>
        /// فی انتخاب ده
        /// </summary>
        public int? selectedUnit10 { get; set; }
        /// <summary>
        /// مبلغ انتخاب ده
        /// </summary>
        public long? priceSelected10 { get; set; }
        /// <summary>
        /// وزن انتخاب یازده
        /// </summary>
        public float? selectedWeight11 { get; set; }
        /// <summary>
        /// فی انتخاب یازده
        /// </summary>
        public int? selectedUnit11 { get; set; }
        /// <summary>
        /// مبلغ انتخاب یازده
        /// </summary>
        public long? priceSelected11 { get; set; }
        /// <summary>
        /// وزن انتخاب دوازده
        /// </summary>
        public float? selectedWeight12 { get; set; }
        /// <summary>
        /// فی انتخاب دوازده
        /// </summary>
        public int? selectedUnit12 { get; set; }
        /// <summary>
        /// مبلغ انتخاب دوازده
        /// </summary>
        public long? priceSelected12 { get; set; }
        /// <summary>
        /// وزن انتخاب اولی
        /// </summary>
        public float? selectedWeight1 { get; set; }
        /// <summary>
        /// فی انتخاب اولی
        /// </summary>
        public int? selectedUnit1 { get; set; }
        /// <summary>
        /// وزن انتخاب دومی
        /// </summary>
        public float? selectedWeight2 { get; set; }
        /// <summary>
        /// فی انتخاب دومی
        /// </summary>
        public int? selectedUnit2 { get; set; }
        /// <summary>
        /// مبلغ انتخاب دومی
        /// </summary>
        public long? priceSelected2 { get; set; }
        /// <summary>
        /// مبلغ انتخاب اولی
        /// </summary>
        public long? priceSelected1 { get; set; }
        /// <summary>
        /// وزن کسر شده
        /// </summary>
        public float? weightDeducted { get; set; }
        /// <summary>
        /// فی کسر شده
        /// </summary>
        public int? deductedUnit { get; set; }
        /// <summary>
        /// مبلغ کسر شده
        /// </summary>
        public long? priceDeducted { get; set; }
        /// <summary>
        /// وزن هزینه سنگ
        /// </summary>
        public float? stoneWeight { get; set; }
        /// <summary>
        /// فی هزینه سنگ
        /// </summary>
        public int? stoneUnit { get; set; }
        /// <summary>
        /// مبلغ هزینه سنگ
        /// </summary>
        public long? priceStone { get; set; }
        /// <summary>
        /// وزن هزینه چرم
        /// </summary>
        public float? leatherWeight { get; set; }
        /// <summary>
        /// فی هزینه چرم
        /// </summary>
        public int? leatherUnit { get; set; }
        /// <summary>
        /// مبلغ هزینه چرم
        /// </summary>
        public long? priceLeather { get; set; }
        /// <summary>
        /// وزن چرم یک دور
        /// </summary>
        public float? oneRoundLeatherWeight { get; set; }
        /// <summary>
        /// فی چرم یک دور
        /// </summary>
        public int? oneRoundLeatherUnit { get; set; }
        /// <summary>
        /// مبلغ چرم یک دور
        /// </summary>
        public long? priceOneRoundLeather { get; set; }
        /// <summary>
        /// وزن چرم یک دور
        /// </summary>
        public float? twoRoundLeatherWeight { get; set; }
        /// <summary>
        /// فی چرم یک دور
        /// </summary>
        public int? twoRoundLeatherUnit { get; set; }
        /// <summary>
        /// مبلغ چرم یک دور
        /// </summary>
        public long? priceTwoRoundLeather { get; set; }
        /// <summary>
        /// وزن چرم دور
        /// </summary>
        public float? roundLeatherWeight { get; set; }
        /// <summary>
        /// فی چرم دور
        /// </summary>
        public int? roundLeatherUnit { get; set; }
        /// <summary>
        /// مبلغ چرم دور
        /// </summary>
        public long? priceRoundLeather { get; set; }
        /// <summary>
        /// وزن هزینه
        /// </summary>
        public float? silverWeight { get; set; }
        /// <summary>
        /// فی هزینه نقره
        /// </summary>
        public int? silverUnit { get; set; }
        /// <summary>
        /// مبلغ هزینه نقره
        /// </summary>
        public long? priceSilver { get; set; }
        /// <summary>
        /// وزن مابه التفاوت سنگ
        /// </summary>
        public float? differenceStoneWeight { get; set; }
        /// <summary>
        /// فی مابه التفاوت سنگ
        /// </summary>
        public int? differenceStoneUnit { get; set; }
        /// <summary>
        /// مبلغ مابه التفاوت سنگ
        /// </summary>
        public long? priceDifferenceStone { get; set; }
        /// <summary>
        /// وزن گیفت کارت
        /// </summary>
        public float? giftWeight { get; set; }
        /// <summary>
        /// فی گیفت کارت
        /// </summary>
        public int? giftUnit { get; set; }
        /// <summary>
        /// مبلغ گیفت کارت
        /// </summary>
        public long? priceGift { get; set; }
        /// <summary>
        /// وزن انتخاب سومی
        /// </summary>
        public float? selectedWeight3 { get; set; }
        /// <summary>
        /// فی انتخاب سومی
        /// </summary>
        public int? selectedUnit3 { get; set; }
        /// <summary>
        /// مبلغ انتخاب سومی
        /// </summary>
        public long? priceSelected3 { get; set; }
        /// <summary>
        /// وزن انتخاب چهارمی
        /// </summary>
        public float? selectedWeight4 { get; set; }
        /// <summary>
        /// فی انتخاب چهارمی
        /// </summary>
        public int? selectedUnit4 { get; set; }
        /// <summary>
        /// مبلغ انتخاب چهارمی
        /// </summary>
        public long? priceSelected4 { get; set; }
        /// <summary>
        /// وزن انتخاب پنجمی
        /// </summary>
        public float? selectedWeight5 { get; set; }
        /// <summary>
        /// فی انتخاب پنجمی
        /// </summary>
        public int? selectedUnit5 { get; set; }
        /// <summary>
        /// مبلغ انتخاب پنجمی
        /// </summary>
        public long? priceSelected5 { get; set; }
        /// <summary>
        /// وزن انتخاب ششمی
        /// </summary>
        public float? selectedWeight6 { get; set; }
        /// <summary>
        /// فی انتخاب ششمی
        /// </summary>
        public int? selectedUnit6 { get; set; }
        /// <summary>
        /// مبلغ انتخاب ششمی
        /// </summary>
        public long? priceSelected6 { get; set; }
        /// <summary>
        /// وزن انتخاب هفتمی
        /// </summary>
        public float? selectedWeight7 { get; set; }
        /// <summary>
        /// فی انتخاب هفتمی
        /// </summary>
        public int? selectedUnit7 { get; set; }
        /// <summary>
        /// مبلغ انتخاب هفتمی
        /// </summary>
        public long? priceSelected7 { get; set; }
        /// <summary>
        /// توضیحات انتخاب پنجمی
        /// </summary>
        public string descriptionSelected5 { get; set; }
        /// <summary>
        /// توضیحات انتخاب ششمی
        /// </summary>
        public string descriptionSelected6 { get; set; }
        /// <summary>
        /// توضیحات انتخاب هفتمی
        /// </summary>
        public string descriptionSelected7 { get; set; }
        /// <summary>
        /// توضیحات انتخاب هشتمی
        /// </summary>
        public string descriptionSelected8 { get; set; }
        /// <summary>
        /// توضیحات انتخاب نهمی
        /// </summary>
        public string descriptionSelected9 { get; set; }
        /// <summary>
        /// توضیحات انتخاب دهمی
        /// </summary>
        public string descriptionSelected10 { get; set; }
        /// <summary>
        /// توضیحات انتخاب یازده
        /// </summary>
        public string descriptionSelected11 { get; set; }
        /// <summary>
        /// توضیحات انتخاب دوازده
        /// </summary>
        public string descriptionSelected12 { get; set; }
        /// <summary>
        /// وزن اجرت طلا
        /// </summary>
        public float? goldWage { get; set; }
        /// <summary>
        /// فی اجرت طلا
        /// </summary>
        public int? goldWageUnit { get; set; }
        /// <summary>
        /// مبلغ اجرت طلا
        /// </summary>
        public long? priceGoldWage { get; set; }
        /// <summary>
        /// ردیف شعبه
        /// </summary>
        public int branchId { get; set; }
        /// <summary>
        /// بدهی حساب طلا
        /// </summary>
        public float goldDebt { get; set; }
        /// <summary>
        /// بدهی حساب ریال
        /// </summary>
        public long rialDebt { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// مهر و امضا فروشنده
        /// </summary>
        public string dealerSignature { get; set; }
        /// <summary>
        /// نام شعب
        /// </summary>
        public string branchName { get; set; }
        public string typePaymentsTitle { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string factorNumber { get; set; }
        /// <summary>
        /// تاریخ
        /// </summary>
        public string date { get; set; }
        /// <summary>
        /// اجرت طلا
        /// </summary>
        public long? wage { get; set; }
        /// <summary>
        /// توضیحات انتخاب اولی
        /// </summary>
        public string descriptionSelected1 { get; set; }
        /// <summary>
        /// توضیحات انتخاب دومی
        /// </summary>
        public string descriptionSelected2 { get; set; }
        /// <summary>
        /// توضیحات انتخاب سومی
        /// </summary>
        public string descriptionSelected3 { get; set; }
        /// <summary>
        /// توضیحات انتخاب چهارمی
        /// </summary>
        public string descriptionSelected4 { get; set; }
        /// <summary>
        /// حساب آخرین دوره
        /// </summary>
        public long? lastPeriodAccount { get; set; }
        /// <summary>
        /// حساب آخرین دوره
        /// </summary>
        public float? lastPeriodAccountWeight { get; set; }
        public virtual List<BranchesPaymentsLogViewModel> branchesPaymentsLogList { get; set; }
    }
    public class BranchesPaymentsLogViewModel
    {
        public int id { get; set; }
        public BranchesPaymentsStatus status { get; set; }
        public string statusTitle { get; set; }
        public int userId { get; set; }
        public string userFullName { get; set; }
        public DateTime date { get; set; }
        public string persianDate { get; set; }
    }
    public class BranchesPaymentsSearchViewModel{
        public int page { get; set; }
        public int count { get; set; }
        public string term { get; set; }
        public int? branch { get; set; }
        public TypePayments? typePayments { get; set; }
    }

    public class BranchesPaymentsDetailsViwModel
    {
        /// <summary>
        /// ردیف
        /// </summary>
        public int? id { get; set; }
        /// <summary>
        /// اجرت طلا
        /// </summary>
        public long? goldWage { get; set; }
        /// <summary>
        /// وزن طلا
        /// </summary>
        public float? goldWeights { get; set; }
        /// <summary>
        /// مبلغ 
        /// </summary>
        public long? amount { get; set; }
        /// <summary>
        /// تعداد محصول متفرقه (چرم ، گیفت)
        /// </summary>
        public int? number { get; set; }
        /// <summary>
        /// عنوان
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// کد
        /// </summary>
        public string code { get; set; }
    }
}
