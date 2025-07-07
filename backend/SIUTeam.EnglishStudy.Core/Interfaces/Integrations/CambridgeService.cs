using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using SIUTeam.EnglishStudy.Core.DTOs;
using SIUTeam.EnglishStudy.Core.Helpers;
using System.Net;

namespace SIUTeam.EnglishStudy.Core.Interfaces.Integrations
{
    public class CambridgeService(HttpClient httpClient, IConfiguration configuration, CryptoString cryptoString) : ICambridgeService
    {
        public async Task<Stream> GetAudioAsync(string audioKey)
        {
            var audioUrl = cryptoString.Decrypt(audioKey);
            if (string.IsNullOrWhiteSpace(audioUrl))
            {
                throw new ArgumentException("Audio key is invalid or empty.", nameof(audioKey));
            }
            return await httpClient.GetStreamAsync(audioUrl);
        }

        public async Task<List<CambridgeDictionaryDto>> GetWordDetailsAsync(string word, string region = "us")
        {
            var baseUrl = configuration["CambridgeService:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("CambridgeService:BaseUrl is not configured.");
            }
            httpClient.BaseAddress = new Uri(baseUrl);
            HtmlDocument doc = new HtmlDocument();
            List<CambridgeDictionaryDto> vocabularies = new List<CambridgeDictionaryDto>();
            try
            {
                var response = await httpClient.GetAsync($"{region}/dictionary/english/{WebUtility.UrlEncode(word)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    doc.LoadHtml(content);
                    var phoneticNodes = doc.DocumentNode.SelectNodes("//div[@class='pr dictionary']");
                    // Check if phoneticNodes is not null and has at least one element

                    if (phoneticNodes != null && phoneticNodes.Count > 0)
                    {
                        foreach (var phoneticNode in phoneticNodes)
                        {
                            //Find the element with class 'region dreg'
                            var regionNode = phoneticNode.SelectSingleNode(".//span[@class='region dreg']");
                            var posgram = phoneticNode.SelectSingleNode(".//span[@class='pos dpos']");
                            var epp = phoneticNode.SelectSingleNode(".//span[contains(@class, 'epp-xref')]");
                            var description = phoneticNode.SelectSingleNode(".//div[contains(@class, 'ddef_d')]");
                            //var description = phoneticNode.SelectSingleNode(".//span[contains(@class, 'ddef_d')]");
                            var synonymsEl = phoneticNode.SelectSingleNode(".//div[contains(@class, 'synonyms')]");
                            var synonyms = synonymsEl?.SelectNodes(".//div[contains(@class, 'item')]");
                            var usRegion = phoneticNode.SelectSingleNode(".//span[contains(@class, 'us dpron-i')]");
                            var ukRegion = phoneticNode.SelectSingleNode(".//span[contains(@class, 'uk dpron-i')]");
                            var examples = phoneticNode.SelectNodes(".//div[contains(@class, 'examp dexamp')]");
                            var vocabulary = new CambridgeDictionaryDto
                            {
                                Word = word,
                                Posgram = posgram?.InnerText?.Trim() ?? string.Empty,
                                Description = description?.InnerText?.Trim() ?? string.Empty,
                                Level = epp?.InnerText?.Trim() ?? string.Empty,
                            };
                            if (usRegion != null)
                            {
                                var audioUs = usRegion?.SelectSingleNode(".//source[@type='audio/mpeg']");
                                var ipaUs = usRegion?.SelectSingleNode(".//span[contains(@class, 'ipa')]");
                                vocabulary.Regions.Add(new CambrigeDictionaryRegionDto
                                {
                                    Name = "US",
                                    AudioKey = cryptoString.Encrypt(audioUs?.GetAttributeValue("src", string.Empty) ?? string.Empty),
                                    Ipa = ipaUs?.InnerText?.Trim() ?? string.Empty
                                });
                            }

                            if (ukRegion != null)
                            {
                                var audioUk = ukRegion?.SelectSingleNode(".//source[@type='audio/mpeg']");
                                var ipaUk = ukRegion?.SelectSingleNode(".//span[contains(@class, 'ipa')]");
                                vocabulary.Regions.Add(new CambrigeDictionaryRegionDto
                                {
                                    Name = "UK",
                                    AudioKey = audioUk?.GetAttributeValue("src", string.Empty) ?? string.Empty,
                                    Ipa = ipaUk?.InnerText?.Trim() ?? string.Empty
                                });
                            }

                            if (synonyms != null)
                            {
                                foreach (var synonym in synonyms)
                                {
                                    var span = synonym.SelectSingleNode(".//span");
                                    vocabulary.Synonyms.Add(span?.InnerText.Trim() ?? string.Empty);
                                }
                            }

                            if (examples != null)
                            {
                                foreach (var example in examples)
                                {
                                    var span = example.SelectSingleNode(".//span[@class='eg deg']");
                                    vocabulary.Examples.Add(span?.InnerText.Trim() ?? string.Empty);
                                }
                            }
                            vocabularies.Add(vocabulary);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return vocabularies;
            }
            return vocabularies;
        }
    }
}
