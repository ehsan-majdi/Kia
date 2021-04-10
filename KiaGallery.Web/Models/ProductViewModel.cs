using KiaGallery.Model;
using System;
using System.Collections.Generic;

namespace KiaGallery.Web.Models
{
    public class ProductViewModel
    {
        public int? id { get; set; }
        public int? productCollectionId { get; set; }
        public string code { get; set; }
        public string bookCode { get; set; }
        public string title { get; set; }
        public ProductType productType { get; set; }
        public OuterWerkCategory? outerWerkCategory { get; set; }
        public bool specialProduct { get; set; }
        public GoldType goldType { get; set; }
        public Sex sex { get; set; }
        public int workshopId { get; set; }
        public int? workshopId2 { get; set; }
        public float? weight { get; set; }
        public float? silverWeight { get; set; }
        public int stoneCount { get; set; }
        public int stonePrice { get; set; }
        public int leatherCount { get; set; }
        public int leatherPrice { get; set; }
        public int? sizeId { get; set; }
        public int? normalSizeValueId { get; set; }
        public int wage { get; set; }
        public bool? canLoop { get; set; }
        public bool? ringSizeType { get; set; }
        public bool? notOrderable { get; set; }
        public bool? notOrderableForBranch { get; set; }
        public bool active { get; set; }
        public int newDay { get; set; }
        public string outerWerkPlacement { get; set; }
        public bool? goldColor { get; set; }
        public bool? rosegoldColor { get; set; }
        public bool? whiteColor { get; set; }
        public int? workshopTagId { get; set; }
        public bool? onlyForWorkshop { get; set; }
        public bool? earringBack { get; set; }
        public bool? orderableBranchType { get; set; }
        public bool? orderableCoWorkerType { get; set; }
        public bool? orderableSolicitorshipType { get; set; }
        public bool? orderableOtherType { get; set; }

        public List<ProductFileViewModel> fileList { get; set; }
        public List<ProductStoneViewModel> stoneList { get; set; }
        public List<ProductLeatherViewModel> leatherList { get; set; }
        public List<OuterWerkType> outerWerkTypeList { get; set; }
        public List<OuterWerkCategory?> outerWerkCategoryList { get; set; }
    }

    public class ProductFileViewModel
    {
        public int? id { get; set; }
        public string fileName { get; set; }
        public FileType fileType { get; set; }
        public int paddingImg { get; set; }
        public int productId { get; set; }
    }

    public class ProductStoneViewModel
    {
        public int id { get; set; }
        public int order { get; set; }
        public int stoneId { get; set; }
        public int? defaultStoneId { get; set; }
        public StoneShape stoneShape { get; set; }
        public int? shapeSizeId { get; set; }
    }

    public class ProductLeatherViewModel
    {
        public int id { get; set; }
        public int order { get; set; }
        public int leatherId { get; set; }
    }

    public class ProductSearchViewModel
    {
        public int page { get; set; }
        public int count { get; set; }
        public bool fav { get; set; }
        public int? workshopId { get; set; }
        public string term { get; set; }
        public bool? leatherBracelet { get; set; }
        public bool? isNew { get; set; }
        public bool? active { get; set; }
        public List<int> type { get; set; }
        public List<int> sex { get; set; }
        public List<int> outerWerkType { get; set; }
        public List<int> outerWerkCategory { get; set; }
        public bool? related { get; set; }
        public bool? set { get; set; }
        public bool? isOuterWerk { get; set; }
        public bool? noModelImage { get; set; }
        public bool? firstPhoto { get; set; }
        public bool? secondPhoto { get; set; }
        public bool? modelImage { get; set; }
        public bool? whiteBack { get; set; }
        public bool? siteImage { get; set; }
        public bool? hasNotCode { get; set; }
        public bool? orderImage { get; set; }
        public bool whiteColor { get; set; }
        public bool goldColor { get; set; }
        public bool rosegoldColor { get; set; }
        public bool orderableBranchType { get; set; }
        public bool orderableSolicitorshipType { get; set; }
        public bool orderableCoWorkerType { get; set; }
        public bool orderableOtherType { get; set; }
        public float? lower { get; set; }
        public float? upper { get; set; }
        public int? collectionId { get; set; }
        public bool? notOrderable { get; set; }
        public ShowProduct? showProduct { get; set; }
        public List<int> goldType { get; set; }
        public bool? specialProduct { get; set; }
        public bool? outerwerkPage { get; set; }
        public List<bool?> color { get; set; }
    }
    public class ResponseProductSearchViewModel
    {
        public int id { get; set; }
        public string productCollectionTitle { get; set; }
        public string productCollectionFileName { get; set; }
        public string code { get; set; }
        public bool? specialProduct { get; set; }
        public string showProductTitle { get; set; }
        public string outerWerkPlacement { get; set; }
        public string bookCode { get; set; }
        public string fileName { get; set; }
        public string modelImage { get; set; }
        public string orderImage { get; set; }
        public string siteImage { get; set; }
        public int? paddingImg { get; set; }
        public ProductType productType { get; set; }
        public string productTypeTitle { get; set; }
        public string title { get; set; }
        public float? weight { get; set; }
        public float? silverWeight { get; set; }
        public bool active { get; set; }
        public int productNew { get; set; }
        public DateTime modifyDate { get; set; }
        public bool isNew { get; set; }
        public bool isFav { get; set; }
        public int related { get; set; }
        public int set { get; set; }
        public bool firstPhoto { get; set; }
        public bool secondPhoto { get; set; }
        public bool? notOrderableForBranch { get; set; }
    }


    public class RelatedProductViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; }
        public string ProductCode { get; set; }
        public string ProductBookCode { get; set; }
        public string ProductFileName { get; set; }
    }
}