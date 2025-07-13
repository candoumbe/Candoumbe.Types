using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Conditions;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Classes;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnitV3;
using Candoumbe.Types.Calendar;
using Candoumbe.Types.Numerics;
using Candoumbe.Types.Strings;
using Architecture = ArchUnitNET.Domain.Architecture;
using Assembly = System.Reflection.Assembly;

using _ = ArchUnitNET.Fluent.ArchRuleDefinition;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Candoumbe.Types.ArchitecturalTests;

public class NumericsArchitecture
{
    private static readonly Assembly s_numbersAssembly = typeof(Number<>).Assembly;
    private static readonly Assembly s_calendarsAssembly = typeof(DateOnlyRange).Assembly;
    private static readonly Assembly s_stringsAssembly = typeof(StringSegmentLinkedList).Assembly;
    private static readonly Assembly s_coreAssembly = typeof(IRange<,>).Assembly;

    private IObjectProvider<IType> NumberLayer => _.Types().That().ResideInAssembly(s_numbersAssembly);
    private IObjectProvider<IType> CalendarLayer => _.Types().That().ResideInAssembly(s_calendarsAssembly);
    private IObjectProvider<IType> StringsLayer => _.Types().That().ResideInAssembly(s_stringsAssembly);
    private IObjectProvider<IType> CoreLayer => _.Types().That().ResideInAssembly(s_coreAssembly);

    private IObjectProvider<Class> NumericClasses => _.Classes()
        .That()
        .Are(NumberLayer);

    private static readonly Architecture s_architecture = new ArchLoader().LoadAssemblies(s_numbersAssembly,
                                                                                          s_calendarsAssembly,
                                                                                          s_stringsAssembly,
                                                                                          s_coreAssembly)
                                                                                       .Build();

    [Fact]
    public void Types_in_NumericLayer_should_reside_in_the_same_namespace_as_Number_type()
    {
        // Act
        IArchRule numbersShouldBeInNumericNamespace = _.Types()
            .That()
            .Are(NumberLayer)
            .Should()
            .ResideInNamespace(typeof(Number<>).Namespace);

        // Assert
        numbersShouldBeInNumericNamespace.Check(s_architecture);
    }

    [Fact]
    public void Classes_that_are_abstract_should_have_name_ending_with_Base()
    {
        const string baseD = @"Base";
        IArchRule abstractClassesShouldHaveNameEndingWithBaseRule = Classes()
                .That()
                .Are(NumberLayer)
                .And()
                .AreAbstract()
                .And()
                .AreNot(typeof(Number<>))
                .Should()
                .FollowCustomCondition(clazz =>
                                       {
                                           ReadOnlySpan<char> name = clazz.Name;
                                           if (clazz.IsGeneric)
                                           {
                                               name = clazz.Name.AsSpan()[..clazz.Name.IndexOf('`')];
                                           }

                                           return new ConditionResult(clazz,
                                                                      name.EndsWith("Base"),
                                                                      @"should ends with ""Base""");

                                       }, "Base suffix indicates that the type is abstract");

        // Assert
        abstractClassesShouldHaveNameEndingWithBaseRule.Check(s_architecture);
    }

    [Fact]
    public void Classes_that_are_not_abstract_should_Be_record()
    {
        IArchRule abstractClassesShouldHaveNameEndingWithBaseRule = Classes()
            .That()
            .Are(NumericClasses)
            .And()
            .AreNotAbstract()
            .Should()
            .BeRecord()
            .AndShould()
            .FollowCustomCondition(clazz =>
                                   {
                                       bool hasNoWritableProperty = clazz.GetPropertyMembers().All(property => property.Writability is not Writability.Writable);

                                       return new ConditionResult(clazz,
                                                                  hasNoWritableProperty,
                                                                  $"{clazz.FullName} cannot have any mutable property");
                                   }, "should be immutable");

        // Assert
        abstractClassesShouldHaveNameEndingWithBaseRule.Check(s_architecture);
    }
}