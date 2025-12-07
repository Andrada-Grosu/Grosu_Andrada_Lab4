namespace Grosu_Andrada_Lab4.Models
{
    public class PredictionHistory
    {
        public int Id { get; set; }
        
        public string? InputData { get; set; }
        public float PredictedPrice { get; set; }

        public DateTime PredictionDate { get; set; } = DateTime.Now;

        public string PaymentType { get; set; } = string.Empty;   


    }
}
