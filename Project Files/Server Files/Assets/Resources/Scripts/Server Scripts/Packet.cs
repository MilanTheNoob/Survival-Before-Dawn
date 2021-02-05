using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>Sent from server to client.</summary>
public enum ServerPackets
{
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerDisconnected,
    chunkData,
    propChunkUpdate,
    itemChunkUpdate,
    structuresChunkUpdate,
    interact,
    stopInteract,
    updateInventory,
    updateVitals,
    treeInteract
}

/// <summary>Sent from client to server.</summary>
public enum ClientPackets
{
    welcomeReceived = 1,
    playerMovement,
    interact,
    modifyVital,
    destroyProp,
    addProp
}

public class Packet : IDisposable
{
    private List<byte> buffer;
    private byte[] readableBuffer;
    private int readPos;

    public Packet() { buffer = new List<byte>(); readPos = 0; }
    public Packet(int _id) { buffer = new List<byte>(); readPos = 0; Write(_id); }
    public Packet(byte[] _data) { buffer = new List<byte>(); readPos = 0; SetBytes(_data); }

    #region Functions

    public void SetBytes(byte[] _data) { Write(_data); readableBuffer = buffer.ToArray(); }
    public void WriteLength() { buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); }
    public void InsertInt(int _value) { buffer.InsertRange(0, BitConverter.GetBytes(_value)); }
    public byte[] ToArray() { readableBuffer = buffer.ToArray(); return readableBuffer; }
    public int Length() { return buffer.Count; }
    public int UnreadLength() { return Length() - readPos; }

    public void Reset(bool _shouldReset = true)
    {
        if (_shouldReset)
        {
            buffer.Clear();
            readableBuffer = null;
            readPos = 0;
        }
        else { readPos -= 4; }
    }

    #endregion

    #region Write Data

    public void Write(byte _value) { buffer.Add(_value); }
    public void Write(byte[] _value) { buffer.AddRange(_value); }
    public void Write(short _value) { buffer.AddRange(BitConverter.GetBytes(_value)); }
    public void Write(int _value) { buffer.AddRange(BitConverter.GetBytes(_value)); }
    public void Write(long _value) { buffer.AddRange(BitConverter.GetBytes(_value)); }
    public void Write(float _value) { buffer.AddRange(BitConverter.GetBytes(_value)); }
    public void Write(bool _value) { buffer.AddRange(BitConverter.GetBytes(_value)); }
    public void Write(string _value) { Write(_value.Length); buffer.AddRange(Encoding.ASCII.GetBytes(_value)); }
    public void Write(Vector2 _value) { Write(_value.x); Write(_value.y); }
    public void Write(Vector3 _value) { Write(_value.x); Write(_value.y); Write(_value.z); }
    public void Write(Quaternion _value) { Write(_value.x); Write(_value.y); Write(_value.z); Write(_value.w); }

    public void Write(List<Vector3> _value)
    {
        Write(_value.Count);
        for (int i = 0; i < _value.Count; i++) { Write(_value[i]); }
    }
    public void Write(List<Quaternion> _value)
    {
        Write(_value.Count);
        for (int i = 0; i < _value.Count; i++) { Write(_value[i]); }
    }
    public void Write(List<int> _value)
    {
        Write(_value.Count);
        for (int i = 0; i < _value.Count; i++) { Write(_value[i]); }
    }
    public void Write(List<string> _value)
    {
        Write(_value.Count);
        for (int i = 0; i < _value.Count; i++) { Write(_value[i]); }
    }

    public void Write(HeightMap heightMap, int width, int height)
    {
        Write(height);
        Write(width);

        Write(heightMap.minValue);
        Write(heightMap.maxValue);

        for (int i = 0; i < width; i++) { for (int j = 0; j < height; j++) { Write(heightMap.values[i, j]); } }
    }
    #endregion

    #region Read Data
    public byte ReadByte(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            byte _value = readableBuffer[readPos];
            if (_moveReadPos) { readPos += 1; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'byte'!"); }
    }
    public byte[] ReadBytes(int _length, bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            byte[] _value = buffer.GetRange(readPos, _length).ToArray();
            if (_moveReadPos) { readPos += _length; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'byte[]'!"); }
    }
    public short ReadShort(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            short _value = BitConverter.ToInt16(readableBuffer, readPos);
            if (_moveReadPos) { readPos += 2; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'short'!"); }
    }
    public int ReadInt(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            int _value = BitConverter.ToInt32(readableBuffer, readPos);
            if (_moveReadPos) { readPos += 4; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'int'!"); }
    }
    public long ReadLong(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            long _value = BitConverter.ToInt64(readableBuffer, readPos);
            if (_moveReadPos) { readPos += 8; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'long'!"); }
    }
    public float ReadFloat(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            float _value = BitConverter.ToSingle(readableBuffer, readPos);
            if (_moveReadPos) { readPos += 4; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'float'!"); }
    }
    public bool ReadBool(bool _moveReadPos = true)
    {
        if (buffer.Count > readPos)
        {
            bool _value = BitConverter.ToBoolean(readableBuffer, readPos);
            if (_moveReadPos) { readPos += 1; }
            return _value;
        }
        else { throw new Exception("Could not read value of type 'bool'!"); }
    }
    public string ReadString(bool _moveReadPos = true)
    {
        try
        {
            int _length = ReadInt();
            string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length);
            if (_moveReadPos && _value.Length > 0) { readPos += _length; }
            return _value;
        }
        catch { throw new Exception("Could not read value of type 'string'!"); }
    }

    public Vector2 ReadVector2(bool _moveReadPos = true) { return new Vector2(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos)); }
    public Vector3 ReadVector3(bool _moveReadPos = true) { return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos)); }
    public Quaternion ReadQuaternion(bool _moveReadPos = true) { return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos)); }

    public List<Vector3> ReadVector3List(bool _moveReadPos = true)
    {
        List<Vector3> list = new List<Vector3>();
        int _length = ReadInt();
        for (int i = 0; i < _length; i++) { list.Add(ReadVector3(_moveReadPos)); }
        return list;
    }
    public List<Quaternion> ReadQuaternionList(bool _moveReadPos = true)
    {
        List<Quaternion> list = new List<Quaternion>();
        int _length = ReadInt();
        for (int i = 0; i < _length; i++) { list.Add(ReadQuaternion(_moveReadPos)); }
        return list;
    }
    public List<int> ReadIntList(bool _moveReadPos = true)
    {
        List<int> list = new List<int>();
        int _length = ReadInt();
        for (int i = 0; i < _length; i++) { list.Add(ReadInt(_moveReadPos)); }
        return list;
    }
    public List<string> ReadStringList(bool _moveReadPos = true)
    {
        List<string> list = new List<string>();
        int _length = ReadInt();
        for (int i = 0; i < _length; i++) { list.Add(ReadString(_moveReadPos)); }
        return list;
    }

    public HeightMap ReadHeightMap(bool _moveReadPos = true)
    {
        int height = ReadInt(_moveReadPos);
        int width = ReadInt(_moveReadPos);

        float minValue = ReadFloat(_moveReadPos);
        float maxValue = ReadFloat(_moveReadPos);

        float[,] values = new float[width, height];
        for (int i = 0; i < width; i++) { for (int j = 0; j < height; j++) { values[i, j] = ReadFloat(_moveReadPos); } }

        return new HeightMap(values, minValue, maxValue);
    }

    #endregion

    private bool disposed = false;

    void Dispose(bool _disposing)
    {
        if (!disposed)
        {
            if (_disposing)
            {
                buffer = null;
                readableBuffer = null;
                readPos = 0;
            }

            disposed = true;
        }
    }
    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
}
