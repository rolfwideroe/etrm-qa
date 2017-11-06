using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TestWCFVolatilityService
{

    public class VolatilitySurfaceTestCase
    {
        public ExpectedResult ExpectedResult { get; set; }
        public InputData InputData { get; set; }
        public ExpectedVolatility[] ExpectedValues { get; set; }
    }
    public class ExpectedResult
    {
        public string Result { get; set; }
        public string ErrorMessage { get; set; }
    }

   public class InputData
    {
       public DateTime ReportDate { get; set; }

       public DateTime? OptionExpiry { get; set; }

       public DateTime OptionFromDate { get; set; }

       public DateTime OptionToDate { get; set; }

       public string LoadType { get; set; }

       public string PriceBookAppendix { get; set; }

       public string ReferenceAreaName { get; set; }

       public string TemplateName { get; set; }

       public string Resolution { get; set; }

       public double Strike { get; set; }

       public DateTime ExerciseTimeOfDay { get; set; }
    }

    public class ExpectedVolatility
    {
        [XmlAttribute]
        public DateTime FromDateTime { get; set; }

        [XmlAttribute]
        public double Value { get; set; }
    }

    //public class ExpectedValues
    //{
    //    [XmlAttribute("Date")]
    //    public DateTime Date { get; set; }
    //    [XmlAttribute("Value")]
    //    public double Value { get; set; }
    //    [XmlText()]
    //    public string Text { get; set; }
    //}
    //public class ExpectedProperties
    //{
    //    private string curveType;
    //    private string volumeUnit;
    //    private string timeZone;

    //    [XmlElement("CurveType", typeof(string))]
    //    public string CurveType
    //    {
    //        get { return this.curveType; }
    //        set { this.curveType = value; }
    //    }

    //    [XmlElement("VolumeUnit", typeof(string))]
    //    public string VolumeUnit
    //    {
    //        get { return this.volumeUnit; }
    //        set { this.volumeUnit = value; }
    //    }

    //    [XmlElement("TimeZone", typeof(string))]
    //    public string TimeZone
    //    {
    //        get { return this.timeZone; }
    //        set { this.timeZone = value; }
    //    }

    //}
}
