// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace SmartCamera
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct GeneralObject : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_1_21(); }
  public static GeneralObject GetRootAsGeneralObject(ByteBuffer _bb) { return GetRootAsGeneralObject(_bb, new GeneralObject()); }
  public static GeneralObject GetRootAsGeneralObject(ByteBuffer _bb, GeneralObject obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public GeneralObject __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public uint ClassId { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint)0; } }
  public SmartCamera.BoundingBox BoundingBoxType { get { int o = __p.__offset(6); return o != 0 ? (SmartCamera.BoundingBox)__p.bb.Get(o + __p.bb_pos) : SmartCamera.BoundingBox.NONE; } }
  public TTable? BoundingBox<TTable>() where TTable : struct, IFlatbufferObject { int o = __p.__offset(8); return o != 0 ? (TTable?)__p.__union<TTable>(o + __p.bb_pos) : null; }
  public SmartCamera.BoundingBox2d BoundingBoxAsBoundingBox2d() { return BoundingBox<SmartCamera.BoundingBox2d>().Value; }
  public float Score { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }

  public static Offset<SmartCamera.GeneralObject> CreateGeneralObject(FlatBufferBuilder builder,
      uint class_id = 0,
      SmartCamera.BoundingBox bounding_box_type = SmartCamera.BoundingBox.NONE,
      int bounding_boxOffset = 0,
      float score = 0.0f) {
    builder.StartTable(4);
    GeneralObject.AddScore(builder, score);
    GeneralObject.AddBoundingBox(builder, bounding_boxOffset);
    GeneralObject.AddClassId(builder, class_id);
    GeneralObject.AddBoundingBoxType(builder, bounding_box_type);
    return GeneralObject.EndGeneralObject(builder);
  }

  public static void StartGeneralObject(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddClassId(FlatBufferBuilder builder, uint classId) { builder.AddUint(0, classId, 0); }
  public static void AddBoundingBoxType(FlatBufferBuilder builder, SmartCamera.BoundingBox boundingBoxType) { builder.AddByte(1, (byte)boundingBoxType, 0); }
  public static void AddBoundingBox(FlatBufferBuilder builder, int boundingBoxOffset) { builder.AddOffset(2, boundingBoxOffset, 0); }
  public static void AddScore(FlatBufferBuilder builder, float score) { builder.AddFloat(3, score, 0.0f); }
  public static Offset<SmartCamera.GeneralObject> EndGeneralObject(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<SmartCamera.GeneralObject>(o);
  }
}


}
