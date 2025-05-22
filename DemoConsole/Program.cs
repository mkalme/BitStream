using System;
using System.Diagnostics;
using System.IO;
using BitStream;

namespace DemoConsole {
    class Program {
        static void Main(string[] args) {
            //MemoryStream stream1 = new MemoryStream(new byte[1024 * 1024 * 100]);
            BitReader bitReader = new BitReader(new MemoryStream(new byte[1024 * 1024 * 100]));

            Stopwatch watch = new Stopwatch();
            watch.Start();

            //while (stream1.Position < stream1.Length) {
            //    stream1.ReadByte();
            //}

            //while (bitReader.BaseStream.Position < bitReader.BaseStream.Length) {
            //    bitReader.ReadBits(8);
            //}

            bitReader.ReadBitArray((int)bitReader.BaseStream.Length, 8);

            watch.Stop();
            Console.WriteLine($"Elapsed milliseconds: {watch.ElapsedMilliseconds}");

            Console.ReadLine();
        }
    }
}
