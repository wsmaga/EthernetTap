//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NetCon.export
{
    using System;
    using System.Collections.Generic;
    
    public partial class Target
    {
        public long id { get; set; }
        public System.DateTime date { get; set; }
        public byte[] rawData { get; set; }
        public NetCon.enums.DataType dataType { get; set; }
        public int arraySize { get; set; }
        public long TargetName_id { get; set; }
    
        public virtual TargetName TargetName { get; set; }
        public virtual VariableEvent VariableEvent { get; set; }
    }
}
