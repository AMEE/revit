using System;
using System.Collections.Generic;
using AMEEClient.Util;

namespace AMEEClient.Model
{
    public class AmeeConstants
    {
        //    // 999,999,999,999,999,999.999
        public const int PRECISION = 18;
        public const int SCALE = 3;
        public const decimal ZERO = Decimal.Zero;
    }
    public enum AmeeObjectType
    {

        DATA_CATEGORY,
        DATA_ITEM,
        DRILL_DOWN,
        PROFILE,
        PROFILE_CATEGORY,
        PROFILE_ITEM,
        UNKNOWN,
        VALUE
    }
    public class AmeeValue : AmeeObject
    {
        public AmeeItem Item { get; set; }
        public AmeeObjectReference ItemRef { get; set; }
        public String Value { get; set; }
        public String Unit { get; set; }
        public String PerUnit { get; set; }
    }
    public class AmeeCategory : AmeeObject
    {
        public List<AmeeCategory> Categories { get; set; }
        public List<AmeeItem> Items { get; set; }
        public AmeeCategory Parent { get; set; }
        public AmeeObjectReference ParentRef { get; set; }
        public List<AmeeObjectReference> CategoryRefs { get; set; }
        public List<AmeeObjectReference> ItemRefs { get; set; }
        public int Page { get; set; }
        public Pager ItemsPager { get; set; }
    }
    public class AmeeItem : AmeeObject
    {
        public List<AmeeValue> Values { get; set; }
        public AmeeCategory Parent { get; set; }
        public AmeeObjectReference ParentRef { get; set; }
        public AmeeValue Value(String localPath){throw new NotImplementedException();}
        public string Label { get; set; }
        public List<AmeeObjectReference> ValueRefs() { throw new NotImplementedException(); }
        public List<AmeeObjectReference> ValueRefs (bool fetchIfNotFetched){throw new NotImplementedException();}
        
    public void AddValueRef(AmeeObjectReference vref) {
        ValueRefs().Add(vref);
    }
    public void ClearValueRefs()
    {
        ValueRefs().Clear();
    }
    }
    public class AmeeObject
    {
        public string Uri { get; set; }
        public string LocalPath { get; set; }
        public string ParentUri { get; set; }
        public AmeeObjectType ObjectType { get; set; }
        public AmeeObjectReference ObjectReference { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
    }

    public class AmeeObjectReference
    {
        public string Uri { get; set; }
        public string Path { get; set; }
        public string LocalPart { get; set; }
        public string ParentUri { get; set; }
        public string UriExceptFirstPart { get; set; }
        public string UriExceptFirstTwoParts { get; set; }
        public string UriFirstTwoParts { get; set; }
        public AmeeObjectReference Parent(AmeeObjectType objectType) { throw new NotImplementedException(); }
        public AmeeObjectReference Child(String localPath,AmeeObjectType objectType) { throw new NotImplementedException(); }
        public AmeeObjectType ObjectType { get; set; }


    }
}
