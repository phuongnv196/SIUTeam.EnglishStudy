namespace SIUTeam.EnglishStudy.Core.DTOs;

// Base interface for file upload
public interface IFileUpload
{
    Stream GetStream();
    string FileName { get; }
    string ContentType { get; }
    long Length { get; }
}
