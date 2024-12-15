// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Usage", "CA2201:Do not raise reserved exception types", Justification = "<Pending>", Scope = "member", Target = "~P:jukumu.AppContext.Context.WorkingDirectory")]
[assembly: SuppressMessage(
    "Design",
    "CA1062:Validate arguments of public methods",
    Justification = "Handled appropriately at runtime",
    Scope = "namespaceanddescendants",
    Target = "~N:jukumu")]