using System;
using System.Runtime.InteropServices;

namespace SoundMixerSoftware.Common.Utils
{
    /// <summary>
    /// Contains useful methods while working with structures.
    /// </summary>
    public static class StructUtil
    {
        /// <summary>
        /// Converts byte array to struct.
        /// </summary>
        /// <param name="buffer"></param>
        /// <typeparam name="T">type of struct.</typeparam>
        /// <returns>STRUCT</returns>
        public static T StructFromBytes<T>(byte[] buffer) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, ptr, size);
            var result = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return result;
        }
        
        /// <summary>
        /// Converts byte array to struct(unsafe).
        /// </summary>
        /// <param name="buffer"></param>
        /// <typeparam name="T">type of struct.</typeparam>
        /// <returns>STRUCT</returns>
        public static unsafe T StructFromBytesUnsafe<T>(byte[] buffer) where T : struct
        {
            fixed (byte* ptr = &buffer[0])
            {
                return Marshal.PtrToStructure<T>((IntPtr)ptr);
            }
        }

        /// <summary>
        /// Converts struct to byte array.
        /// </summary>
        /// <param name="structure"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static byte[] StructToBytes<T>(T structure) where T : struct
        {
            var size = Marshal.SizeOf(typeof(T));
            var ptr = Marshal.AllocHGlobal(size);
            var result = new byte[size];
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, result, 0, size);
            Marshal.FreeHGlobal(ptr);
            return result;
        }
    }
}