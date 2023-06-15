using System;
using System.Buffers;
using System.Buffers.Binary;

namespace Rocco.RelayServer.Core.Services;

public class PrefixBufferWriter : IBufferWriter<byte>
{
    private readonly Memory<byte> _memory;

    private readonly IBufferWriter<byte> _writer;

    private int _count;

    public PrefixBufferWriter(IBufferWriter<byte> writer)
    {
        _writer = writer;
        _memory = writer.GetMemory();
    }

    public void Advance(int count)
    {
        _count += count;
    }

    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        var start = _count + 4;

        return _memory[start..];
    }

    public Span<byte> GetSpan(int sizeHint = 0)
    {
        var start = _count + 4;

        return _memory.Span[start..];
    }

    public void Complete()
    {
        BinaryPrimitives.WriteInt32BigEndian(_memory.Span, _count);

        _writer.Advance(_count + 4);
    }
}