using System.Runtime.CompilerServices;

namespace CashPack
{
    internal static class BitHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong KeyToNumber(byte[] key, int totalBits)
        {
            int firstByteBits = totalBits % 8;
            int wholeBytes = (totalBits - firstByteBits) / 8;

            byte halfByte = key[key.Length - wholeBytes - 1];
            
            ulong keyToNumber = (firstByteBits == 0 ? 0U : ((byte)(((byte)(halfByte << (8 - firstByteBits))) >> (8 - firstByteBits))));
            
            for (int i = (key.Length) - wholeBytes; i < key.Length; i++)
            {
                keyToNumber = keyToNumber << 8;
                keyToNumber |= key[i];
            }
            
            return keyToNumber;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetLastBitNumberBytes(byte[] key, ulong number, int totalBits)
        {
            int firstByteBits = totalBits % 8;
            int wholeBytes = (totalBits - firstByteBits) / 8;
            
            // First byte
            byte halfByte = key[key.Length - wholeBytes - 1];

            // Drop the last bits
            halfByte = ((byte) (((byte) (halfByte >> firstByteBits)) << firstByteBits));

            // Get the mask
            byte lastByteFromNumber = (byte)(number >> (wholeBytes * 8));

            // Set value
            halfByte |= lastByteFromNumber;

            // Set in key
            key[key.Length - wholeBytes - 1] = halfByte;
            
            for (int i = key.Length - wholeBytes; i < key.Length; i++)
            {
                byte value = (byte)(((number >> ((key.Length - i - 1) * 8)) & 255));
                key[i] = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ReduceKey(byte[] key, int totalBits)
        {
            int firstByteBits = totalBits % 8;
            int wholeBytes = (totalBits - firstByteBits) / 8;
            
            byte halfByte = key[key.Length - wholeBytes - 1];

            key[key.Length - wholeBytes - 1] = (byte)((byte)(halfByte >> firstByteBits) << firstByteBits);

            for (int i = key.Length - wholeBytes; i < key.Length; i++)
            {
                key[i] = 0;
            }
        }
    }
}