// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace SmartCamera
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct ObjectDetectionTop : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_1_21(); }
  public static ObjectDetectionTop GetRootAsObjectDetectionTop(ByteBuffer _bb) { return GetRootAsObjectDetectionTop(_bb, new ObjectDetectionTop()); }
  public static ObjectDetectionTop GetRootAsObjectDetectionTop(ByteBuffer _bb, ObjectDetectionTop obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public ObjectDetectionTop __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public SmartCamera.ObjectDetectionData? Perception { get { int o = __p.__offset(4); return o != 0 ? (SmartCamera.ObjectDetectionData?)(new SmartCamera.ObjectDetectionData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

  public static Offset<SmartCamera.ObjectDetectionTop> CreateObjectDetectionTop(FlatBufferBuilder builder,
      Offset<SmartCamera.ObjectDetectionData> perceptionOffset = default(Offset<SmartCamera.ObjectDetectionData>)) {
    builder.StartTable(1);
    ObjectDetectionTop.AddPerception(builder, perceptionOffset);
    return ObjectDetectionTop.EndObjectDetectionTop(builder);
  }

  public static void StartObjectDetectionTop(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddPerception(FlatBufferBuilder builder, Offset<SmartCamera.ObjectDetectionData> perceptionOffset) { builder.AddOffset(0, perceptionOffset.Value, 0); }
  public static Offset<SmartCamera.ObjectDetectionTop> EndObjectDetectionTop(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<SmartCamera.ObjectDetectionTop>(o);
  }
  public static void FinishObjectDetectionTopBuffer(FlatBufferBuilder builder, Offset<SmartCamera.ObjectDetectionTop> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedObjectDetectionTopBuffer(FlatBufferBuilder builder, Offset<SmartCamera.ObjectDetectionTop> offset) { builder.FinishSizePrefixed(offset.Value); }
}


}
