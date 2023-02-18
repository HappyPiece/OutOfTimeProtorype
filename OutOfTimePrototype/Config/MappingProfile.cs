using AutoMapper;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Config;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LectureHallDto, LectureHall>().IgnoreNullProperties();
        CreateMap<LectureHallUpdateModel, LectureHall>().IgnoreNullProperties();
    }
}

public static class MappingExpressionExtension
{
    public static void IgnoreNullProperties<TDestination, TSource>(
        this IMappingExpression<TDestination, TSource> expression)
    {
        expression.ForAllMembers(options =>
            options.Condition((_, _, srcMember) => srcMember != null));
    }
}