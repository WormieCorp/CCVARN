// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Performance",
	"CA1819:Properties should not return arrays",
	Justification = "This needs to be of a string array due to Spectre.Cli",
	Scope = "member",
	Target = "~P:CCVARN.Options.ParseOption.AdditionalOutputs")]
