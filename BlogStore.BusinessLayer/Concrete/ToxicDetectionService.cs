using BlogStore.BusinessLayer.Abstract;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace BlogStore.BusinessLayer.Concrete
{
    public class ToxicDetectionService : IToxicDetectionService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly string? _apiToken;
        private readonly string? _modelName;
        private const double TOXICITY_THRESHOLD = 0.005; 

        public ToxicDetectionService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
            _apiToken = _configuration["HuggingFace:ApiToken"];
            _modelName = _configuration["HuggingFace:ModelName"];
            
            // HTTP client'ı yapılandır
            if (!string.IsNullOrEmpty(_apiToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiToken}");
            }
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "BlogStore-ToxicDetection/1.0");
        }

        public async Task<bool> IsToxicCommentAsync(string commentText)
        {
            var toxicityScore = await GetToxicityScoreAsync(commentText);
            
            // Daha akıllı toksiklik kontrolü
          
            return toxicityScore > TOXICITY_THRESHOLD;
        }

        public async Task<double> GetToxicityScoreAsync(string commentText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(commentText))
                    return 0.0;

                if (string.IsNullOrEmpty(_modelName))
                {
                    return 0.0;
                }

                // Hugging Face API'ye gönderilecek veri
                var requestData = new
                {
                    inputs = commentText
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // API çağrısı
                var response = await _httpClient.PostAsync($"https://api-inference.huggingface.co/models/{_modelName}", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    // API yanıtını parse et
                    var result = JsonConvert.DeserializeObject<List<List<Dictionary<string, object>>>>(responseContent);
                    
                    if (result != null && result.Count > 0 && result[0].Count > 0)
                    {
                        // En yüksek toksiklik skorunu bul
                        double maxToxicityScore = 0.0;
                        
                        foreach (var category in result[0])
                        {
                            if (category.ContainsKey("score"))
                            {
                                var score = Convert.ToDouble(category["score"]);
                                if (score > maxToxicityScore)
                                {
                                    maxToxicityScore = score;
                                }
                            }
                        }
                        
                        return maxToxicityScore;
                    }
                }
             
                // API çağrısı başarısız olursa varsayılan olarak güvenli kabul et
                return 0.0;
            }
            catch (Exception)
            {
                // Hata durumunda güvenli tarafta kal
                return 0.0;
            }
        }
    }
}
//Süleyman Yıldız 2025-08-02