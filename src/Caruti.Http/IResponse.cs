﻿namespace Caruti.Http;

public interface IResponse
{
    public string Protocol { get; }
    byte[]? Body { get; }
    IDictionary<string, string[]> Headers { get; }
    Task SendHtml(string html, EStatusCode statusCode);
    Task StatusCode(EStatusCode statusCode);
}