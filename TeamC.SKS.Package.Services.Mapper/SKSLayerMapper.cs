using System;
using System.Collections.Generic;
using System.Text;
using BL = TeamC.SKS.BusinessLogic.Entities;
using SVC = TeamC.SKS.Package.Services.DTOs.Models;
using DAL = TeamC.SKS.DataAccess.Entities;
using AutoMapper;
using NetTopologySuite.Geometries;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using System.IO;
using Newtonsoft.Json;

namespace TeamC.SKS.Package.Services.Mapper
{
    internal class GeoJsonConverter : IValueConverter<string, Geometry>, IValueConverter<Geometry, string>
    {
        public GeoJsonConverter()
        {
        }
        public Geometry Convert(string sourceMember, ResolutionContext context)
        {
            var serializer = GeoJsonSerializer.CreateDefault();
            var feature = (Feature)serializer.Deserialize(new StringReader(sourceMember), typeof(Feature));
            return feature.Geometry;
        }

        public string Convert(Geometry sourceMember, ResolutionContext context)
        {
            var serializer = GeoJsonSerializer.Create();
            var sw = new StringWriter();
            var geofeature = new Feature(sourceMember, new AttributesTable());
            serializer.Serialize(sw, geofeature, typeof(Feature));
            return sw.ToString();
        }
    }

    public class SKSLayerMapper : Profile
    {
        public SKSLayerMapper()
        {
            //DTO INTO BL
            CreateMap<SVC.WarehouseNextHops, BL.WarehouseNextHops>()
                .ReverseMap();

            CreateMap<SVC.Hop, BL.Hop>()
                .Include<SVC.Warehouse, BL.Warehouse>()
                .Include<SVC.Truck, BL.Truck>()
                .Include<SVC.Transferwarehouse, BL.Transferwarehouse>();

            CreateMap<SVC.Truck, BL.Truck>()
                .ForMember(dest => dest.RegionGeometry, opt => opt.ConvertUsing<GeoJsonConverter, string>(src => src.RegionGeoJson));
            CreateMap<SVC.Warehouse, BL.Warehouse>();
            CreateMap<SVC.Transferwarehouse, BL.Transferwarehouse>()
                .ForMember(dest => dest.RegionGeometry, opt => opt.ConvertUsing<GeoJsonConverter, string>(src => src.RegionGeoJson));

            CreateMap<SVC.Parcel, BL.Parcel>()
                .ForMember(dest => dest.TrackingId, opt => opt.Ignore())
                .ForMember(dest => dest.State, opt => opt.Ignore())
                .ForMember(dest => dest.VisitedHops, opt => opt.Ignore())
                .ForMember(dest => dest.FutureHops, opt => opt.Ignore());

            CreateMap<SVC.Receipient, BL.Receipient>()
                .ReverseMap();
            CreateMap<SVC.HopArrival, BL.HopArrival>()
                .ReverseMap();

            //BL INTO DTO
            CreateMap<BL.Parcel, SVC.NewParcelInfo>()
                .ForMember(dest => dest.TrackingId, opt => opt.MapFrom(src => src.TrackingId))
                .ForAllOtherMembers(dest => dest.Ignore());

            CreateMap<BL.Hop, SVC.Hop>()
                .Include<BL.Warehouse, SVC.Warehouse>()
                .Include<BL.Truck, SVC.Truck>()
                .Include<BL.Transferwarehouse, SVC.Transferwarehouse>();

            CreateMap<BL.Truck, SVC.Truck>()
                .ForMember(dest => dest.RegionGeoJson, opt => opt.ConvertUsing<GeoJsonConverter, Geometry>(src => src.RegionGeometry));
            CreateMap<BL.Warehouse, SVC.Warehouse>();
            CreateMap<BL.Transferwarehouse, SVC.Transferwarehouse>()
                .ForMember(dest => dest.RegionGeoJson, x => x.ConvertUsing<GeoJsonConverter, Geometry>(src => src.RegionGeometry));


            CreateMap<BL.Parcel, SVC.TrackingInformation>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
                .ForMember(dest => dest.VisitedHops, opt => opt.MapFrom(src => src.VisitedHops))
                .ForMember(dest => dest.FutureHops, opt => opt.MapFrom(src => src.FutureHops))
                .ForAllOtherMembers(dest => dest.Ignore());

            //BL INTO DAL
            CreateMap<BL.Parcel, DAL.Parcel>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());

            CreateMap<BL.Hop, DAL.Hop>()
                .Include<BL.Warehouse, DAL.Warehouse>()
                .Include<BL.Truck, DAL.Truck>()
                .Include<BL.Transferwarehouse, DAL.Transferwarehouse>();

            CreateMap<BL.HopArrival, DAL.HopArrival>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => src.DateTime));

            CreateMap<BL.Receipient, DAL.Receipient>()
                .ForMember(dest => dest.ID, opt => opt.Ignore());

            CreateMap<BL.Transferwarehouse, DAL.Transferwarehouse>();

            CreateMap<BL.Truck, DAL.Truck>();

            CreateMap<BL.Warehouse, DAL.Warehouse>();

            CreateMap<BL.WarehouseNextHops, DAL.WarehouseNextHops>()
                .ForMember(dest => dest.HopB, opt => opt.MapFrom(src => src.Hop));

            //DAL INTO BL

            CreateMap<DAL.Parcel, BL.Parcel>();

            CreateMap<DAL.Hop, BL.Hop>()
                .Include<DAL.Warehouse, BL.Warehouse>()
                .Include<DAL.Truck, BL.Truck>()
                .Include<DAL.Transferwarehouse, BL.Transferwarehouse>();

            CreateMap<DAL.HopArrival, BL.HopArrival>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.TimeStamp));

            CreateMap<DAL.Receipient, BL.Receipient>();

            CreateMap<DAL.Transferwarehouse, BL.Transferwarehouse>();

            CreateMap<DAL.Truck, BL.Truck>();

            CreateMap<DAL.Warehouse, BL.Warehouse>();

            CreateMap<DAL.WarehouseNextHops, BL.WarehouseNextHops>()
                .ForMember(dest => dest.Hop, opt => opt.MapFrom(src => src.HopB));

            //Webhook DTO - DAL without BL because of reasons

            CreateMap<SVC.WebhookResponse, DAL.Webhook>()
                .ReverseMap();

            CreateMap<BL.Parcel, SVC.WebhookMessage>();
        }
    }
}
