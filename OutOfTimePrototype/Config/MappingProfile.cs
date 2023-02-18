using System.Linq.Expressions;
using AutoMapper;
using LanguageExt.ClassInstances;
using OutOfTimePrototype.DAL.Models;
using OutOfTimePrototype.DTO;

namespace OutOfTimePrototype.Config;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<LectureHallDto, LectureHall>().IgnoreNullProperties().IgnoreProperty(dto => dto.Id);
        CreateMap<LectureHallUpdateDto, LectureHall>().IgnoreNullProperties();
        CreateMap<EducatorDto, Educator>().IgnoreNullProperties().IgnoreProperty(dto => dto.Id);
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