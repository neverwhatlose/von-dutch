using Spectre.Console;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using von_dutch.Menu;

namespace von_dutch.Managers
{
    /// <summary>
    /// Менеджер для работы с моделью GigaChat.
    /// </summary>
    public class GigaChatAiService
    {
        private readonly string _accessToken;

        // Очевидно сикреты нельзя хранить в коде, но в тз не сказано
        // что токен от гигачата должен храниться в конфиг файле или где-либо еще :)))
        private const string BasicAuth = "Basic ZTdhMmU4OTUtMTllMi00YzU5LWFiNWEtNDk1YmJjNGY1ZDViOjQ2MGUyMzU1LTg2MDUtNDNlMS1iMjRhLWE1NmFiZjE0ZTNiNg==";
        private const string OauthUrl = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
        private const string GigaChatUrl = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";
        
        private static readonly Lazy<GigaChatAiService> SingletonInstance = new(() => new GigaChatAiService());
        public static GigaChatAiService Instance => SingletonInstance.Value;

        private GigaChatAiService()
        {
            _accessToken = GetBearerTokenAsync().GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(_accessToken))
            {
                return;
            }

            Console.WriteLine("Не удалось получить токен доступа. Завершение.");
        }

        /// <summary>
        /// Переводит предложение с английского на русский с использованием GigaChat.
        /// </summary>
        /// <param name="englishSentence">Предложение на английском языке для перевода.</param>
        /// <returns>Переведенное предложение на русском языке или сообщение об ошибке.</returns>
        /// <exception cref="HttpRequestException">Выбрасывается, если произошла ошибка при запросе к GigaChat.</exception>
        public async Task<string> TranslateSentence(string englishSentence)
        {
            if (string.IsNullOrWhiteSpace(englishSentence))
            {
                return "Пустая фраза для перевода.";
            }
            
            string requestBody = $@"
                {{
                  ""model"": ""GigaChat"",
                  ""messages"": [
                    {{
                      ""role"": ""system"",
                      ""content"": ""Ты профессиональный переводчик с английского на русский. Переведи предложение на русский язык, максимально сохраняя смысл, контекст и стилистику. Учитывай сленг, идиомы и культурные особенности. Если предложение содержит аббревиатуры или сокращения, расшифруй их и переведи в соответствии с контекстом. Пример: Предложение: 'ngl i was sleeping entire day', Перевод: Если честно, я спал весь день. Теперь переведи следующее:""
                    }},
                    {{
                      ""role"": ""user"",
                      ""content"": ""Предложение: {englishSentence}""
                    }}
                  ],
                  ""stream"": false,
                  ""update_interval"": 0
                }}";

            string responseJson = await CallGigaChatAsync(requestBody);
            return ExtractAssistantContent(responseJson);
        }

        /// <summary>
        /// Переводит слово или фразу в контексте предложения с использованием GigaChat.
        /// </summary>
        /// <param name="sentence">Предложение, содержащее слово или фразу для перевода.</param>
        /// <param name="word">Слово или фраза для перевода.</param>
        /// <returns>Переведенное слово или фраза на русском языке или сообщение об ошибке.</returns>
        /// <exception cref="HttpRequestException">Выбрасывается, если произошла ошибка при запросе к GigaChat.</exception>
        public async Task<string> TranslateWordWithContext(string sentence, string word)
        {
            if (string.IsNullOrWhiteSpace(sentence))
            {
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "en-ru.json",
                    word,
                    string.Empty,
                    "❌"
                );
                
                return "Пустое предложение.";
            }

            if (string.IsNullOrWhiteSpace(word))
            {
                return "Пустое слово для перевода.";
            }
            
            if (!sentence.Contains(word, StringComparison.CurrentCultureIgnoreCase))
            {
                HistoryManager.Log(
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    "en-ru.json",
                    word,
                    string.Empty,
                    "❌"
                );
                
                TerminalUi.DisplayMessage($"Слово \"{word}\" не найдено в предложении!", Color.Red);
                return string.Empty;
            }
            
            string requestBody = $@"
                {{
                  ""model"": ""GigaChat"",
                  ""messages"": [
                    {{
                      ""role"": ""system"",
                      ""content"": ""Ты профессиональный переводчик с английского на русский. Переведи слово или фразу, учитывая контекст предложения. Если слово или фраза являются сленгом, сокращением или аббревиатурой, обязательно расшифруй ее и переведи с учетом смысла и контекста. Не предупреждай пользователя о том, что это был сленг, сокращение или аббревиатура. Если ты не уверен в одном точном переводе, предложи до 5 вариантов перевода, чтобы пользователь мог выбрать подходящий. Не давай пояснений, не объясняй свой выбор и не предлагай альтернативные формулировки. Просто переведи. Пример: Предложение: 'ngl i was sleeping entire day', Слово/фраза: 'ngl', Перевод: если честно. Пример: Предложение: 'brb, gonna grab some food', Слово/фраза: 'brb', Перевод: сейчас вернусь. Теперь переведи следующее:""
                    }},
                    {{      
                      ""role"": ""user"",
                      ""content"": ""Предложение: '{sentence}', Слово/фраза: '{word}', Перевод:""
                    }}
                  ],
                  ""stream"": false,
                  ""update_interval"": 0,
                  ""temperature"": 0.2
                }}";

            string responseJson = await CallGigaChatAsync(requestBody);
            return ExtractAssistantContent(responseJson);
        }

        /// <summary>
        /// Выполняет запрос к GigaChat API.
        /// </summary>
        /// <param name="requestBody">Тело запроса в формате JSON.</param>
        /// <returns>Ответ от GigaChat в формате JSON.</returns>
        /// <exception cref="HttpRequestException">Выбрасывается, если произошла ошибка при запросе к GigaChat.</exception>
        private async Task<string> CallGigaChatAsync(string requestBody)
        {
            using HttpClient client = CreateHttpClient();
            
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(GigaChatUrl, content);
            string responseJson = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return responseJson;
            }
            
            TerminalUi.DisplayMessageWaiting($"Ошибка при вызове GigaChat: {response.StatusCode}", Color.Red);

            return responseJson;
        }

        /// <summary>
        /// Извлекает содержимое ответа от GigaChat.
        /// </summary>
        /// <param name="responseJson">Ответ от GigaChat в формате JSON.</param>
        /// <returns>Содержимое ответа или сообщение об ошибке.</returns>
        /// <exception cref="JsonException">Выбрасывается, если произошла ошибка при парсинге JSON.</exception>
        private static string ExtractAssistantContent(string responseJson)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(responseJson);
                JsonElement root = doc.RootElement;

                if (!root.TryGetProperty("choices", out JsonElement choices) || choices.GetArrayLength() <= 0)
                {
                    return "Неверный формат ответа GigaChat (нет поля choices или content).";
                }

                JsonElement firstChoice = choices[0];
                if (firstChoice.TryGetProperty("message", out JsonElement message) &&
                    message.TryGetProperty("content", out JsonElement content))
                {
                    return content.GetString() ?? "";
                }
                return "Неверный формат ответа GigaChat (нет поля choices или content).";
            }
            catch (Exception ex)
            {
                return "Ошибка при парсинге ответа: " + ex.Message;
            }
        }

        /// <summary>
        /// Создает HttpClient с настройками для работы с GigaChat API.
        /// </summary>
        /// <returns>Настроенный экземпляр HttpClient.</returns>
        private HttpClient CreateHttpClient()
        {
            HttpClientHandler handler = new ()
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            HttpClient client = new (handler);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        /// <summary>
        /// Получает токен доступа для работы с GigaChat API.
        /// </summary>
        /// <returns>Токен доступа или пустая строка, если токен не удалось получить.</returns>
        /// <exception cref="HttpRequestException">Выбрасывается, если произошла ошибка при запросе токена.</exception>
        /// <exception cref="JsonException">Выбрасывается, если произошла ошибка при парсинге JSON.</exception>
        private static async Task<string> GetBearerTokenAsync()
        {
            HttpClientHandler handler = new ()
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };

            using HttpClient client = new (handler);
            
            client.DefaultRequestHeaders.Add("Authorization", BasicAuth);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("RqUID", Guid.NewGuid().ToString());
            
            StringContent body = new(
                "scope=GIGACHAT_API_PERS", 
                Encoding.UTF8, 
                "application/x-www-form-urlencoded"
                );
            
            HttpResponseMessage response = await client.PostAsync(OauthUrl, body);
            
            if (!response.IsSuccessStatusCode)
            {
                TerminalUi.DisplayMessageWaiting($"Ошибка при получении токена: {response.StatusCode}", Color.Red);
                return string.Empty;
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            try
            {
                JsonDocument doc = JsonDocument.Parse(responseJson);
                JsonElement root = doc.RootElement;

                if (root.TryGetProperty("access_token", out JsonElement tokenProp))
                {
                    return tokenProp.GetString() ?? string.Empty;
                }
            }
            catch
            {
                TerminalUi.DisplayMessageWaiting("Ошибка при парсинге токена", Color.Red);
            }

            return string.Empty;
        }
    }
}