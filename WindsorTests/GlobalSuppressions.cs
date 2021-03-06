using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly",
        MessageId = "TearDown", Scope = "member",
        Target = "WindsorTests.InterceptorLogging.Tests.LoggingTests.#TearDown()")
]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "WindsorTests.InterceptorLogging.Tests")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "WindsorTests.Lifestyle")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "WindsorTests.Lifestyle.Tests")]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "SetUp",
        Scope = "member", Target = "WindsorTests.InterceptorLogging.Tests.LoggingTests.#SetUp()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Scope = "member",
        Target =
            "WindsorTests.InterceptorLogging.DictionaryArgumentFormatter.#.ctor(System.Collections.Generic.IDictionary`2<System.Type,WindsorTests.InterceptorLogging.DictionaryArgumentFormatter+Spec>)"
    )]
// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.