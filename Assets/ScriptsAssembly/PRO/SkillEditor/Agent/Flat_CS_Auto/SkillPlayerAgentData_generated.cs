// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace PRO.Flat
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct SkillPlayerAgentData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static SkillPlayerAgentData GetRootAsSkillPlayerAgentData(ByteBuffer _bb) { return GetRootAsSkillPlayerAgentData(_bb, new SkillPlayerAgentData()); }
  public static SkillPlayerAgentData GetRootAsSkillPlayerAgentData(ByteBuffer _bb, SkillPlayerAgentData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public SkillPlayerAgentData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public bool Play { get { int o = __p.__offset(4); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public float Time { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetFloat(o + __p.bb_pos) : (float)0.0f; } }
  public int NowFrame { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public string IdleLoadPath { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetIdleLoadPathBytes() { return __p.__vector_as_span<byte>(10, 1); }
#else
  public ArraySegment<byte>? GetIdleLoadPathBytes() { return __p.__vector_as_arraysegment(10); }
#endif
  public byte[] GetIdleLoadPathArray() { return __p.__vector_as_array<byte>(10); }
  public string SkillLoadPath { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetSkillLoadPathBytes() { return __p.__vector_as_span<byte>(12, 1); }
#else
  public ArraySegment<byte>? GetSkillLoadPathBytes() { return __p.__vector_as_arraysegment(12); }
#endif
  public byte[] GetSkillLoadPathArray() { return __p.__vector_as_array<byte>(12); }

  public static Offset<PRO.Flat.SkillPlayerAgentData> CreateSkillPlayerAgentData(FlatBufferBuilder builder,
      bool play = false,
      float time = 0.0f,
      int now_frame = 0,
      StringOffset idle_load_pathOffset = default(StringOffset),
      StringOffset skill_load_pathOffset = default(StringOffset)) {
    builder.StartTable(5);
    SkillPlayerAgentData.AddSkillLoadPath(builder, skill_load_pathOffset);
    SkillPlayerAgentData.AddIdleLoadPath(builder, idle_load_pathOffset);
    SkillPlayerAgentData.AddNowFrame(builder, now_frame);
    SkillPlayerAgentData.AddTime(builder, time);
    SkillPlayerAgentData.AddPlay(builder, play);
    return SkillPlayerAgentData.EndSkillPlayerAgentData(builder);
  }

  public static void StartSkillPlayerAgentData(FlatBufferBuilder builder) { builder.StartTable(5); }
  public static void AddPlay(FlatBufferBuilder builder, bool play) { builder.AddBool(0, play, false); }
  public static void AddTime(FlatBufferBuilder builder, float time) { builder.AddFloat(1, time, 0.0f); }
  public static void AddNowFrame(FlatBufferBuilder builder, int nowFrame) { builder.AddInt(2, nowFrame, 0); }
  public static void AddIdleLoadPath(FlatBufferBuilder builder, StringOffset idleLoadPathOffset) { builder.AddOffset(3, idleLoadPathOffset.Value, 0); }
  public static void AddSkillLoadPath(FlatBufferBuilder builder, StringOffset skillLoadPathOffset) { builder.AddOffset(4, skillLoadPathOffset.Value, 0); }
  public static Offset<PRO.Flat.SkillPlayerAgentData> EndSkillPlayerAgentData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.Flat.SkillPlayerAgentData>(o);
  }
}


static public class SkillPlayerAgentDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*Play*/, 1 /*bool*/, 1, false)
      && verifier.VerifyField(tablePos, 6 /*Time*/, 4 /*float*/, 4, false)
      && verifier.VerifyField(tablePos, 8 /*NowFrame*/, 4 /*int*/, 4, false)
      && verifier.VerifyString(tablePos, 10 /*IdleLoadPath*/, false)
      && verifier.VerifyString(tablePos, 12 /*SkillLoadPath*/, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
