using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using static Grosu_Andrada_Lab4.PricePredictionModel;

namespace Grosu_Andrada_Lab4.Controllers
{
    public class PredictionController : Controller
    {
        public IActionResult Price(ModelInput input)
        {
            // Load the model
            MLContext mlContext = new MLContext();
            // Create predection engine related to the loaded train model
            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), "PricePredictionModel.mlnet");

            ITransformer mlModel = mlContext.Model.Load(modelPath, out var modelInputSchema);

            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput,
           ModelOutput>(mlModel);
            // Try model on sample data to predict fair price
            ModelOutput result = predEngine.Predict(input);
            ViewBag.Price = result.Score;
            return View(input);
        }

    }
}
