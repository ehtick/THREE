﻿using Newtonsoft.Json;
using Silk.NET.OpenGLES;
using System;
using System.Collections.Generic;


namespace THREE
{
    [Serializable]
    public class BufferType
    {
        public int buffer;

        public int Type;

        public int BytesPerElement;

        public int Version;

    }

    [Serializable]
    public class GLAttributes : Dictionary<object, object>
    {
        // buffers = this
        GL gl;
        public GLAttributes(GL gl)
        {
            this.gl = gl;
        }

        public unsafe BufferType CreateBuffer<T>(IBufferAttribute attribute, GLEnum bufferType)
        {
            var attr = attribute as BufferAttribute<T>;
            T[] array = attr.Array;
            GLEnum usage = (GLEnum)attr.Usage;

            uint buffer;

            int type = (int)VertexAttribPointerType.Float;

            int bytePerElement = 4;

            gl.GenBuffers(1, out buffer);
            gl.BindBuffer(bufferType, buffer);


            if (attribute.Type == typeof(float)) 
            {
                if (array.Length > 0)
                {
                    //gl.BufferData(bufferType, (array.Length * sizeof(float)), array as float[], usage);
                    fixed (float* p = &(array as float[])[0])
                    {
                        gl.BufferData(bufferType, (uint)(array.Length * sizeof(float)), p, usage);
                    }
                    type = (int)VertexAttribPointerType.Float;
                    bytePerElement = sizeof(float);
                }
            }
            else if (attribute.Type == typeof(System.Single))
            {
                //gl.BufferData(bufferType, (array.Length * sizeof(float)), array as float[], usage);
                fixed (System.Single* p = &(array as System.Single[])[0])
                {
                    gl.BufferData(bufferType, (uint)(array.Length * sizeof(System.Single)), p, usage);
                }
                type = (int)VertexAttribPointerType.Float;
                bytePerElement = sizeof(float);
            }
            else if (attribute.Type == typeof(int))
            {
                //gl.BufferData(bufferType, (array.Length * sizeof(int)), array as int[], usage);
                fixed (int* p = &(array as int[])[0])
                {
                    gl.BufferData(bufferType, (uint)(array.Length * sizeof(int)), p, usage);
                }
                type = (int)VertexAttribPointerType.UnsignedInt;
                bytePerElement = sizeof(int);
            }
            else if (attribute.Type == typeof(uint))
            {
                //gl.BufferData(bufferType, (array.Length * sizeof(uint)), array as uint[], usage);
                fixed (uint* p = &(array as uint[])[0])
                {
                    gl.BufferData(bufferType, (uint)(array.Length * sizeof(uint)), p, usage);
                }
                type = (int)VertexAttribPointerType.UnsignedInt;
                bytePerElement = sizeof(uint);
            }
            else if (attribute.Type == typeof(byte))
            {
                //gl.BufferData(bufferType, (array.Length * sizeof(byte)), array as byte[], usage);
                fixed (byte* p = &(array as byte[])[0])
                {
                    gl.BufferData(bufferType, (uint)(array.Length * sizeof(byte)), p, usage);
                }
                type = (int)VertexAttribPointerType.UnsignedInt;
                bytePerElement = sizeof(byte);
            }
            else
            {
                //gl.BufferData(bufferType, (array.Length * 2), array as short[], usage);
                fixed (short* p = &(array as short[])[0])
                {
                    gl.BufferData(bufferType, (uint)(array.Length * sizeof(short)), p, usage);
                }
                type = (int)VertexAttribPointerType.UnsignedShort;
                bytePerElement = sizeof(short);
            }


            return new BufferType { buffer = (int)buffer, Type = type, BytesPerElement = bytePerElement, Version = attr.Version };
        }

        public unsafe void UpdateBuffer<T>(int buffer, IBufferAttribute attribute, GLEnum bufferType)
        {
            var attr = attribute as BufferAttribute<T>;
            var array = attr.Array;
            var updateRange = attr.UpdateRange;

            gl.BindBuffer(bufferType, (uint)buffer);

            if (updateRange.Count == -1)
            {
                if (null != array as float[])
                {
                    //gl.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(float), array as float[]);
                    fixed (float* p = &(array as float[])[0])
                    {
                        gl.BufferSubData(bufferType, IntPtr.Zero, (uint)array.Length * sizeof(float), p);
                    }
                   
                }
                else if (null != array as ushort[])
                {
                    //gl.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(ushort), array as ushort[]);
                    fixed (ushort* p = &(array as ushort[])[0])
                    {
                        gl.BufferSubData(bufferType, IntPtr.Zero, (uint)array.Length * sizeof(ushort), p);
                    }
                }
                else if (null != array as uint[])
                {
                    //gl.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(uint), array as uint[]);
                    fixed (uint* p = &(array as uint[])[0])
                    {
                        gl.BufferSubData(bufferType, IntPtr.Zero, (uint)array.Length * sizeof(uint), p);
                    }
                }
                else if (null != array as int[])
                {
                    //gl.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(uint), array as uint[]);
                    fixed (int* p = &(array as int[])[0])
                    {
                        gl.BufferSubData(bufferType, IntPtr.Zero, (uint)array.Length * sizeof(int), p);
                    }
                }
                else if (null != array as byte[])
                {
                    //gl.BufferSubData(bufferType, IntPtr.Zero, attribute.Length * sizeof(byte), array as byte[]);
                    fixed (byte* p = &(array as byte[])[0])
                    {
                        gl.BufferSubData(bufferType, IntPtr.Zero, (uint)array.Length * sizeof(byte), p);
                    }
                }
            }
            else
            {
                int length = updateRange.Offset + updateRange.Count;
                int startIndex = updateRange.Offset;

                T[] subarray = new T[length];

                Array.Copy(array, startIndex, subarray, 0, length);

                if (null != array as float[])
                {
                    //gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(float)), length * sizeof(float), subarray as float[]);
                    fixed(float* p = &(array as float[])[0])
                    {
                        gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(float)),(uint)(array.Length * sizeof(float)), p);
                    }
                }
                else if (null != array as ushort[])
                {
                    //gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(ushort)), length * sizeof(float), subarray as float[]);
                    fixed (ushort* p = &(array as ushort[])[0])
                    {
                        gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(ushort)), (uint)(array.Length * sizeof(ushort)), p);
                    }
                }
                else if (null != array as uint[])
                {
                    //gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(uint)), length * sizeof(uint), subarray as uint[]);
                    fixed (uint* p = &(array as uint[])[0])
                    {
                        gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(uint)), (uint)(array.Length * sizeof(uint)), p);
                    }
                }
                else if (null != array as int[])
                {
                    //gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(uint)), length * sizeof(uint), subarray as uint[]);
                    fixed (int* p = &(array as int[])[0])
                    {
                        gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(int)), (uint)(array.Length * sizeof(int)), p);
                    }
                }
                else if (null != array as byte[])
                {
                    //gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(byte)), length * sizeof(byte), subarray as byte[]);
                    fixed (byte* p = &(array as byte[])[0])
                    {
                        gl.BufferSubData(bufferType, new IntPtr(updateRange.Offset * sizeof(byte)), (uint)(array.Length * sizeof(byte)), p);
                    }
                }

                (attribute as BufferAttribute<T>).UpdateRange.Count = -1;

            }
        }

        public BufferType Get<T>(object attribute)
        {
            //if(!this.ContainsKey(attribute))
            //{
            //    this.Add(attribute,new BufferType());
            //}
            if (attribute is InterleavedBufferAttribute<T>) attribute = (attribute as InterleavedBufferAttribute<T>).Data;


            return this.ContainsKey(attribute) ? (BufferType)this[attribute] : null;
        }

        //public void Remove(string attribute)
        //{
        //    this.Remove(attribute);
        //}
        public void UpdateBufferAttribute(GLBufferAttribute attribute, GLEnum bufferType)
        {
            BufferType cached = this[attribute] as BufferType;
            if (cached != null || cached.Version < attribute.Version)
            {
                this.Add(attribute, new BufferType { buffer = attribute.Buffer, Type = attribute.Type, BytesPerElement = attribute.ElementSize, Version = attribute.Version });
            }
        }
        public void Update<T>(IBufferAttribute attribute, GLEnum bufferType)
        {
            if (attribute is InterleavedBufferAttribute<T>)
                attribute = (attribute as InterleavedBufferAttribute<T>).Data;


            BufferType data = this.Get<T>(attribute);

            //if (!this.ContainsKey(attribute))
            //{
            //    this.Add(attribute,CreateBuffer(attribute,bufferType));
            //}
            if (data == null)
            {
                this.Add(attribute, CreateBuffer<T>(attribute, bufferType));
            }
            else if (data.Version < (attribute as BufferAttribute<T>).Version)
            {
                UpdateBuffer<T>(data.buffer, attribute, bufferType);
                //BufferType data = (BufferType)this[attribute];
                //if (data.Version < attribute.Version)
                //{
                //    UpdateBuffer<T>(data.buffer, attribute, bufferType);
                //    data.Version = attribute.Version;
                //    this[attribute] = data;
                //}

            }
        }
    }
}
