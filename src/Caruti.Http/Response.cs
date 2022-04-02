﻿using System.Net.Sockets;

namespace Caruti.Http;

public class Response : IResponse
{
    public string Protocol { get; }

    public byte[]? Body { get; private set; }
    public IDictionary<string, string[]> Headers { get; } = new Dictionary<string, string[]>();

    private readonly NetworkStream _stream;
    private readonly byte[] _newLineBytes = Encoding.UTF8.GetBytes(Environment.NewLine);

    public Response(string protocol, NetworkStream stream)
    {
        Protocol = protocol;
        _stream = stream;
    }

    private async Task WriteBody(byte[] data)
    {
        if (Body is not null)
            throw new InvalidOperationException("response body alredy set");

        Body = data;

        await _stream.WriteAsync(_newLineBytes);
        await _stream.WriteAsync(Encoding.UTF8.GetBytes($"Content-Length: {data.Length}"));
        await _stream.WriteAsync(_newLineBytes);
        await _stream.WriteAsync(_newLineBytes);
        await _stream.WriteAsync(data);
    }

    public async Task SendHtml(string html, EStatusCode statusCode)
    {
        await _stream.WriteAsync(Encoding.UTF8.GetBytes($"{Protocol} {(int)statusCode} {statusCode}"));
        await _stream.WriteAsync(_newLineBytes);
        await _stream.WriteAsync(Encoding.UTF8.GetBytes("Content-Type: text/html; charset=UTF-8"));
        await WriteBody(Encoding.UTF8.GetBytes(html));
        await _stream.FlushAsync();
    }

    public async Task StatusCode(EStatusCode statusCode)
    {
        await _stream.WriteAsync(Encoding.UTF8.GetBytes($"{Protocol} {(int)statusCode} {statusCode}"));
        await WriteBody(Array.Empty<byte>());
        await _stream.FlushAsync();
    }
}