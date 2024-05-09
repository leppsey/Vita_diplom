// Decompiled with JetBrains decompiler
// Type: PmToIfc.ePmType
// Assembly: PmToIfc, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 077B02CA-DD19-4238-9138-4E8E05A79472
// Assembly location: C:\Users\aspir\Desktop\Plantlinker\MAYTEST\Plantlinker\Plantlinker\PmToIfc\PmToIfc.dll

#nullable disable
namespace PmToIfc
{
  public enum ePmType
  {
    Model = 0,
    Equipment = 1,
    Parametric = 2,
    Shape = 3,
    Nozzle = 4,
    Prismatic = 5,
    Member = 11, // 0x0000000B
    Column = 12, // 0x0000000C
    Beam = 13, // 0x0000000D
    Plate = 14, // 0x0000000E
    Slab = 15, // 0x0000000F
    Wall = 16, // 0x00000010
    Footing = 18, // 0x00000012
    Hole = 20, // 0x00000014
    Handrail = 21, // 0x00000015
    Stair = 22, // 0x00000016
    Ladder = 23, // 0x00000017
    Foundation = 24, // 0x00000018
    Gridline = 25, // 0x00000019
    GridPlane = 26, // 0x0000001A
    Grid = 27, // 0x0000001B
    PipeRun = 31, // 0x0000001F
    PipeAsm = 32, // 0x00000020
    PipeComponent = 33, // 0x00000021
    Instrument = 34, // 0x00000022
    Specialty = 35, // 0x00000023
    PipeSupport = 36, // 0x00000024
    PipeSubRun = 37, // 0x00000025
    CwayRun = 41, // 0x00000029
    CwayComponent = 42, // 0x0000002A
    Conduit = 43, // 0x0000002B
    ConduitComponent = 44, // 0x0000002C
    ElectricalSupport = 45, // 0x0000002D
    Cable = 46, // 0x0000002E
    DuctRun = 51, // 0x00000033
    DuctComponent = 52, // 0x00000034
    DuctSupport = 55, // 0x00000037
    SpaceVolume = 61, // 0x0000003D
    SpaceSection = 62, // 0x0000003E
    AuxGraphic = 70, // 0x00000046
    AuxLine = 71, // 0x00000047
    AuxCircle = 72, // 0x00000048
    AuxRect = 73, // 0x00000049
    AuxArc = 74, // 0x0000004A
    Support = 81, // 0x00000051
    Hanger = 82, // 0x00000052
    IsometricSheet = 98, // 0x00000062
    Group = 99, // 0x00000063
    GenericSystem = 100, // 0x00000064
    AreaSystem = 101, // 0x00000065
    UnitSystem = 102, // 0x00000066
    GridSystem = 103, // 0x00000067
    EquipmentSystem = 104, // 0x00000068
    StructureSystem = 105, // 0x00000069
    PipingSystem = 106, // 0x0000006A
    DuctingSystem = 107, // 0x0000006B
    ElectricalSystem = 108, // 0x0000006C
    ConduitSystem = 109, // 0x0000006D
    Pipeline = 110, // 0x0000006E
    Ductwork = 111, // 0x0000006F
    Cableway = 112, // 0x00000070
    SpaceSystem = 201, // 0x000000C9
    DrawSystem = 202, // 0x000000CA
    ClipSystem = 203, // 0x000000CB
    AuxSystem = 301, // 0x0000012D
    IsometricSystem = 501, // 0x000001F5
    Multi = 999, // 0x000003E7
    Generic = 1000, // 0x000003E8
  }
}
