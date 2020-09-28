// This file is used by Code Analysis to maintain SuppressMessage attributes that are applied to
// this project. Project-level suppressions either have no target or are given a specific target and
// scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Usage",
	"CA2227:Collection properties should be read only",
	Justification = "This needs the setter due to serialization",
	Scope = "member",
	Target = "~P:CCVARN.Core.Configuration.Config.TypeScopes")]
