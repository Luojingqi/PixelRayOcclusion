// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace PRO.Flat
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct StartCombatEffectData : IFlatbufferObject
{
  private Struct __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public void __init(int _i, ByteBuffer _bb) { __p = new Struct(_i, _bb); }
  public StartCombatEffectData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Type { get { return __p.bb.GetInt(__p.bb_pos + 0); } }
  public int Value { get { return __p.bb.GetInt(__p.bb_pos + 4); } }

  public static Offset<PRO.Flat.StartCombatEffectData> CreateStartCombatEffectData(FlatBufferBuilder builder, int Type, int Value) {
    builder.Prep(4, 8);
    builder.PutInt(Value);
    builder.PutInt(Type);
    return new Offset<PRO.Flat.StartCombatEffectData>(builder.Offset);
  }
}

public struct CombatContext_ByAgentData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static CombatContext_ByAgentData GetRootAsCombatContext_ByAgentData(ByteBuffer _bb) { return GetRootAsCombatContext_ByAgentData(_bb, new CombatContext_ByAgentData()); }
  public static CombatContext_ByAgentData GetRootAsCombatContext_ByAgentData(ByteBuffer _bb, CombatContext_ByAgentData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public CombatContext_ByAgentData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string RoleGuid { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRoleGuidBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetRoleGuidBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetRoleGuidArray() { return __p.__vector_as_array<byte>(4); }
  public PRO.Flat.RoleInfoData? RoleInfo { get { int o = __p.__offset(6); return o != 0 ? (PRO.Flat.RoleInfoData?)(new PRO.Flat.RoleInfoData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
  public PRO.Flat.StartCombatEffectData? StartCombatEffectDataList(int j) { int o = __p.__offset(8); return o != 0 ? (PRO.Flat.StartCombatEffectData?)(new PRO.Flat.StartCombatEffectData()).__assign(__p.__vector(o) + j * 8, __p.bb) : null; }
  public int StartCombatEffectDataListLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public bool PlayAffectedAnimation { get { int o = __p.__offset(10); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public string LogBuilder { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetLogBuilderBytes() { return __p.__vector_as_span<byte>(12, 1); }
#else
  public ArraySegment<byte>? GetLogBuilderBytes() { return __p.__vector_as_arraysegment(12); }
#endif
  public byte[] GetLogBuilderArray() { return __p.__vector_as_array<byte>(12); }

  public static Offset<PRO.Flat.CombatContext_ByAgentData> CreateCombatContext_ByAgentData(FlatBufferBuilder builder,
      StringOffset role_guidOffset = default(StringOffset),
      Offset<PRO.Flat.RoleInfoData> role_infoOffset = default(Offset<PRO.Flat.RoleInfoData>),
      VectorOffset start_combat_effect_data_listOffset = default(VectorOffset),
      bool play_affected_animation = false,
      StringOffset log_builderOffset = default(StringOffset)) {
    builder.StartTable(5);
    CombatContext_ByAgentData.AddLogBuilder(builder, log_builderOffset);
    CombatContext_ByAgentData.AddStartCombatEffectDataList(builder, start_combat_effect_data_listOffset);
    CombatContext_ByAgentData.AddRoleInfo(builder, role_infoOffset);
    CombatContext_ByAgentData.AddRoleGuid(builder, role_guidOffset);
    CombatContext_ByAgentData.AddPlayAffectedAnimation(builder, play_affected_animation);
    return CombatContext_ByAgentData.EndCombatContext_ByAgentData(builder);
  }

  public static void StartCombatContext_ByAgentData(FlatBufferBuilder builder) { builder.StartTable(5); }
  public static void AddRoleGuid(FlatBufferBuilder builder, StringOffset roleGuidOffset) { builder.AddOffset(0, roleGuidOffset.Value, 0); }
  public static void AddRoleInfo(FlatBufferBuilder builder, Offset<PRO.Flat.RoleInfoData> roleInfoOffset) { builder.AddOffset(1, roleInfoOffset.Value, 0); }
  public static void AddStartCombatEffectDataList(FlatBufferBuilder builder, VectorOffset startCombatEffectDataListOffset) { builder.AddOffset(2, startCombatEffectDataListOffset.Value, 0); }
  public static void StartStartCombatEffectDataListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static void AddPlayAffectedAnimation(FlatBufferBuilder builder, bool playAffectedAnimation) { builder.AddBool(3, playAffectedAnimation, false); }
  public static void AddLogBuilder(FlatBufferBuilder builder, StringOffset logBuilderOffset) { builder.AddOffset(4, logBuilderOffset.Value, 0); }
  public static Offset<PRO.Flat.CombatContext_ByAgentData> EndCombatContext_ByAgentData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.Flat.CombatContext_ByAgentData>(o);
  }
}


static public class CombatContext_ByAgentDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyString(tablePos, 4 /*RoleGuid*/, false)
      && verifier.VerifyTable(tablePos, 6 /*RoleInfo*/, PRO.Flat.RoleInfoDataVerify.Verify, false)
      && verifier.VerifyVectorOfData(tablePos, 8 /*StartCombatEffectDataList*/, 8 /*PRO.Flat.StartCombatEffectData*/, false)
      && verifier.VerifyField(tablePos, 10 /*PlayAffectedAnimation*/, 1 /*bool*/, 1, false)
      && verifier.VerifyString(tablePos, 12 /*LogBuilder*/, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct CombatContextData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static CombatContextData GetRootAsCombatContextData(ByteBuffer _bb) { return GetRootAsCombatContextData(_bb, new CombatContextData()); }
  public static CombatContextData GetRootAsCombatContextData(ByteBuffer _bb, CombatContextData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public CombatContextData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string RoleGuid { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRoleGuidBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetRoleGuidBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetRoleGuidArray() { return __p.__vector_as_array<byte>(4); }
  public PRO.Flat.RoleInfoData? RoleInfo { get { int o = __p.__offset(6); return o != 0 ? (PRO.Flat.RoleInfoData?)(new PRO.Flat.RoleInfoData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
  public PRO.Flat.StartCombatEffectData? StartCombatEffectDataList(int j) { int o = __p.__offset(8); return o != 0 ? (PRO.Flat.StartCombatEffectData?)(new PRO.Flat.StartCombatEffectData()).__assign(__p.__vector(o) + j * 8, __p.bb) : null; }
  public int StartCombatEffectDataListLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public int CastASpellType { get { int o = __p.__offset(10); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public string LogBuilder { get { int o = __p.__offset(12); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetLogBuilderBytes() { return __p.__vector_as_span<byte>(12, 1); }
#else
  public ArraySegment<byte>? GetLogBuilderBytes() { return __p.__vector_as_arraysegment(12); }
#endif
  public byte[] GetLogBuilderArray() { return __p.__vector_as_array<byte>(12); }
  public PRO.Flat.CombatContext_ByAgentData? ByAgentDataList(int j) { int o = __p.__offset(14); return o != 0 ? (PRO.Flat.CombatContext_ByAgentData?)(new PRO.Flat.CombatContext_ByAgentData()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int ByAgentDataListLength { get { int o = __p.__offset(14); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<PRO.Flat.CombatContextData> CreateCombatContextData(FlatBufferBuilder builder,
      StringOffset role_guidOffset = default(StringOffset),
      Offset<PRO.Flat.RoleInfoData> role_infoOffset = default(Offset<PRO.Flat.RoleInfoData>),
      VectorOffset start_combat_effect_data_listOffset = default(VectorOffset),
      int cast_a_spell_type = 0,
      StringOffset log_builderOffset = default(StringOffset),
      VectorOffset by_agent_data_listOffset = default(VectorOffset)) {
    builder.StartTable(6);
    CombatContextData.AddByAgentDataList(builder, by_agent_data_listOffset);
    CombatContextData.AddLogBuilder(builder, log_builderOffset);
    CombatContextData.AddCastASpellType(builder, cast_a_spell_type);
    CombatContextData.AddStartCombatEffectDataList(builder, start_combat_effect_data_listOffset);
    CombatContextData.AddRoleInfo(builder, role_infoOffset);
    CombatContextData.AddRoleGuid(builder, role_guidOffset);
    return CombatContextData.EndCombatContextData(builder);
  }

  public static void StartCombatContextData(FlatBufferBuilder builder) { builder.StartTable(6); }
  public static void AddRoleGuid(FlatBufferBuilder builder, StringOffset roleGuidOffset) { builder.AddOffset(0, roleGuidOffset.Value, 0); }
  public static void AddRoleInfo(FlatBufferBuilder builder, Offset<PRO.Flat.RoleInfoData> roleInfoOffset) { builder.AddOffset(1, roleInfoOffset.Value, 0); }
  public static void AddStartCombatEffectDataList(FlatBufferBuilder builder, VectorOffset startCombatEffectDataListOffset) { builder.AddOffset(2, startCombatEffectDataListOffset.Value, 0); }
  public static void StartStartCombatEffectDataListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(8, numElems, 4); }
  public static void AddCastASpellType(FlatBufferBuilder builder, int castASpellType) { builder.AddInt(3, castASpellType, 0); }
  public static void AddLogBuilder(FlatBufferBuilder builder, StringOffset logBuilderOffset) { builder.AddOffset(4, logBuilderOffset.Value, 0); }
  public static void AddByAgentDataList(FlatBufferBuilder builder, VectorOffset byAgentDataListOffset) { builder.AddOffset(5, byAgentDataListOffset.Value, 0); }
  public static VectorOffset CreateByAgentDataListVector(FlatBufferBuilder builder, Offset<PRO.Flat.CombatContext_ByAgentData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateByAgentDataListVectorBlock(FlatBufferBuilder builder, Offset<PRO.Flat.CombatContext_ByAgentData>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateByAgentDataListVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<PRO.Flat.CombatContext_ByAgentData>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateByAgentDataListVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<PRO.Flat.CombatContext_ByAgentData>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartByAgentDataListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<PRO.Flat.CombatContextData> EndCombatContextData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.Flat.CombatContextData>(o);
  }
}


static public class CombatContextDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyString(tablePos, 4 /*RoleGuid*/, false)
      && verifier.VerifyTable(tablePos, 6 /*RoleInfo*/, PRO.Flat.RoleInfoDataVerify.Verify, false)
      && verifier.VerifyVectorOfData(tablePos, 8 /*StartCombatEffectDataList*/, 8 /*PRO.Flat.StartCombatEffectData*/, false)
      && verifier.VerifyField(tablePos, 10 /*CastASpellType*/, 4 /*int*/, 4, false)
      && verifier.VerifyString(tablePos, 12 /*LogBuilder*/, false)
      && verifier.VerifyVectorOfTables(tablePos, 14 /*ByAgentDataList*/, PRO.Flat.CombatContext_ByAgentDataVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
