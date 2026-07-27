using UnityEngine;
using System.IO;

public static class BinarySerializationExtension
{
    // Writes Vector2 components as two sequential 4-byte floats
    public static void Write(this BinaryWriter writer, Vector2 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }

    // Reads two sequential 4-byte floats and returns a Vector2
    public static Vector2 ReadVector2(this BinaryReader reader)
    {
        float x = reader.ReadSingle();
        float y = reader.ReadSingle();
        return new Vector2(x, y);
    }

}
