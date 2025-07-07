// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SIUTeam.EnglishStudy.Infrastructure.Data;
using SIUTeam.EnglishStudy.Infrastructure.Data.Configuration;
using SIUTeam.EnglishStudy.Infrastructure.Repositories;
using SIUTeam.EnglishStudy.Infrastructure.Data.Seeders;


var services = new ServiceCollection();

services.Configure<MongoDbSettings>(options =>
{
    options.ConnectionString = "mongodb+srv://siunvp:1h7qOwGZPDlqYZ2h@siu.vmhcq.mongodb.net/";
    options.DatabaseName = "EnglishStudy";
});

var serviceProvider = services.BuildServiceProvider();

var appDataContext = new MongoDbContext(serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>());

var vocabularyRepository = new VocabularyRepository(appDataContext);

await SIUTeam.EnglishStudy.Infrastructure.Data.Seeders.VocabularySeeder.SeedAsync(vocabularyRepository, appDataContext);