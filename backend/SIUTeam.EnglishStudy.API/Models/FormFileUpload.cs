using Microsoft.AspNetCore.Http;
using SIUTeam.EnglishStudy.Core.DTOs;

namespace SIUTeam.EnglishStudy.API.Models;

public class FormFileUpload : IFileUpload
{
    private readonly IFormFile _formFile;

    public FormFileUpload(IFormFile formFile)
    {
        _formFile = formFile ?? throw new ArgumentNullException(nameof(formFile));
    }

    public Stream GetStream() => _formFile.OpenReadStream();

    public string FileName => _formFile.FileName;

    public string ContentType => _formFile.ContentType;

    public long Length => _formFile.Length;
}
