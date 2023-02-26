using System.Linq.Expressions;
using System.Xml;
using AutoMapper;
using OutOfTimePrototype.Dal.Models;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.Dto;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Configurations;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Disable Null
        CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<string?, string>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<DateTime?, DateTime>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<TimeSpan?, TimeSpan>().ConvertUsing((src, dest) => src ?? dest);
        CreateMap<Guid?, Guid>().ConvertUsing((src, dest) => src ?? dest);

        // Campus building
        CreateMap<CampusBuildingDto, CampusBuilding>().IgnoreNullProperties().IgnoreProperty(dto => dto.Id);

        // Lecture hall
        CreateMap<LectureHallDto, LectureHall>().IgnoreNullProperties().IgnoreProperty(dto => dto.Id);
        // .ForMember(hall => hall.HostBuilding, opts => opts.MapFrom((dto, hall) =>
        // {
        //     if (hall.HostBuilding is null) return new CampusBuilding { Id = dto.HostBuildingId };
        //
        //     hall.HostBuilding.Id = dto.HostBuildingId;
        //     return hall.HostBuilding;
        // }));
        CreateMap<LectureHallUpdateDto, LectureHall>().IgnoreNullProperties();

        CreateMap<EducatorDto, Educator>().IgnoreNullProperties().IgnoreProperty(dto => dto.Id);
        CreateMap<UserDto, User>().IgnoreNullProperties().IgnoreProperty(dto => dto.Id);
    }
}

public static class MappingExpressionExtension
{
    public static IMappingExpression<TSource, TDestination> IgnoreNullProperties<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression
    )
    {
        expression.ForAllMembers(options =>
            options.Condition((_, _, srcMember) => srcMember != null));
        return expression;
    }

    public static IMappingExpression<TSource, TDestination> IgnoreProperty<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression, Expression<Func<TDestination, object>> getProp
    )
    {
        expression.ForMember(getProp, configurationExpression => configurationExpression.Ignore());
        return expression;
    }
}