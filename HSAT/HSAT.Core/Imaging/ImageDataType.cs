﻿namespace HSAT.Core.Imaging
{
    // same as gdal
    public enum ImageDataType
    {
        GDT_Unknown = 0,
        GDT_Byte = 1,
        GDT_Int8 = 14,
        GDT_UInt16 = 2,
        GDT_Int16 = 3,
        GDT_UInt32 = 4,
        GDT_Int32 = 5,
        GDT_UInt64 = 12,
        GDT_Int64 = 13,
        GDT_Float32 = 6,
        GDT_Float64 = 7,
        GDT_CInt16 = 8,
        GDT_CInt32 = 9,
        GDT_CFloat32 = 10,
        GDT_CFloat64 = 11,
        GDT_TypeCount = 0xF
    }
}
