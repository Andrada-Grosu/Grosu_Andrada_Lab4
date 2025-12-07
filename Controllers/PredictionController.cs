using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Grosu_Andrada_Lab4.Data;
using Grosu_Andrada_Lab4.Models;
using static Grosu_Andrada_Lab4.PricePredictionModel;

namespace Grosu_Andrada_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        private readonly Grosu_Andrada_Lab4Context _context;

        public PredictionController(Grosu_Andrada_Lab4Context context)
        {
            _context = context;
        }

        // ==================== PREDICȚIE PREȚ ====================

        [HttpGet]
        public IActionResult Price()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Price(ModelInput input)
        {
            MLContext mlContext = new MLContext();

            var modelPath = Path.Combine(Directory.GetCurrentDirectory(),
                                         "PricePredictionModel.mlnet");

            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            ModelOutput result = predEngine.Predict(input);

            ViewBag.Price = result.Score;
            return View(input);
        }

        // ==================== DASHBOARD ====================

        [HttpGet]
        public async Task<IActionResult> Dashboard(DateTime? fromDate, DateTime? toDate)
        {
            // 0. Pornim de la un query filtrabil
            var query = _context.PredictionHistory.AsQueryable();

            // ⚠️ Aici folosește NUMELE real al câmpului de dată din PredictionHistory:
            //    - dacă ai `PredictionDate`, lasă așa
            //    - dacă ai `CreatedAt`, schimbă în CreatedAt
            if (fromDate.HasValue)
                query = query.Where(p => p.PredictionDate.Date >= fromDate.Value.Date);

            if (toDate.HasValue)
                query = query.Where(p => p.PredictionDate.Date <= toDate.Value.Date);

            // 1. Numărul total de predicții
            var totalPredictions = await query.CountAsync();

            // 2. Preț mediu per tip de plată + număr de predicții per tip
            var paymentTypeStats = await query
                .GroupBy(p => p.PaymentType)
                .Select(g => new PaymentTypeStat
                {
                    PaymentType = g.Key,
                    AveragePrice = g.Average(x => x.PredictedPrice),
                    Count = g.Count()
                })
                .ToListAsync();

            // 3. Distribuția prețurilor pe intervale (buckets)
            var allPrices = await query
                .Select(p => p.PredictedPrice)
                .ToListAsync();

            var buckets = new List<PriceBucketStat>
    {
        new PriceBucketStat { BucketLabel = "0 - 10" },
        new PriceBucketStat { BucketLabel = "10 - 20" },
        new PriceBucketStat { BucketLabel = "20 - 30" },
        new PriceBucketStat { BucketLabel = "30 - 50" },
        new PriceBucketStat { BucketLabel = "> 50" }
    };

            foreach (var price in allPrices)
            {
                if (price < 10)
                    buckets[0].Count++;
                else if (price < 20)
                    buckets[1].Count++;
                else if (price < 30)
                    buckets[2].Count++;
                else if (price < 50)
                    buckets[3].Count++;
                else
                    buckets[4].Count++;
            }

            // 4. Construim ViewModel-ul, inclusiv intervalul curent
            var vm = new DashboardViewModel
            {
                TotalPredictions = totalPredictions,
                PaymentTypeStats = paymentTypeStats,
                PriceBuckets = buckets,
                FromDate = fromDate,
                ToDate = toDate
            };

            return View(vm);
        }

    }
}
