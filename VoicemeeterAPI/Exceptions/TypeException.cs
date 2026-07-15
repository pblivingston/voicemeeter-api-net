// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI.Exceptions;

public class TypeException : VmArgumentException
{
    public Type? Type { get; }

    public TypeException(Type type, string message, string paramName)
        : base(message, paramName)
        => this.Type = type;

    public TypeException(Type type, string message)
        : base(message)
        => this.Type = type;

    public TypeException(Type type)
        : base()
        => this.Type = type;

    public TypeException(string message)
        : base(message)
    { }

    public TypeException()
        : base()
    { }

    public override string Message
    {
        get
        {
            var lines = new List<string>();

            if (!string.IsNullOrWhiteSpace(base.Message))
            {
                lines.Add(base.Message);
            }

            if (this.Type is not null)
            {
                lines.Add($"Type: {this.Type.Name}");
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
