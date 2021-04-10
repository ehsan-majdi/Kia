namespace KiaGallery.Model.Context
{
    public class CustomerCreditLog
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public long? Credit { get; set; }
        public long? UsedCredit { get; set; }
        public CustomerCardLevel CustomerCardLevel { get; set; }
        public virtual CustomerLoyality CustomerLoyality { get; set; }
    }
}
