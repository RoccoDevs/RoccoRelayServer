using System.Buffers;

namespace RoccoServe.Framework.Server.Protocols
{
    public interface IMessageWriter<TMessage>
    {
        void WriteMessage(TMessage message, IBufferWriter<byte> output);
    }
}
