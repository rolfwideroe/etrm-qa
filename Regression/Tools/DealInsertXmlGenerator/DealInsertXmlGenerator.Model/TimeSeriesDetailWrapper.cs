using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DealInsertXmlGenerator.Model
{
    public class TimeSeriesDetailWrapper
    {
        public string TimeSeriesId;
        public DateTime FromDate;
        public DateTime ToDate;
        public double Volume;
        public double Price;
        public double VolumeOutsideProfile;
        public double PricesOutsideProfile;
        public double RealizedVolume;
        public double RealizedPrice;


        public TimeSeriesDetailWrapper(string timeSeriesId,DateTime fromDate, DateTime toDate, double volume, double price, double volumeOutsideProfile, double pricesOutsideProfile, double realizedVolume, double realizedPrice)
        {
            TimeSeriesId = timeSeriesId;
            FromDate = fromDate;
            ToDate = toDate;
            Volume = volume;
            Price = price;
            VolumeOutsideProfile = volumeOutsideProfile;
            PricesOutsideProfile = pricesOutsideProfile;
            RealizedVolume = realizedVolume;
            RealizedPrice = realizedPrice;
        }
    }
}
