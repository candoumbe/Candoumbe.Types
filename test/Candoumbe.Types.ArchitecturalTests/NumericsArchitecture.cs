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

    private static readonly Architecture s_architecture = new ArchLoader().LoadAssemblies(s_numbersAssembly,
                                                                                        s_calendarsAssembly,
                                                                                        s_stringsAssembly,
                                                                                        s_coreAssembly)
                                                                                       .Build();



    [Fact]
    public void Should_only_depends_on_CoreLayer()
    {
        IArchRule numbersShouldBeInNumericNamespace = _.Types()
            .That()
            .Are(NumberLayer)
            .Should()
            .ResideInNamespace(typeof(Number<>).Namespace);

        const string baseD = @"Base(`\d+)?";
        GivenClassesConjunction numberClasses = Classes()
            .That()
            .Are(NumberLayer);

        IArchRule classesThatAreAbstract = numberClasses
            .And()
            .AreAbstract()
            .Should()
            .HaveNameEndingWith(baseD)
            .Because("Base suffix indicates that the type is abstract");

        IArchRule numberClassesShouldBeComparable = numberClasses
            .And()
            .AreNotAbstract()
            .Should()
            .ImplementInterface(typeof(IComparable<>))
            .AndShould()
            .BeRecord();

        IArchRule numberClassesShouldBeImmutable = numberClasses
            .Should()
            .FollowCustomCondition(clazz =>
                                   {
                                       bool hasNoWritableProperty = clazz.GetPropertyMembers().All(property => property.Writability != Writability.Writable);

                                       return new ConditionResult(clazz,
                                                                  hasNoWritableProperty,
                                                                  $"{clazz.FullName} cannot have any mutable property");
                                   }, "should be immutable");

        // Assert
        numbersShouldBeInNumericNamespace
            .And(classesThatAreAbstract)
            .And(numberClassesShouldBeComparable)
            .And(numberClassesShouldBeImmutable)
            .Check(s_architecture);
    }
}