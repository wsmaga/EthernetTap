﻿using NetCon.util;
using System;

namespace NetCon.export.entities
{
    class Target
    {
        public long TargetID { get; set; }
        public DateTime Date { get; set; }
        public byte[] RawData { get; set; }
        public DataType DataType { get; set; }
        public long TargetNameID { get; set; }

        public virtual TargetName TargetName { get; set; }
        public virtual VariableEvent VariableEvent { get; set; }
    }
}