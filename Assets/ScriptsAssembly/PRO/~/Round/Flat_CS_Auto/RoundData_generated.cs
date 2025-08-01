// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace PRO.TurnBased.Flat
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct OperateT2Data : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static OperateT2Data GetRootAsOperateT2Data(ByteBuffer _bb) { return GetRootAsOperateT2Data(_bb, new OperateT2Data()); }
  public static OperateT2Data GetRootAsOperateT2Data(ByteBuffer _bb, OperateT2Data obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public OperateT2Data __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Guid { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetGuidBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetGuidBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetGuidArray() { return __p.__vector_as_array<byte>(4); }
  public PRO.Flat.CombatContextData? Context { get { int o = __p.__offset(6); return o != 0 ? (PRO.Flat.CombatContextData?)(new PRO.Flat.CombatContextData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }
  public byte ExtendData(int j) { int o = __p.__offset(8); return o != 0 ? __p.bb.Get(__p.__vector(o) + j * 1) : (byte)0; }
  public int ExtendDataLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
#if ENABLE_SPAN_T
  public Span<byte> GetExtendDataBytes() { return __p.__vector_as_span<byte>(8, 1); }
#else
  public ArraySegment<byte>? GetExtendDataBytes() { return __p.__vector_as_arraysegment(8); }
#endif
  public byte[] GetExtendDataArray() { return __p.__vector_as_array<byte>(8); }

  public static Offset<PRO.TurnBased.Flat.OperateT2Data> CreateOperateT2Data(FlatBufferBuilder builder,
      StringOffset guidOffset = default(StringOffset),
      Offset<PRO.Flat.CombatContextData> contextOffset = default(Offset<PRO.Flat.CombatContextData>),
      VectorOffset extend_dataOffset = default(VectorOffset)) {
    builder.StartTable(3);
    OperateT2Data.AddExtendData(builder, extend_dataOffset);
    OperateT2Data.AddContext(builder, contextOffset);
    OperateT2Data.AddGuid(builder, guidOffset);
    return OperateT2Data.EndOperateT2Data(builder);
  }

  public static void StartOperateT2Data(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddGuid(FlatBufferBuilder builder, StringOffset guidOffset) { builder.AddOffset(0, guidOffset.Value, 0); }
  public static void AddContext(FlatBufferBuilder builder, Offset<PRO.Flat.CombatContextData> contextOffset) { builder.AddOffset(1, contextOffset.Value, 0); }
  public static void AddExtendData(FlatBufferBuilder builder, VectorOffset extendDataOffset) { builder.AddOffset(2, extendDataOffset.Value, 0); }
  public static VectorOffset CreateExtendDataVector(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); for (int i = data.Length - 1; i >= 0; i--) builder.AddByte(data[i]); return builder.EndVector(); }
  public static VectorOffset CreateExtendDataVectorBlock(FlatBufferBuilder builder, byte[] data) { builder.StartVector(1, data.Length, 1); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateExtendDataVectorBlock(FlatBufferBuilder builder, ArraySegment<byte> data) { builder.StartVector(1, data.Count, 1); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateExtendDataVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<byte>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartExtendDataVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(1, numElems, 1); }
  public static Offset<PRO.TurnBased.Flat.OperateT2Data> EndOperateT2Data(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.TurnBased.Flat.OperateT2Data>(o);
  }
}


static public class OperateT2DataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyString(tablePos, 4 /*Guid*/, false)
      && verifier.VerifyTable(tablePos, 6 /*Context*/, PRO.Flat.CombatContextDataVerify.Verify, false)
      && verifier.VerifyVectorOfData(tablePos, 8 /*ExtendData*/, 1 /*byte*/, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct TurnState1_OperateData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static TurnState1_OperateData GetRootAsTurnState1_OperateData(ByteBuffer _bb) { return GetRootAsTurnState1_OperateData(_bb, new TurnState1_OperateData()); }
  public static TurnState1_OperateData GetRootAsTurnState1_OperateData(ByteBuffer _bb, TurnState1_OperateData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public TurnState1_OperateData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public PRO.TurnBased.Flat.OperateT2Data? NowOperateListT2(int j) { int o = __p.__offset(4); return o != 0 ? (PRO.TurnBased.Flat.OperateT2Data?)(new PRO.TurnBased.Flat.OperateT2Data()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int NowOperateListT2Length { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<PRO.TurnBased.Flat.TurnState1_OperateData> CreateTurnState1_OperateData(FlatBufferBuilder builder,
      VectorOffset now_operate_list_t2Offset = default(VectorOffset)) {
    builder.StartTable(1);
    TurnState1_OperateData.AddNowOperateListT2(builder, now_operate_list_t2Offset);
    return TurnState1_OperateData.EndTurnState1_OperateData(builder);
  }

  public static void StartTurnState1_OperateData(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddNowOperateListT2(FlatBufferBuilder builder, VectorOffset nowOperateListT2Offset) { builder.AddOffset(0, nowOperateListT2Offset.Value, 0); }
  public static VectorOffset CreateNowOperateListT2Vector(FlatBufferBuilder builder, Offset<PRO.TurnBased.Flat.OperateT2Data>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateNowOperateListT2VectorBlock(FlatBufferBuilder builder, Offset<PRO.TurnBased.Flat.OperateT2Data>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateNowOperateListT2VectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<PRO.TurnBased.Flat.OperateT2Data>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateNowOperateListT2VectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<PRO.TurnBased.Flat.OperateT2Data>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartNowOperateListT2Vector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<PRO.TurnBased.Flat.TurnState1_OperateData> EndTurnState1_OperateData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.TurnBased.Flat.TurnState1_OperateData>(o);
  }
}


static public class TurnState1_OperateDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyVectorOfTables(tablePos, 4 /*NowOperateListT2*/, PRO.TurnBased.Flat.OperateT2DataVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct TurnFSMData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static TurnFSMData GetRootAsTurnFSMData(ByteBuffer _bb) { return GetRootAsTurnFSMData(_bb, new TurnFSMData()); }
  public static TurnFSMData GetRootAsTurnFSMData(ByteBuffer _bb, TurnFSMData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public TurnFSMData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int NowState { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public string RoleGuid { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetRoleGuidBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
  public ArraySegment<byte>? GetRoleGuidBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public byte[] GetRoleGuidArray() { return __p.__vector_as_array<byte>(6); }
  public int Index { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public PRO.TurnBased.Flat.TurnState1_OperateData? StateOperate { get { int o = __p.__offset(10); return o != 0 ? (PRO.TurnBased.Flat.TurnState1_OperateData?)(new PRO.TurnBased.Flat.TurnState1_OperateData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

  public static Offset<PRO.TurnBased.Flat.TurnFSMData> CreateTurnFSMData(FlatBufferBuilder builder,
      int now_state = 0,
      StringOffset role_guidOffset = default(StringOffset),
      int index = 0,
      Offset<PRO.TurnBased.Flat.TurnState1_OperateData> state_operateOffset = default(Offset<PRO.TurnBased.Flat.TurnState1_OperateData>)) {
    builder.StartTable(4);
    TurnFSMData.AddStateOperate(builder, state_operateOffset);
    TurnFSMData.AddIndex(builder, index);
    TurnFSMData.AddRoleGuid(builder, role_guidOffset);
    TurnFSMData.AddNowState(builder, now_state);
    return TurnFSMData.EndTurnFSMData(builder);
  }

  public static void StartTurnFSMData(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddNowState(FlatBufferBuilder builder, int nowState) { builder.AddInt(0, nowState, 0); }
  public static void AddRoleGuid(FlatBufferBuilder builder, StringOffset roleGuidOffset) { builder.AddOffset(1, roleGuidOffset.Value, 0); }
  public static void AddIndex(FlatBufferBuilder builder, int index) { builder.AddInt(2, index, 0); }
  public static void AddStateOperate(FlatBufferBuilder builder, Offset<PRO.TurnBased.Flat.TurnState1_OperateData> stateOperateOffset) { builder.AddOffset(3, stateOperateOffset.Value, 0); }
  public static Offset<PRO.TurnBased.Flat.TurnFSMData> EndTurnFSMData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.TurnBased.Flat.TurnFSMData>(o);
  }
}


static public class TurnFSMDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*NowState*/, 4 /*int*/, 4, false)
      && verifier.VerifyString(tablePos, 6 /*RoleGuid*/, false)
      && verifier.VerifyField(tablePos, 8 /*Index*/, 4 /*int*/, 4, false)
      && verifier.VerifyTable(tablePos, 10 /*StateOperate*/, PRO.TurnBased.Flat.TurnState1_OperateDataVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct RoundState3_TurnData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static RoundState3_TurnData GetRootAsRoundState3_TurnData(ByteBuffer _bb) { return GetRootAsRoundState3_TurnData(_bb, new RoundState3_TurnData()); }
  public static RoundState3_TurnData GetRootAsRoundState3_TurnData(ByteBuffer _bb, RoundState3_TurnData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public RoundState3_TurnData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int NowRoundNum { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public int NowTurnIndex { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public PRO.TurnBased.Flat.TurnFSMData? TurnFsmList(int j) { int o = __p.__offset(8); return o != 0 ? (PRO.TurnBased.Flat.TurnFSMData?)(new PRO.TurnBased.Flat.TurnFSMData()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int TurnFsmListLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<PRO.TurnBased.Flat.RoundState3_TurnData> CreateRoundState3_TurnData(FlatBufferBuilder builder,
      int now_round_num = 0,
      int now_turn_index = 0,
      VectorOffset turn_fsm_listOffset = default(VectorOffset)) {
    builder.StartTable(3);
    RoundState3_TurnData.AddTurnFsmList(builder, turn_fsm_listOffset);
    RoundState3_TurnData.AddNowTurnIndex(builder, now_turn_index);
    RoundState3_TurnData.AddNowRoundNum(builder, now_round_num);
    return RoundState3_TurnData.EndRoundState3_TurnData(builder);
  }

  public static void StartRoundState3_TurnData(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddNowRoundNum(FlatBufferBuilder builder, int nowRoundNum) { builder.AddInt(0, nowRoundNum, 0); }
  public static void AddNowTurnIndex(FlatBufferBuilder builder, int nowTurnIndex) { builder.AddInt(1, nowTurnIndex, 0); }
  public static void AddTurnFsmList(FlatBufferBuilder builder, VectorOffset turnFsmListOffset) { builder.AddOffset(2, turnFsmListOffset.Value, 0); }
  public static VectorOffset CreateTurnFsmListVector(FlatBufferBuilder builder, Offset<PRO.TurnBased.Flat.TurnFSMData>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateTurnFsmListVectorBlock(FlatBufferBuilder builder, Offset<PRO.TurnBased.Flat.TurnFSMData>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateTurnFsmListVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<PRO.TurnBased.Flat.TurnFSMData>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateTurnFsmListVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<PRO.TurnBased.Flat.TurnFSMData>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartTurnFsmListVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<PRO.TurnBased.Flat.RoundState3_TurnData> EndRoundState3_TurnData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.TurnBased.Flat.RoundState3_TurnData>(o);
  }
}


static public class RoundState3_TurnDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*NowRoundNum*/, 4 /*int*/, 4, false)
      && verifier.VerifyField(tablePos, 6 /*NowTurnIndex*/, 4 /*int*/, 4, false)
      && verifier.VerifyVectorOfTables(tablePos, 8 /*TurnFsmList*/, PRO.TurnBased.Flat.TurnFSMDataVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
public struct RoundFSMData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static RoundFSMData GetRootAsRoundFSMData(ByteBuffer _bb) { return GetRootAsRoundFSMData(_bb, new RoundFSMData()); }
  public static RoundFSMData GetRootAsRoundFSMData(ByteBuffer _bb, RoundFSMData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public RoundFSMData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public string Guid { get { int o = __p.__offset(4); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetGuidBytes() { return __p.__vector_as_span<byte>(4, 1); }
#else
  public ArraySegment<byte>? GetGuidBytes() { return __p.__vector_as_arraysegment(4); }
#endif
  public byte[] GetGuidArray() { return __p.__vector_as_array<byte>(4); }
  public int NowState { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public string RoleHash(int j) { int o = __p.__offset(8); return o != 0 ? __p.__string(__p.__vector(o) + j * 4) : null; }
  public int RoleHashLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }
  public PRO.TurnBased.Flat.RoundState3_TurnData? State3Turn { get { int o = __p.__offset(10); return o != 0 ? (PRO.TurnBased.Flat.RoundState3_TurnData?)(new PRO.TurnBased.Flat.RoundState3_TurnData()).__assign(__p.__indirect(o + __p.bb_pos), __p.bb) : null; } }

  public static Offset<PRO.TurnBased.Flat.RoundFSMData> CreateRoundFSMData(FlatBufferBuilder builder,
      StringOffset guidOffset = default(StringOffset),
      int now_state = 0,
      VectorOffset role_hashOffset = default(VectorOffset),
      Offset<PRO.TurnBased.Flat.RoundState3_TurnData> state3_turnOffset = default(Offset<PRO.TurnBased.Flat.RoundState3_TurnData>)) {
    builder.StartTable(4);
    RoundFSMData.AddState3Turn(builder, state3_turnOffset);
    RoundFSMData.AddRoleHash(builder, role_hashOffset);
    RoundFSMData.AddNowState(builder, now_state);
    RoundFSMData.AddGuid(builder, guidOffset);
    return RoundFSMData.EndRoundFSMData(builder);
  }

  public static void StartRoundFSMData(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddGuid(FlatBufferBuilder builder, StringOffset guidOffset) { builder.AddOffset(0, guidOffset.Value, 0); }
  public static void AddNowState(FlatBufferBuilder builder, int nowState) { builder.AddInt(1, nowState, 0); }
  public static void AddRoleHash(FlatBufferBuilder builder, VectorOffset roleHashOffset) { builder.AddOffset(2, roleHashOffset.Value, 0); }
  public static VectorOffset CreateRoleHashVector(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateRoleHashVectorBlock(FlatBufferBuilder builder, StringOffset[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateRoleHashVectorBlock(FlatBufferBuilder builder, ArraySegment<StringOffset> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateRoleHashVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<StringOffset>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartRoleHashVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static void AddState3Turn(FlatBufferBuilder builder, Offset<PRO.TurnBased.Flat.RoundState3_TurnData> state3TurnOffset) { builder.AddOffset(3, state3TurnOffset.Value, 0); }
  public static Offset<PRO.TurnBased.Flat.RoundFSMData> EndRoundFSMData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<PRO.TurnBased.Flat.RoundFSMData>(o);
  }
}


static public class RoundFSMDataVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyString(tablePos, 4 /*Guid*/, false)
      && verifier.VerifyField(tablePos, 6 /*NowState*/, 4 /*int*/, 4, false)
      && verifier.VerifyVectorOfStrings(tablePos, 8 /*RoleHash*/, false)
      && verifier.VerifyTable(tablePos, 10 /*State3Turn*/, PRO.TurnBased.Flat.RoundState3_TurnDataVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}
