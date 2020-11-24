using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text.Json;
using RoccoServe.Framework.Server.Protocols;

namespace RoccoServe.SocketServer
{
    public class LengthPrefixedProtocol : IMessageReader<Message>, IMessageWriter<Message>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out Message message)
        {
            var reader = new SequenceReader<byte>(input);

            var length = 0;
            
            JsonSerializer.de
            // if (input.Length >= 4)
            // {
            //     reader.tryre
            //     // length |= input.Slice(0,1);
            //     // length |= (((int)input[offset + 1]) << 8);
            //     // length |= (((int)input[offset + 2]) << 16);
            //     // length |= (((int)input[offset + 3]) << 24);
            // }
            
            // if (!reader.TryReadBigEndian(out int length) || input.Length < length)
            // {
            //     message = default;
            //     return false;
            // }

            var payload = input.Slice(reader.Position, length);
            message = new Message(payload);

            consumed = payload.End;
            examined = consumed;
            return true;
        }

        public void WriteMessage(Message message, IBufferWriter<byte> output)
        {
            var lengthBuffer = output.GetSpan(4);
            BinaryPrimitives.WriteInt32BigEndian(lengthBuffer, (int)message.Payload.Length);
            output.Advance(4);
            foreach (var memory in message.Payload)
            {
                output.Write(memory.Span);
            }
        }
    }

    public struct Message
    {
        public Message(byte[] payload) : this(new ReadOnlySequence<byte>(payload))
        {

        }

        public Message(ReadOnlySequence<byte> payload)
        {
            Payload = payload;
        }

        public ReadOnlySequence<byte> Payload { get; }
    }
}
