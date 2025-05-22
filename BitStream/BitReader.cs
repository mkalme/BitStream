using System;
using System.IO;

namespace BitStream {
    public class BitReader : IDisposable {
        public Stream BaseStream { get; }
        private bool _leaveStreamOpen;

        private byte _bufferByte;
        private int _bufferByteIndex = 0;
        private bool _bufferByteRed = false;

        private byte[] _buffer;
        private int _bufferIndex = 0;

        private bool _disposedValue;

        public BitReader(Stream baseStream, bool leaveStreamOpen = false) {
            BaseStream = baseStream;
            _leaveStreamOpen = leaveStreamOpen;
        }

        public virtual long ReadBits(int count) {
            ulong output = 0;

            int leftCount = count;
            while (leftCount > 0) {
                InitializeBufferByte();

                int extractCount = 8 - _bufferByteIndex;
                if (extractCount > leftCount) extractCount = leftCount;

                output |= (ulong)ExtractValue(_bufferByte, extractCount, _bufferByteIndex) << (count - leftCount);

                _bufferByteIndex += extractCount;
                leftCount -= extractCount;
            }

            return (long)output;
        }
        public virtual long[] ReadBitArray(long length, int bitCount) {
            InitializeBufferArray(length * bitCount);

            long[] output = new long[length];

            for (int i = 0; i < output.Length; i++) {
                ulong value = 0;

                int leftBitCount = bitCount;
                while (leftBitCount > 0) {
                    int indexInByte = _bufferIndex % 8;

                    int extractCount = 8 - indexInByte;
                    if (extractCount > leftBitCount) extractCount = leftBitCount;

                    value |= (ulong)ExtractValue(_buffer[_bufferIndex / 8], extractCount, indexInByte) << (bitCount - leftBitCount);

                    _bufferIndex += extractCount;
                    _bufferByteIndex += extractCount;
                    leftBitCount -= extractCount;
                }

                output[i] = (long)value;
            }

            return output;
        }
        
        protected void InitializeBufferByte() {
            if (_bufferByteIndex > 7 || !_bufferByteRed) {
                _bufferByte = (byte)BaseStream.ReadByte();
                _bufferByteIndex = 0;

                _bufferByteRed = true;
            }
        }
        protected void InitializeBufferArray(long bitCount) {
            InitializeBufferByte();

            if (bitCount == 0) {
                _buffer = new byte[0];
                return;
            }

            int totalLength = (int)Math.Ceiling((_bufferIndex + bitCount) / 8F);

            _buffer = new byte[totalLength];
            _buffer[0] = _bufferByte;

            _bufferIndex = _bufferByteIndex;

            BaseStream.Read(_buffer, 1, _buffer.Length - 1);
        }

        private static byte ExtractValue(byte source, int n, int count) {
            return (byte)(source << (8 - n - count) >> 8 - count);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) {
                if (!_leaveStreamOpen) {
                    BaseStream.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
