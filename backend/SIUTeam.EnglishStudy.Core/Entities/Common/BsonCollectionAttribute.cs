namespace SIUTeam.EnglishStudy.Core.Entities;

// Attribute to specify MongoDB collection name
public class BsonCollectionAttribute : Attribute
{
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}
